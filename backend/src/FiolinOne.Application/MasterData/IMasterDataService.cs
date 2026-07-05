namespace FiolinOne.Application.MasterData;

public interface IMasterDataService
{
    Task<IReadOnlyList<MasterDataDto>> GetItemsAsync(string type, string? search, CancellationToken cancellationToken);
    Task<MasterDataDto?> GetItemAsync(string type, Guid id, CancellationToken cancellationToken);
    Task<MasterDataDto> CreateItemAsync(string type, CreateMasterDataRequest request, CancellationToken cancellationToken);
    Task<MasterDataDto?> UpdateItemAsync(string type, Guid id, UpdateMasterDataRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteItemAsync(string type, Guid id, CancellationToken cancellationToken);
}
