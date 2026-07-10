using FiolinOne.Application.MasterData;
using FiolinOne.Domain.MasterData;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Color = FiolinOne.Domain.MasterData.Color;
using Size = FiolinOne.Domain.MasterData.Size;

namespace FiolinOne.Infrastructure.MasterData;

public sealed class MasterDataRepository(ApplicationDbContext dbContext) : IMasterDataRepository
{
    public IQueryable<MasterDataEntity> Query(string type)
    {
        return Set(type).AsNoTracking();
    }

    public async Task<MasterDataEntity?> GetByIdAsync(string type, Guid id, CancellationToken cancellationToken)
    {
        return await Set(type).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MasterDataEntity>> GetByIdsAsync(
        string type,
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await Set(type)
            .Where(item => ids.Contains(item.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(string type, Guid id, CancellationToken cancellationToken)
    {
        return Set(type).AnyAsync(item => item.Id == id, cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string type, string code, Guid? excludedId, CancellationToken cancellationToken)
    {
        return Set(type).AnyAsync(
            item => item.Code == code && (!excludedId.HasValue || item.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<bool> NameExistsAsync(string type, string name, Guid? excludedId, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToLower();

        return Set(type).AnyAsync(
            item => item.Name.ToLower() == normalizedName && (!excludedId.HasValue || item.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task<MasterDataEntity> CreateAsync(
        string type,
        string name,
        string code,
        bool isActive,
        int sortOrder,
        CancellationToken cancellationToken)
    {
        var item = Create(type, name, code, isActive, sortOrder);

        await dbContext.AddAsync(item, cancellationToken);

        return item;
    }

    public void Delete(string type, MasterDataEntity entity)
    {
        dbContext.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<MasterDataEntity> Set(string type)
    {
        return Normalize(type) switch
        {
            "brands" => dbContext.Brands,
            "categories" => dbContext.Categories,
            "seasons" => dbContext.Seasons,
            "colors" => dbContext.Colors,
            "sizes" => dbContext.Sizes,
            "fabric-types" => dbContext.FabricTypes,
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported master data type.")
        };
    }

    private static MasterDataEntity Create(string type, string name, string code, bool isActive, int sortOrder)
    {
        return Normalize(type) switch
        {
            "brands" => new Brand(name, code, isActive, sortOrder),
            "categories" => new Category(name, code, isActive, sortOrder),
            "seasons" => new Season(name, code, isActive, sortOrder),
            "colors" => new Color(name, code, isActive, sortOrder),
            "sizes" => new Size(name, code, isActive, sortOrder),
            "fabric-types" => new FabricType(name, code, isActive, sortOrder),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Unsupported master data type.")
        };
    }

    private static string Normalize(string type)
    {
        return type.Trim().ToLowerInvariant();
    }
}
