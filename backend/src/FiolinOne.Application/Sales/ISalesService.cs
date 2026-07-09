using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Sales;

public interface ISalesService
{
    Task<PagedResult<SalesOrderDto>> GetOrdersAsync(SalesQuery query, CancellationToken cancellationToken);
    Task<SalesOrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken);
    Task<SalesOrderDto> CreateOrderAsync(CreateSalesOrderRequest request, CancellationToken cancellationToken);
    Task<SalesOrderDto?> UpdateOrderAsync(Guid id, UpdateSalesOrderRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteOrderAsync(Guid id, CancellationToken cancellationToken);
}
