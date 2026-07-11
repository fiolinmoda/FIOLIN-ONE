using FiolinOne.Domain.Products;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Purchasing;
using FiolinOne.Domain.Fabric;
using FiolinOne.Domain.Operations;
using FiolinOne.Domain.Production;
using FiolinOne.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FiolinOne.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
    DbSet<ProductImportHistory> ProductImportHistories { get; }
    DbSet<ProductImportProfile> ProductImportProfiles { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Category> Categories { get; }
    DbSet<Season> Seasons { get; }
    DbSet<Color> Colors { get; }
    DbSet<Size> Sizes { get; }
    DbSet<FabricType> FabricTypes { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }
    DbSet<GoodsReceipt> GoodsReceipts { get; }
    DbSet<GoodsReceiptItem> GoodsReceiptItems { get; }
    DbSet<PurchaseInvoice> PurchaseInvoices { get; }
    DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems { get; }
    DbSet<FiolinOne.Domain.Fabric.Fabric> Fabrics { get; }
    DbSet<FabricMovement> FabricMovements { get; }
    DbSet<FabricReservation> FabricReservations { get; }
    DbSet<ProductionOrder> ProductionOrders { get; }
    DbSet<ProductionOrderItem> ProductionOrderItems { get; }
    DbSet<CuttingRecord> CuttingRecords { get; }
    DbSet<WorkshopShipment> WorkshopShipments { get; }
    DbSet<WorkshopReturn> WorkshopReturns { get; }
    DbSet<WarehouseEntry> WarehouseEntries { get; }
    DbSet<ProductionTimelineEntry> ProductionTimelineEntries { get; }
    DbSet<SalesOrder> SalesOrders { get; }
    DbSet<SalesOrderItem> SalesOrderItems { get; }
    DbSet<GoodsReceiptOperation> GoodsReceiptOperations { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
