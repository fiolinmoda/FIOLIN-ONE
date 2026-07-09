using FiolinOne.Application.Common.Models;
using FiolinOne.Domain.Products;
using FiolinOne.Domain.Sales;

namespace FiolinOne.Application.Sales;

public interface ISalesRepository
{
    Task<PagedResult<SalesOrder>> GetOrdersAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<SalesOrder?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> SalesOrderNumberExistsAsync(string salesOrderNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task<ProductVariant?> GetVariantByIdAsync(Guid productVariantId, CancellationToken cancellationToken);
    Task AddOrderAsync(SalesOrder order, CancellationToken cancellationToken);
    Task ReplaceItemsAsync(SalesOrder order, IReadOnlyList<SalesOrderItem> items, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
