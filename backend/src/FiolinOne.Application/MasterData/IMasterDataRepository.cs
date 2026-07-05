using FiolinOne.Domain.MasterData;

namespace FiolinOne.Application.MasterData;

public interface IMasterDataRepository
{
    IQueryable<MasterDataEntity> Query(string type);
    Task<MasterDataEntity?> GetByIdAsync(string type, Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(string type, Guid id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string type, string code, Guid? excludedId, CancellationToken cancellationToken);
    Task<MasterDataEntity> CreateAsync(string type, string name, string code, bool isActive, int sortOrder, CancellationToken cancellationToken);
    void Delete(string type, MasterDataEntity entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
