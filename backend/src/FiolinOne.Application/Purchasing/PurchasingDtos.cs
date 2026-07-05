using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Purchasing;

public sealed record SupplierDto(
    Guid Id,
    string SupplierCode,
    string SupplierName,
    string? Phone,
    string? Email,
    string? Address,
    string? TaxNumber,
    string? PaymentTerm,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PurchaseOrderDto(
    Guid Id,
    string PurchaseNumber,
    Guid SupplierId,
    string SupplierName,
    DateTime OrderDate,
    DateTime? ExpectedDate,
    string Status,
    string? Notes,
    decimal TotalAmount,
    IReadOnlyList<PurchaseOrderItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PurchaseOrderItemDto(
    Guid Id,
    Guid PurchaseOrderId,
    Guid? FabricTypeId,
    string? FabricType,
    Guid? ColorId,
    string? Color,
    string ItemName,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal ReceivedQuantity,
    decimal RemainingQuantity,
    string Status);

public sealed record GoodsReceiptDto(
    Guid Id,
    string ReceiptNumber,
    Guid SupplierId,
    string SupplierName,
    Guid? PurchaseOrderId,
    string? PurchaseNumber,
    DateTime ReceiptDate,
    string Warehouse,
    string Status,
    string? Notes,
    IReadOnlyList<GoodsReceiptItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record GoodsReceiptItemDto(
    Guid Id,
    Guid GoodsReceiptId,
    Guid? PurchaseOrderItemId,
    string ItemName,
    decimal ReceivedQuantity,
    string Unit,
    string Acceptance,
    decimal DifferenceQuantity);

public sealed record PurchaseInvoiceDto(
    Guid Id,
    string InvoiceNumber,
    DateTime InvoiceDate,
    Guid SupplierId,
    string SupplierName,
    Guid? PurchaseOrderId,
    string? PurchaseNumber,
    decimal InvoiceAmount,
    string Status,
    string? Notes,
    IReadOnlyList<PurchaseInvoiceItemDto> Items,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PurchaseInvoiceItemDto(
    Guid Id,
    Guid PurchaseInvoiceId,
    Guid? PurchaseOrderItemId,
    string ItemName,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalAmount);

public sealed record PurchasingQuery(
    string? Search,
    string? Status,
    string? SortBy,
    string? SortDirection,
    int Page = 1,
    int PageSize = 25)
{
    public QueryParameters ToParameters() => new(Search, Status, SortBy, SortDirection, Page, PageSize);
}
