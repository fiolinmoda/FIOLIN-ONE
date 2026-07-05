using FiolinOne.Domain.Products;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FiolinOne.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }
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

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
