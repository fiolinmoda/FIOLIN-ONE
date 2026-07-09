using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Application.Common.Models;
using FiolinOne.Domain.Products;
using FiolinOne.Domain.Sales;

namespace FiolinOne.Application.Sales;

public sealed class SalesService(ISalesRepository salesRepository, IDocumentNumberGenerator documentNumberGenerator) : ISalesService
{
    private static readonly HashSet<string> ValidStatuses = [SalesOrderStatuses.Draft, SalesOrderStatuses.Approved, SalesOrderStatuses.Completed, SalesOrderStatuses.Cancelled];

    public async Task<PagedResult<SalesOrderDto>> GetOrdersAsync(SalesQuery query, CancellationToken cancellationToken)
    {
        var result = await salesRepository.GetOrdersAsync(query.ToParameters(), cancellationToken);

        return new PagedResult<SalesOrderDto>(
            result.Items.Select(ToDto).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages);
    }

    public async Task<SalesOrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await salesRepository.GetOrderByIdAsync(id, cancellationToken);

        return order is null ? null : ToDto(order);
    }

    public async Task<SalesOrderDto> CreateOrderAsync(CreateSalesOrderRequest request, CancellationToken cancellationToken)
    {
        ValidateStatus(request.Status);
        var salesOrderNumber = await GetDocumentNumberAsync(request.SalesOrderNumber, cancellationToken);
        await EnsureOrderNumberIsUniqueAsync(salesOrderNumber, null, cancellationToken);

        var orderId = Guid.NewGuid();
        var items = await ToItemsAsync(orderId, request.Items, cancellationToken);
        var order = new SalesOrder(
            orderId,
            salesOrderNumber,
            request.CustomerName.Trim(),
            ToUtc(request.OrderDate),
            request.Status.Trim(),
            NormalizeOptional(request.Notes),
            items);

        if (order.Status == SalesOrderStatuses.Completed)
        {
            ApplyStockDecrease(items);
        }

        await salesRepository.AddOrderAsync(order, cancellationToken);
        await salesRepository.SaveChangesAsync(cancellationToken);

        order = await salesRepository.GetOrderByIdAsync(order.Id, cancellationToken) ?? order;

        return ToDto(order);
    }

    public async Task<SalesOrderDto?> UpdateOrderAsync(Guid id, UpdateSalesOrderRequest request, CancellationToken cancellationToken)
    {
        ValidateStatus(request.Status);
        var order = await salesRepository.GetOrderByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return null;
        }

        if (order.Status is SalesOrderStatuses.Completed or SalesOrderStatuses.Cancelled)
        {
            throw new InvalidOperationException("Tamamlanan veya iptal edilen satış siparişi değiştirilemez.");
        }

        await EnsureOrderNumberIsUniqueAsync(request.SalesOrderNumber, id, cancellationToken);
        var items = await ToItemsAsync(id, request.Items, cancellationToken);

        if (request.Status.Trim() == SalesOrderStatuses.Completed)
        {
            ApplyStockDecrease(items);
        }

        order.Update(
            request.CustomerName.Trim(),
            ToUtc(request.OrderDate),
            request.Status.Trim(),
            NormalizeOptional(request.Notes),
            items);

        await salesRepository.ReplaceItemsAsync(order, items, cancellationToken);
        await salesRepository.SaveChangesAsync(cancellationToken);

        order = await salesRepository.GetOrderByIdAsync(id, cancellationToken) ?? order;

        return ToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await salesRepository.GetOrderByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return false;
        }

        if (order.Status == SalesOrderStatuses.Completed)
        {
            throw new InvalidOperationException("Stok hareketi oluşan tamamlanmış satış siparişi silinemez.");
        }

        order.SoftDelete();
        await salesRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<IReadOnlyList<SalesOrderItem>> ToItemsAsync(Guid orderId, IReadOnlyList<SalesOrderItemRequest> requests, CancellationToken cancellationToken)
    {
        var items = new List<SalesOrderItem>();

        foreach (var request in requests)
        {
            var variant = await salesRepository.GetVariantByIdAsync(request.ProductVariantId, cancellationToken)
                ?? throw new InvalidOperationException("Ürün varyantı bulunamadı.");

            items.Add(new SalesOrderItem(orderId, variant, request.Quantity, request.UnitPrice));
        }

        return items;
    }

    private static void ApplyStockDecrease(IReadOnlyList<SalesOrderItem> items)
    {
        foreach (var item in items)
        {
            if (item.ProductVariant is null)
            {
                throw new InvalidOperationException("Ürün varyantı bulunamadı.");
            }

            item.ProductVariant.DecreaseStock(item.Quantity);
        }
    }

    private async Task<string> GetDocumentNumberAsync(string requestedNumber, CancellationToken cancellationToken)
    {
        return string.IsNullOrWhiteSpace(requestedNumber)
            ? await documentNumberGenerator.GenerateAsync(DocumentNumberTypes.SalesOrder, cancellationToken)
            : requestedNumber.Trim();
    }

    private async Task EnsureOrderNumberIsUniqueAsync(string salesOrderNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await salesRepository.SalesOrderNumberExistsAsync(salesOrderNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Bu satış siparişi numarası zaten kullanılıyor.");
        }
    }

    private static void ValidateStatus(string status)
    {
        if (!ValidStatuses.Contains(status.Trim()))
        {
            throw new InvalidOperationException("Geçersiz satış siparişi durumu.");
        }
    }

    private static SalesOrderDto ToDto(SalesOrder order)
    {
        return new SalesOrderDto(
            order.Id,
            order.SalesOrderNumber,
            order.CustomerName,
            order.OrderDate,
            order.Status,
            order.TotalAmount,
            order.Notes,
            order.Items.Where(item => !item.IsDeleted).Select(ToDto).ToList(),
            order.CreatedAt,
            order.UpdatedAt);
    }

    private static SalesOrderItemDto ToDto(SalesOrderItem item)
    {
        var variant = item.ProductVariant;
        var product = variant?.Product;

        return new SalesOrderItemDto(
            item.Id,
            item.ProductVariantId,
            product?.ProductCode ?? string.Empty,
            product?.ProductName ?? string.Empty,
            variant?.Color?.Name ?? string.Empty,
            variant?.Size?.Name ?? string.Empty,
            variant?.Barcode ?? string.Empty,
            item.Quantity,
            item.UnitPrice,
            item.TotalAmount,
            variant?.Stock ?? 0);
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTime ToUtc(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            return value;
        }

        return DateTime.SpecifyKind(value.Date, DateTimeKind.Utc);
    }

}
