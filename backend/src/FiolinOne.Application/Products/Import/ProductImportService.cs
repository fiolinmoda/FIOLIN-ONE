using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.Products.Import;

public sealed class ProductImportService(IApplicationDbContext dbContext) : IProductImportService
{
    private const string DefaultUser = "Administrator";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ProductImportPreviewDto> PreviewAsync(
        Stream fileStream,
        string fileName,
        ProductImportPreviewRequest request,
        CancellationToken cancellationToken)
    {
        EnsureXlsx(fileName);
        var bytes = await ReadBytesAsync(fileStream, cancellationToken);
        var workbook = XlsxWorkbook.Read(bytes);
        var signature = CreateSignature(workbook.Headers);
        var savedProfile = await dbContext.ProductImportProfiles
            .AsNoTracking()
            .Where(profile => profile.FileSignature == signature)
            .OrderByDescending(profile => profile.UpdatedAtUtc ?? profile.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        var mapping = request.Mapping ?? (savedProfile is null ? SuggestMapping(workbook.Headers) : DeserializeMapping(savedProfile.MappingJson));
        var rows = await AnalyzeRowsAsync(workbook, mapping, request.MissingMasterDataMode, false, cancellationToken);
        var summary = BuildSummary(rows);

        return new ProductImportPreviewDto(
            fileName,
            signature,
            workbook.Headers,
            mapping,
            savedProfile is null ? null : ToDto(savedProfile),
            summary,
            rows.Take(100).Select(row => new ProductImportPreviewRowDto(row.RowNumber, row.ModelCode, row.ProductName, row.Status, row.Errors)).ToList());
    }

    public async Task<ProductImportResultDto> ImportAsync(
        Stream fileStream,
        string fileName,
        string userName,
        ProductImportExecuteRequest request,
        CancellationToken cancellationToken)
    {
        EnsureXlsx(fileName);
        var stopwatch = Stopwatch.StartNew();
        var bytes = await ReadBytesAsync(fileStream, cancellationToken);
        var workbook = XlsxWorkbook.Read(bytes);
        var signature = CreateSignature(workbook.Headers);
        var rows = await AnalyzeRowsAsync(workbook, request.Mapping, request.MissingMasterDataMode, true, cancellationToken);

        if (request.MissingMasterDataMode.Equals("Cancel", StringComparison.OrdinalIgnoreCase)
            && rows.Any(row => row.Errors.Any(error => error.Contains("bulunamadı", StringComparison.CurrentCultureIgnoreCase))))
        {
            throw new InvalidOperationException("Eksik sistem tanımları bulundu. Lütfen tanımları oluşturun veya içe aktarma seçeneğini değiştirin.");
        }

        var insertableRows = rows
            .Where(row => row.Status == ProductImportRowStatus.New)
            .GroupBy(row => row.ModelCode, StringComparer.CurrentCultureIgnoreCase)
            .Select(group => group.First())
            .ToList();

        var products = new List<Product>(insertableRows.Count);
        var variants = new List<ProductVariant>(insertableRows.Count);

        foreach (var row in insertableRows)
        {
            var product = new Product(row.ModelCode, row.ProductName, row.BrandId, row.CategoryId, row.SeasonId, "Active");
            products.Add(product);

            if (row.ColorId.HasValue && row.SizeId.HasValue)
            {
                variants.Add(new ProductVariant(
                    product.Id,
                    row.ColorId.Value,
                    row.SizeId.Value,
                    CreateBarcode(row.ModelCode, row.ColorName, row.SizeName),
                    null,
                    row.Stock,
                    "Active"));
            }
        }

        await dbContext.Products.AddRangeAsync(products, cancellationToken);
        await dbContext.ProductVariants.AddRangeAsync(variants, cancellationToken);

        if (request.SaveProfile)
        {
            await SaveProfileAsync(signature, request.ProfileName, request.Mapping, userName, cancellationToken);
        }

        stopwatch.Stop();
        var summary = BuildSummary(rows);
        var errorRows = rows
            .Where(row => row.Errors.Count > 0)
            .Select(row => new ProductImportErrorRowDto(row.RowNumber, row.ModelCode, row.ProductName, string.Join(" | ", row.Errors)))
            .ToList();

        var history = new ProductImportHistory(
            string.IsNullOrWhiteSpace(userName) ? DefaultUser : userName,
            fileName,
            summary.Total,
            insertableRows.Count,
            summary.ExistingProducts,
            summary.Skipped,
            summary.Error,
            (int)stopwatch.ElapsedMilliseconds,
            "Completed",
            errorRows.Count == 0 ? null : "Hatalı satırlar oluştu.");

        await dbContext.ProductImportHistories.AddAsync(history, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ProductImportResultDto(summary, insertableRows.Count, summary.ExistingProducts, summary.Skipped, summary.Error, (int)stopwatch.ElapsedMilliseconds, errorRows);
    }

    public async Task<IReadOnlyList<ProductImportHistoryDto>> GetHistoryAsync(CancellationToken cancellationToken)
    {
        return await dbContext.ProductImportHistories
            .AsNoTracking()
            .OrderByDescending(history => history.CreatedAtUtc)
            .Take(50)
            .Select(history => new ProductImportHistoryDto(
                history.Id,
                history.ImportedAt,
                history.UserName,
                history.FileName,
                history.TotalRecords,
                history.InsertedRecords,
                history.ExistingRecords,
                history.SkippedRecords,
                history.ErrorRecords,
                history.DurationMilliseconds,
                history.Status,
                history.Notes))
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<AnalyzedRow>> AnalyzeRowsAsync(
        XlsxWorkbook workbook,
        ProductImportMapping mapping,
        string missingMasterDataMode,
        bool allowCreateMasterData,
        CancellationToken cancellationToken)
    {
        var products = await dbContext.Products.AsNoTracking().Select(product => product.ProductCode).ToListAsync(cancellationToken);
        var existingProductCodes = products.ToHashSet(StringComparer.CurrentCultureIgnoreCase);
        var brands = await dbContext.Brands.ToListAsync(cancellationToken);
        var categories = await dbContext.Categories.ToListAsync(cancellationToken);
        var seasons = await dbContext.Seasons.ToListAsync(cancellationToken);
        var colors = await dbContext.Colors.ToListAsync(cancellationToken);
        var sizes = await dbContext.Sizes.ToListAsync(cancellationToken);
        var fabricTypes = await dbContext.FabricTypes.ToListAsync(cancellationToken);
        var localCodes = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        var analyzedRows = new List<AnalyzedRow>(workbook.Rows.Count);
        var mode = string.IsNullOrWhiteSpace(missingMasterDataMode) ? "Cancel" : missingMasterDataMode.Trim();

        foreach (var row in workbook.Rows)
        {
            var analyzed = new AnalyzedRow(row.RowNumber)
            {
                ModelCode = GetValue(row, mapping.ModelCode),
                ProductName = GetValue(row, mapping.ProductName),
                BrandName = GetValue(row, mapping.Brand),
                CategoryName = GetValue(row, mapping.Category),
                SeasonName = GetValue(row, mapping.Season),
                ColorName = GetValue(row, mapping.Color),
                SizeName = GetValue(row, mapping.Size),
                FabricTypeName = GetValue(row, mapping.FabricType),
                Stock = ParseStock(GetValue(row, mapping.Stock))
            };

            if (string.IsNullOrWhiteSpace(analyzed.ModelCode))
            {
                analyzed.Errors.Add("Model Kodu zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(analyzed.ProductName))
            {
                analyzed.Errors.Add("Ürün Adı zorunludur.");
            }

            if (analyzed.Stock < 0)
            {
                analyzed.Errors.Add("Stok negatif olamaz.");
            }

            if (!string.IsNullOrWhiteSpace(analyzed.ModelCode) && existingProductCodes.Contains(analyzed.ModelCode))
            {
                analyzed.Status = ProductImportRowStatus.Existing;
            }
            else if (!string.IsNullOrWhiteSpace(analyzed.ModelCode) && !localCodes.Add(analyzed.ModelCode))
            {
                analyzed.Status = ProductImportRowStatus.Skipped;
                analyzed.Errors.Add("Dosya içinde aynı Model Kodu tekrar ediyor.");
            }

            ResolveMasterData(analyzed, brands, categories, seasons, colors, sizes, fabricTypes, mode, allowCreateMasterData);

            if (analyzed.Errors.Count > 0 && analyzed.Status != ProductImportRowStatus.Existing)
            {
                analyzed.Status = analyzed.Errors.Any(error => error.Contains("bulunamadı", StringComparison.CurrentCultureIgnoreCase))
                    ? ProductImportRowStatus.Skipped
                    : ProductImportRowStatus.Error;
            }
            else if (analyzed.Status != ProductImportRowStatus.Existing && analyzed.Status != ProductImportRowStatus.Skipped)
            {
                analyzed.Status = ProductImportRowStatus.New;
            }

            analyzedRows.Add(analyzed);
        }

        return analyzedRows;
    }

    private void ResolveMasterData(
        AnalyzedRow row,
        List<Brand> brands,
        List<Category> categories,
        List<Season> seasons,
        List<Color> colors,
        List<Size> sizes,
        List<FabricType> fabricTypes,
        string mode,
        bool allowCreate)
    {
        row.BrandId = Resolve(row.BrandName, brands, dbContext.Brands, name => new Brand(name, CreateCode(name), true, 0), mode, allowCreate, row.Errors, "Marka");
        row.CategoryId = Resolve(row.CategoryName, categories, dbContext.Categories, name => new Category(name, CreateCode(name), true, 0), mode, allowCreate, row.Errors, "Kategori");
        row.SeasonId = Resolve(row.SeasonName, seasons, dbContext.Seasons, name => new Season(name, CreateCode(name), true, 0), mode, allowCreate, row.Errors, "Sezon");
        row.ColorId = Resolve(row.ColorName, colors, dbContext.Colors, name => new Color(name, CreateCode(name), true, 0), mode, allowCreate, row.Errors, "Renk");
        row.SizeId = Resolve(row.SizeName, sizes, dbContext.Sizes, name => new Size(name, CreateCode(name), true, 0), mode, allowCreate, row.Errors, "Beden");
        _ = Resolve(row.FabricTypeName, fabricTypes, dbContext.FabricTypes, name => new FabricType(name, CreateCode(name), true, 0), mode, allowCreate, row.Errors, "Kumaş Tipi");
    }

    private static Guid? Resolve<T>(
        string value,
        List<T> items,
        DbSet<T> dbSet,
        Func<string, T> create,
        string mode,
        bool allowCreate,
        List<string> errors,
        string label)
        where T : MasterDataEntity
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var existing = items.FirstOrDefault(item =>
            string.Equals(item.Name, value.Trim(), StringComparison.CurrentCultureIgnoreCase) ||
            string.Equals(item.Code, value.Trim(), StringComparison.CurrentCultureIgnoreCase));

        if (existing is not null)
        {
            return existing.Id;
        }

        if (mode.Equals("Create", StringComparison.OrdinalIgnoreCase) && !allowCreate)
        {
            return Guid.Empty;
        }

        if (mode.Equals("Create", StringComparison.OrdinalIgnoreCase) && allowCreate)
        {
            var created = create(value.Trim());
            items.Add(created);
            dbSet.Add(created);
            return created.Id;
        }

        errors.Add($"{label} sistemde bulunamadı: {value}");
        return null;
    }

    private async Task SaveProfileAsync(string signature, string? profileName, ProductImportMapping mapping, string userName, CancellationToken cancellationToken)
    {
        var mappingJson = JsonSerializer.Serialize(mapping, JsonOptions);
        var existing = await dbContext.ProductImportProfiles.FirstOrDefaultAsync(profile => profile.FileSignature == signature, cancellationToken);

        if (existing is null)
        {
            await dbContext.ProductImportProfiles.AddAsync(new ProductImportProfile(
                string.IsNullOrWhiteSpace(profileName) ? "Fiolin Ürün Stok Sistemi" : profileName.Trim(),
                signature,
                mappingJson,
                string.IsNullOrWhiteSpace(userName) ? DefaultUser : userName),
                cancellationToken);
            return;
        }

        existing.Update(string.IsNullOrWhiteSpace(profileName) ? existing.ProfileName : profileName.Trim(), mappingJson, userName);
    }

    private static ProductImportSummaryDto BuildSummary(IReadOnlyList<AnalyzedRow> rows)
    {
        return new ProductImportSummaryDto(
            rows.Count,
            rows.Count(row => row.Status is ProductImportRowStatus.New or ProductImportRowStatus.Existing),
            rows.Count(row => row.Errors.Any(error => error.Contains("zorunludur", StringComparison.CurrentCultureIgnoreCase))),
            rows.Count(row => row.Status == ProductImportRowStatus.Error),
            rows.Count(row => row.Status == ProductImportRowStatus.New),
            rows.Count(row => row.Status == ProductImportRowStatus.Existing),
            rows.Count(row => row.Status == ProductImportRowStatus.Skipped));
    }

    private static ProductImportMapping SuggestMapping(IReadOnlyList<string> headers)
    {
        return new ProductImportMapping(
            Find(headers, "model", "model kodu", "ürün kodu", "kod"),
            Find(headers, "ürün adı", "urun adi", "model adı", "ad"),
            Find(headers, "marka"),
            Find(headers, "kategori"),
            Find(headers, "sezon"),
            Find(headers, "renk"),
            Find(headers, "beden"),
            Find(headers, "kumaş", "kumas", "fabric"),
            Find(headers, "alış", "alis", "purchase"),
            Find(headers, "satış", "satis", "sale"),
            Find(headers, "stok", "stock"));
    }

    private static string? Find(IReadOnlyList<string> headers, params string[] candidates)
    {
        return headers.FirstOrDefault(header => candidates.Any(candidate => Normalize(header).Contains(Normalize(candidate), StringComparison.OrdinalIgnoreCase)));
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant()
            .Replace("ı", "i", StringComparison.Ordinal)
            .Replace("ğ", "g", StringComparison.Ordinal)
            .Replace("ü", "u", StringComparison.Ordinal)
            .Replace("ş", "s", StringComparison.Ordinal)
            .Replace("ö", "o", StringComparison.Ordinal)
            .Replace("ç", "c", StringComparison.Ordinal);
    }

    private static string GetValue(XlsxRow row, string? header)
    {
        return !string.IsNullOrWhiteSpace(header) && row.Values.TryGetValue(header, out var value) ? value.Trim() : string.Empty;
    }

    private static int ParseStock(string value)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.CurrentCulture, out var stock))
        {
            return stock;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var invariantStock))
        {
            return (int)Math.Round(invariantStock);
        }

        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var currentStock)
            ? (int)Math.Round(currentStock)
            : 0;
    }

    private static string CreateCode(string value)
    {
        var normalized = Regex.Replace(Normalize(value).ToUpperInvariant(), "[^A-Z0-9]", string.Empty);
        return string.IsNullOrWhiteSpace(normalized) ? Guid.NewGuid().ToString("N")[..8].ToUpperInvariant() : normalized[..Math.Min(normalized.Length, 40)];
    }

    private static string CreateBarcode(string modelCode, string color, string size)
    {
        var barcode = CreateCode($"{modelCode}{color}{size}");
        return barcode[..Math.Min(barcode.Length, 60)];
    }

    private static ProductImportMapping DeserializeMapping(string mappingJson)
    {
        return JsonSerializer.Deserialize<ProductImportMapping>(mappingJson, JsonOptions) ?? new ProductImportMapping(null, null, null, null, null, null, null, null, null, null, null);
    }

    private static ProductImportProfileDto ToDto(ProductImportProfile profile)
    {
        return new ProductImportProfileDto(profile.Id, profile.ProfileName, profile.FileSignature, DeserializeMapping(profile.MappingJson), profile.CreatedAtUtc, profile.UpdatedAtUtc);
    }

    private static string CreateSignature(IReadOnlyList<string> headers)
    {
        var raw = string.Join("|", headers.Select(Normalize));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
    }

    private static async Task<byte[]> ReadBytesAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken);
        return memory.ToArray();
    }

    private static void EnsureXlsx(string fileName)
    {
        if (!fileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Sadece .xlsx dosyaları kabul edilir.");
        }
    }

    private sealed class AnalyzedRow(int rowNumber)
    {
        public int RowNumber { get; } = rowNumber;
        public string ModelCode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public string FabricTypeName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public Guid? BrandId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SeasonId { get; set; }
        public Guid? ColorId { get; set; }
        public Guid? SizeId { get; set; }
        public string Status { get; set; } = ProductImportRowStatus.New;
        public List<string> Errors { get; } = [];
    }

    private static class ProductImportRowStatus
    {
        public const string New = "New";
        public const string Existing = "Existing";
        public const string Skipped = "Skipped";
        public const string Error = "Error";
    }

    private sealed record XlsxWorkbook(IReadOnlyList<string> Headers, IReadOnlyList<XlsxRow> Rows)
    {
        public static XlsxWorkbook Read(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            var sharedStrings = ReadSharedStrings(archive);
            var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml")
                ?? archive.GetEntry(@"xl\worksheets\sheet1.xml")
                ?? throw new InvalidOperationException("Excel dosyasında ilk sayfa bulunamadı.");
            using var sheetStream = sheetEntry.Open();
            var document = XDocument.Load(sheetStream);
            XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            var rows = document.Descendants(main + "row").ToList();

            if (rows.Count == 0)
            {
                return new XlsxWorkbook([], []);
            }

            var headers = ReadCells(rows[0], sharedStrings, main).Values.Select(value => value.Trim()).Where(value => value.Length > 0).ToList();
            var dataRows = rows.Skip(1)
                .Select(row =>
                {
                    var cells = ReadCells(row, sharedStrings, main);
                    var values = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
                    for (var index = 0; index < headers.Count; index++)
                    {
                        values[headers[index]] = cells.GetValueOrDefault(index + 1, string.Empty);
                    }

                    return new XlsxRow(int.Parse(row.Attribute("r")?.Value ?? "0"), values);
                })
                .Where(row => row.Values.Values.Any(value => !string.IsNullOrWhiteSpace(value)))
                .ToList();

            return new XlsxWorkbook(headers, dataRows);
        }

        private static List<string> ReadSharedStrings(ZipArchive archive)
        {
            var entry = archive.GetEntry("xl/sharedStrings.xml");
            if (entry is null)
            {
                return [];
            }

            using var stream = entry.Open();
            var document = XDocument.Load(stream);
            XNamespace main = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            return document.Descendants(main + "si").Select(item => string.Concat(item.Descendants(main + "t").Select(text => text.Value))).ToList();
        }

        private static Dictionary<int, string> ReadCells(XElement row, List<string> sharedStrings, XNamespace main)
        {
            var values = new Dictionary<int, string>();
            foreach (var cell in row.Elements(main + "c"))
            {
                var reference = cell.Attribute("r")?.Value ?? string.Empty;
                var columnIndex = ColumnIndex(reference);
                var raw = cell.Element(main + "v")?.Value ?? string.Empty;
                var type = cell.Attribute("t")?.Value;
                values[columnIndex] = type == "s" && int.TryParse(raw, out var sharedIndex) && sharedIndex >= 0 && sharedIndex < sharedStrings.Count
                    ? sharedStrings[sharedIndex]
                    : raw;
            }

            return values;
        }

        private static int ColumnIndex(string reference)
        {
            var letters = new string(reference.TakeWhile(char.IsLetter).ToArray()).ToUpperInvariant();
            var index = 0;
            foreach (var letter in letters)
            {
                index = (index * 26) + letter - 'A' + 1;
            }

            return index;
        }
    }

    private sealed record XlsxRow(int RowNumber, IReadOnlyDictionary<string, string> Values);
}
