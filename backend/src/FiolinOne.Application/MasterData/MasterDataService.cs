using FiolinOne.Domain.MasterData;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.MasterData;

public sealed class MasterDataService(IMasterDataRepository masterDataRepository) : IMasterDataService
{
    public async Task<IReadOnlyList<MasterDataDto>> GetItemsAsync(string type, string? search, CancellationToken cancellationToken)
    {
        var query = masterDataRepository.Query(type);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(item =>
                item.Name.ToLower().Contains(term) ||
                item.Code.ToLower().Contains(term));
        }

        var items = await query
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .ToListAsync(cancellationToken);

        return items.Select(ToDto).ToList();
    }

    public async Task<MasterDataDto?> GetItemAsync(string type, Guid id, CancellationToken cancellationToken)
    {
        var item = await masterDataRepository.GetByIdAsync(type, id, cancellationToken);

        return item is null ? null : ToDto(item);
    }

    public async Task<MasterDataDto> CreateItemAsync(string type, CreateMasterDataRequest request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        await EnsureNameIsUniqueAsync(type, name, null, cancellationToken);

        var code = await ResolveCodeAsync(type, request.Code, null, cancellationToken);
        var sortOrder = request.SortOrder ?? await GetNextSortOrderAsync(type, cancellationToken);

        var item = await masterDataRepository.CreateAsync(
            type,
            name,
            code,
            request.IsActive,
            sortOrder,
            cancellationToken);

        await masterDataRepository.SaveChangesAsync(cancellationToken);

        return ToDto(item);
    }

    public async Task<MasterDataDto?> UpdateItemAsync(
        string type,
        Guid id,
        UpdateMasterDataRequest request,
        CancellationToken cancellationToken)
    {
        var item = await masterDataRepository.GetByIdAsync(type, id, cancellationToken);

        if (item is null)
        {
            return null;
        }

        var name = request.Name.Trim();
        await EnsureNameIsUniqueAsync(type, name, id, cancellationToken);

        var code = await ResolveCodeAsync(type, request.Code, id, cancellationToken, item.Code);
        var sortOrder = request.SortOrder ?? item.SortOrder;

        item.Update(name, code, request.IsActive, sortOrder);
        await masterDataRepository.SaveChangesAsync(cancellationToken);

        return ToDto(item);
    }

    public async Task<IReadOnlyList<MasterDataDto>> ReorderItemsAsync(
        string type,
        ReorderMasterDataRequest request,
        CancellationToken cancellationToken)
    {
        var currentItemIds = await masterDataRepository.Query(type)
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.Name)
            .Select(item => item.Id)
            .ToListAsync(cancellationToken);

        if (currentItemIds.Count != request.ItemIds.Count || request.ItemIds.Except(currentItemIds).Any())
        {
            throw new InvalidOperationException("Sıralama kaydedilemedi. Lütfen listeyi yenileyip tekrar deneyin.");
        }

        var items = await masterDataRepository.GetByIdsAsync(type, request.ItemIds, cancellationToken);

        if (items.Count != request.ItemIds.Count)
        {
            throw new InvalidOperationException("Sıralama kaydedilemedi. Bazı kayıtlar bulunamadı.");
        }

        var lookup = items.ToDictionary(item => item.Id);

        for (var index = 0; index < request.ItemIds.Count; index++)
        {
            lookup[request.ItemIds[index]].ChangeSortOrder(index);
        }

        await masterDataRepository.SaveChangesAsync(cancellationToken);

        return (await GetItemsAsync(type, null, cancellationToken)).ToList();
    }

    public async Task<bool> DeleteItemAsync(string type, Guid id, CancellationToken cancellationToken)
    {
        var item = await masterDataRepository.GetByIdAsync(type, id, cancellationToken);

        if (item is null)
        {
            return false;
        }

        masterDataRepository.Delete(type, item);
        await masterDataRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<string> ResolveCodeAsync(
        string type,
        string? requestedCode,
        Guid? excludedId,
        CancellationToken cancellationToken,
        string? existingCode = null)
    {
        if (!string.IsNullOrWhiteSpace(requestedCode))
        {
            var code = requestedCode.Trim();
            await EnsureCodeIsUniqueAsync(type, code, excludedId, cancellationToken);
            return code;
        }

        if (!string.IsNullOrWhiteSpace(existingCode))
        {
            return existingCode;
        }

        return await GenerateCodeAsync(type, cancellationToken);
    }

    private async Task<string> GenerateCodeAsync(string type, CancellationToken cancellationToken)
    {
        var prefix = CodePrefix(type);
        var sequence = await masterDataRepository.Query(type).CountAsync(cancellationToken) + 1;

        while (true)
        {
            var code = $"{prefix}-{sequence:000000}";

            if (!await masterDataRepository.CodeExistsAsync(type, code, null, cancellationToken))
            {
                return code;
            }

            sequence++;
        }
    }

    private async Task<int> GetNextSortOrderAsync(string type, CancellationToken cancellationToken)
    {
        var lastSortOrder = await masterDataRepository.Query(type)
            .OrderByDescending(item => item.SortOrder)
            .Select(item => (int?)item.SortOrder)
            .FirstOrDefaultAsync(cancellationToken);

        return (lastSortOrder ?? -1) + 1;
    }

    private async Task EnsureCodeIsUniqueAsync(
        string type,
        string code,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await masterDataRepository.CodeExistsAsync(type, code, excludedId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Aynı kodda kayıt mevcut.");
        }
    }

    private async Task EnsureNameIsUniqueAsync(
        string type,
        string name,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await masterDataRepository.NameExistsAsync(type, name, excludedId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Aynı isimde kayıt mevcut.");
        }
    }

    private static string CodePrefix(string type)
    {
        return type.Trim().ToLowerInvariant() switch
        {
            "brands" => "BRD",
            "categories" => "CAT",
            "seasons" => "SEA",
            "colors" => "CLR",
            "sizes" => "SIZ",
            "fabric-types" => "FBT",
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Desteklenmeyen master data tipi.")
        };
    }

    private static MasterDataDto ToDto(MasterDataEntity item)
    {
        return new MasterDataDto(
            item.Id,
            item.Name,
            item.Code,
            item.IsActive,
            item.SortOrder,
            item.CreatedAt,
            item.UpdatedAt);
    }
}
