namespace FiolinOne.Application.Purchasing;

public sealed record CreateSupplierRequest(
    string SupplierCode,
    string SupplierName,
    string? Phone,
    string? Email,
    string? Address,
    string? TaxNumber,
    string? PaymentTerm,
    bool Active);

public sealed record UpdateSupplierRequest(
    string SupplierCode,
    string SupplierName,
    string? Phone,
    string? Email,
    string? Address,
    string? TaxNumber,
    string? PaymentTerm,
    bool Active);

public sealed record CreatePurchaseOrderRequest(
    string PurchaseNumber,
    Guid SupplierId,
    DateTime OrderDate,
    DateTime? ExpectedDate,
    string Status,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemRequest> Items);

public sealed record UpdatePurchaseOrderRequest(
    string PurchaseNumber,
    Guid SupplierId,
    DateTime OrderDate,
    DateTime? ExpectedDate,
    string Status,
    string? Notes,
    IReadOnlyList<PurchaseOrderItemRequest> Items);

public sealed record PurchaseOrderItemRequest(
    Guid? Id,
    Guid? FabricTypeId,
    Guid? ColorId,
    string ItemName,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal ReceivedQuantity,
    string Status);

public sealed record CreateGoodsReceiptRequest(
    string ReceiptNumber,
    Guid SupplierId,
    Guid? PurchaseOrderId,
    DateTime ReceiptDate,
    string Warehouse,
    string Status,
    string? Notes,
    IReadOnlyList<GoodsReceiptItemRequest> Items);

public sealed record UpdateGoodsReceiptRequest(
    string ReceiptNumber,
    Guid SupplierId,
    Guid? PurchaseOrderId,
    DateTime ReceiptDate,
    string Warehouse,
    string Status,
    string? Notes,
    IReadOnlyList<GoodsReceiptItemRequest> Items);

public sealed record GoodsReceiptItemRequest(
    Guid? Id,
    Guid? PurchaseOrderItemId,
    string ItemName,
    decimal ReceivedQuantity,
    string Unit,
    string Acceptance,
    decimal DifferenceQuantity);

public sealed record CreatePurchaseInvoiceRequest(
    string InvoiceNumber,
    DateTime InvoiceDate,
    Guid SupplierId,
    Guid? PurchaseOrderId,
    decimal InvoiceAmount,
    string Status,
    string? Notes,
    IReadOnlyList<PurchaseInvoiceItemRequest> Items);

public sealed record UpdatePurchaseInvoiceRequest(
    string InvoiceNumber,
    DateTime InvoiceDate,
    Guid SupplierId,
    Guid? PurchaseOrderId,
    decimal InvoiceAmount,
    string Status,
    string? Notes,
    IReadOnlyList<PurchaseInvoiceItemRequest> Items);

public sealed record PurchaseInvoiceItemRequest(
    Guid? Id,
    Guid? PurchaseOrderItemId,
    string ItemName,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    decimal TotalAmount);
