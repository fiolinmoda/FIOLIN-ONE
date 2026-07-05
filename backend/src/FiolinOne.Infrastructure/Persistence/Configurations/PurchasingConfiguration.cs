using FiolinOne.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        ConfigurePurchasingEntity(builder);

        builder.Property(supplier => supplier.SupplierCode).HasColumnName("supplier_code").HasMaxLength(50).IsRequired();
        builder.Property(supplier => supplier.SupplierName).HasColumnName("supplier_name").HasMaxLength(200).IsRequired();
        builder.Property(supplier => supplier.Phone).HasColumnName("phone").HasMaxLength(50);
        builder.Property(supplier => supplier.Email).HasColumnName("email").HasMaxLength(150);
        builder.Property(supplier => supplier.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(supplier => supplier.TaxNumber).HasColumnName("tax_number").HasMaxLength(50);
        builder.Property(supplier => supplier.PaymentTerm).HasColumnName("payment_term").HasMaxLength(100);
        builder.Property(supplier => supplier.Active).HasColumnName("active").IsRequired();

        builder.HasIndex(supplier => supplier.SupplierCode).IsUnique();
        builder.HasIndex(supplier => supplier.SupplierName);
        builder.HasQueryFilter(supplier => !supplier.IsDeleted);
        builder.Ignore(supplier => supplier.CreatedAt);
        builder.Ignore(supplier => supplier.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        ConfigurePurchasingEntity(builder);

        builder.Property(order => order.PurchaseNumber).HasColumnName("purchase_number").HasMaxLength(50).IsRequired();
        builder.Property(order => order.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(order => order.OrderDate).HasColumnName("order_date").IsRequired();
        builder.Property(order => order.ExpectedDate).HasColumnName("expected_date");
        builder.Property(order => order.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(order => order.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasOne(order => order.Supplier)
            .WithMany()
            .HasForeignKey(order => order.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.PurchaseOrder)
            .HasForeignKey(item => item.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(order => order.PurchaseNumber).IsUnique();
        builder.HasIndex(order => order.SupplierId);
        builder.HasIndex(order => order.Status);
        builder.HasQueryFilter(order => !order.IsDeleted);
        builder.Ignore(order => order.CreatedAt);
        builder.Ignore(order => order.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("purchase_order_items");
        ConfigurePurchasingEntity(builder);

        builder.Property(item => item.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(item => item.FabricTypeId).HasColumnName("fabric_type_id");
        builder.Property(item => item.ColorId).HasColumnName("color_id");
        builder.Property(item => item.ItemName).HasColumnName("item_name").HasMaxLength(200).IsRequired();
        builder.Property(item => item.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.Unit).HasColumnName("unit").HasMaxLength(30).IsRequired();
        builder.Property(item => item.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.ReceivedQuantity).HasColumnName("received_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.Status).HasColumnName("status").HasMaxLength(50).IsRequired();

        builder.HasOne(item => item.FabricType)
            .WithMany()
            .HasForeignKey(item => item.FabricTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Color)
            .WithMany()
            .HasForeignKey(item => item.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.PurchaseOrderId);
        builder.HasIndex(item => item.Status);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.RemainingQuantity);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class GoodsReceiptConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("goods_receipts");
        ConfigurePurchasingEntity(builder);

        builder.Property(receipt => receipt.ReceiptNumber).HasColumnName("receipt_number").HasMaxLength(50).IsRequired();
        builder.Property(receipt => receipt.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(receipt => receipt.PurchaseOrderId).HasColumnName("purchase_order_id");
        builder.Property(receipt => receipt.ReceiptDate).HasColumnName("receipt_date").IsRequired();
        builder.Property(receipt => receipt.Warehouse).HasColumnName("warehouse").HasMaxLength(150).IsRequired();
        builder.Property(receipt => receipt.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(receipt => receipt.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasOne(receipt => receipt.Supplier)
            .WithMany()
            .HasForeignKey(receipt => receipt.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(receipt => receipt.PurchaseOrder)
            .WithMany()
            .HasForeignKey(receipt => receipt.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(receipt => receipt.Items)
            .WithOne(item => item.GoodsReceipt)
            .HasForeignKey(item => item.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(receipt => receipt.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(receipt => receipt.ReceiptNumber).IsUnique();
        builder.HasIndex(receipt => receipt.SupplierId);
        builder.HasIndex(receipt => receipt.Status);
        builder.HasQueryFilter(receipt => !receipt.IsDeleted);
        builder.Ignore(receipt => receipt.CreatedAt);
        builder.Ignore(receipt => receipt.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class GoodsReceiptItemConfiguration : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.ToTable("goods_receipt_items");
        ConfigurePurchasingEntity(builder);

        builder.Property(item => item.GoodsReceiptId).HasColumnName("goods_receipt_id").IsRequired();
        builder.Property(item => item.PurchaseOrderItemId).HasColumnName("purchase_order_item_id");
        builder.Property(item => item.ItemName).HasColumnName("item_name").HasMaxLength(200).IsRequired();
        builder.Property(item => item.ReceivedQuantity).HasColumnName("received_quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.Unit).HasColumnName("unit").HasMaxLength(30).IsRequired();
        builder.Property(item => item.Acceptance).HasColumnName("acceptance").HasMaxLength(50).IsRequired();
        builder.Property(item => item.DifferenceQuantity).HasColumnName("difference_quantity").HasPrecision(18, 4).IsRequired();

        builder.HasOne(item => item.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(item => item.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.GoodsReceiptId);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        builder.ToTable("purchase_invoices");
        ConfigurePurchasingEntity(builder);

        builder.Property(invoice => invoice.InvoiceNumber).HasColumnName("invoice_number").HasMaxLength(50).IsRequired();
        builder.Property(invoice => invoice.InvoiceDate).HasColumnName("invoice_date").IsRequired();
        builder.Property(invoice => invoice.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(invoice => invoice.PurchaseOrderId).HasColumnName("purchase_order_id");
        builder.Property(invoice => invoice.InvoiceAmount).HasColumnName("invoice_amount").HasPrecision(18, 4).IsRequired();
        builder.Property(invoice => invoice.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(invoice => invoice.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasOne(invoice => invoice.Supplier)
            .WithMany()
            .HasForeignKey(invoice => invoice.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(invoice => invoice.PurchaseOrder)
            .WithMany()
            .HasForeignKey(invoice => invoice.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(invoice => invoice.Items)
            .WithOne(item => item.PurchaseInvoice)
            .HasForeignKey(item => item.PurchaseInvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(invoice => invoice.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(invoice => invoice.InvoiceNumber).IsUnique();
        builder.HasIndex(invoice => invoice.SupplierId);
        builder.HasIndex(invoice => invoice.Status);
        builder.HasQueryFilter(invoice => !invoice.IsDeleted);
        builder.Ignore(invoice => invoice.CreatedAt);
        builder.Ignore(invoice => invoice.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class PurchaseInvoiceItemConfiguration : IEntityTypeConfiguration<PurchaseInvoiceItem>
{
    public void Configure(EntityTypeBuilder<PurchaseInvoiceItem> builder)
    {
        builder.ToTable("purchase_invoice_items");
        ConfigurePurchasingEntity(builder);

        builder.Property(item => item.PurchaseInvoiceId).HasColumnName("purchase_invoice_id").IsRequired();
        builder.Property(item => item.PurchaseOrderItemId).HasColumnName("purchase_order_item_id");
        builder.Property(item => item.ItemName).HasColumnName("item_name").HasMaxLength(200).IsRequired();
        builder.Property(item => item.Quantity).HasColumnName("quantity").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.Unit).HasColumnName("unit").HasMaxLength(30).IsRequired();
        builder.Property(item => item.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 4).IsRequired();

        builder.HasOne(item => item.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(item => item.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => item.PurchaseInvoiceId);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigurePurchasingEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasKey("Id");
        builder.Property<Guid>("Id").HasColumnName("id");
        builder.Property<DateTime>("CreatedAtUtc").HasColumnName("created_at").IsRequired();
        builder.Property<DateTime?>("UpdatedAtUtc").HasColumnName("updated_at");
        builder.Property<string?>("CreatedBy").HasColumnName("created_by").HasMaxLength(100);
        builder.Property<string?>("UpdatedBy").HasColumnName("updated_by").HasMaxLength(100);
        builder.Property<bool>("IsDeleted").HasColumnName("is_deleted").IsRequired();
        builder.Property<DateTime?>("DeletedAtUtc").HasColumnName("deleted_at");
        builder.Property<string?>("DeletedBy").HasColumnName("deleted_by").HasMaxLength(100);
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}
