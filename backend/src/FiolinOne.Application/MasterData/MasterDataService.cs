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
        await EnsureCodeIsUniqueAsync(type, request.Code, null, cancellationToken);

        var item = await masterDataRepository.CreateAsync(
            type,
            request.Name.Trim(),
            request.Code.Trim(),
            request.IsActive,
            request.SortOrder,
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

        await EnsureCodeIsUniqueAsync(type, request.Code, id, cancellationToken);

        item.Update(request.Name.Trim(), request.Code.Trim(), request.IsActive, request.SortOrder);
        await masterDataRepository.SaveChangesAsync(cancellationToken);

        return ToDto(item);
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

    private async Task EnsureCodeIsUniqueAsync(
        string type,
        string code,
        Guid? excludedId,
        CancellationToken cancellationToken)
    {
        var exists = await masterDataRepository.CodeExistsAsync(type, code.Trim(), excludedId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Code already exists.");
        }
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
