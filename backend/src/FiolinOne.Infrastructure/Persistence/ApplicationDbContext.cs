using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.Fabric;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Products;
using FiolinOne.Domain.Production;
using FiolinOne.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Color> Colors => Set<Color>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<FabricType> FabricTypes => Set<FabricType>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<GoodsReceipt> GoodsReceipts => Set<GoodsReceipt>();
    public DbSet<GoodsReceiptItem> GoodsReceiptItems => Set<GoodsReceiptItem>();
    public DbSet<PurchaseInvoice> PurchaseInvoices => Set<PurchaseInvoice>();
    public DbSet<PurchaseInvoiceItem> PurchaseInvoiceItems => Set<PurchaseInvoiceItem>();
    public DbSet<FiolinOne.Domain.Fabric.Fabric> Fabrics => Set<FiolinOne.Domain.Fabric.Fabric>();
    public DbSet<FabricMovement> FabricMovements => Set<FabricMovement>();
    public DbSet<FabricReservation> FabricReservations => Set<FabricReservation>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<ProductionOrderItem> ProductionOrderItems => Set<ProductionOrderItem>();
    public DbSet<CuttingRecord> CuttingRecords => Set<CuttingRecord>();
    public DbSet<WorkshopShipment> WorkshopShipments => Set<WorkshopShipment>();
    public DbSet<WorkshopReturn> WorkshopReturns => Set<WorkshopReturn>();
    public DbSet<WarehouseEntry> WarehouseEntries => Set<WarehouseEntry>();
    public DbSet<ProductionTimelineEntry> ProductionTimelineEntries => Set<ProductionTimelineEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
