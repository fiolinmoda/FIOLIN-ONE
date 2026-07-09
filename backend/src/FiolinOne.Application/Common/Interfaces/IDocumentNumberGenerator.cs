namespace FiolinOne.Application.Common.Interfaces;

public interface IDocumentNumberGenerator
{
    Task<string> GenerateAsync(string documentType, CancellationToken cancellationToken);
}

public static class DocumentNumberTypes
{
    public const string Product = "Product";
    public const string Fabric = "Fabric";
    public const string PurchaseOrder = "PurchaseOrder";
    public const string GoodsReceipt = "GoodsReceipt";
    public const string PurchaseInvoice = "PurchaseInvoice";
    public const string ProductionOrder = "ProductionOrder";
    public const string FabricReservation = "FabricReservation";
}

