using FiolinOne.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("sales_orders");
        ConfigureTrackedEntity(builder);

        builder.Property(order => order.SalesOrderNumber).HasColumnName("sales_order_number").HasMaxLength(50).IsRequired();
        builder.Property(order => order.CustomerName).HasColumnName("customer_name").HasMaxLength(200).IsRequired();
        builder.Property(order => order.OrderDate).HasColumnName("order_date").IsRequired();
        builder.Property(order => order.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(order => order.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 4).IsRequired();
        builder.Property(order => order.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasMany(order => order.Items).WithOne(item => item.SalesOrder).HasForeignKey(item => item.SalesOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(order => order.SalesOrderNumber).IsUnique();
        builder.HasIndex(order => order.CustomerName);
        builder.HasIndex(order => order.Status);
        builder.HasQueryFilter(order => !order.IsDeleted);
        builder.Ignore(order => order.CreatedAt);
        builder.Ignore(order => order.UpdatedAt);
    }

    private static void ConfigureTrackedEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
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
        builder.Property<uint>("RowVersion").HasColumnName("row_version").HasDefaultValueSql("'0'::xid").IsRowVersion();
    }
}

public sealed class SalesOrderItemConfiguration : IEntityTypeConfiguration<SalesOrderItem>
{
    public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        builder.ToTable("sales_order_items");
        ConfigureTrackedEntity(builder);

        builder.Property(item => item.SalesOrderId).HasColumnName("sales_order_id").IsRequired();
        builder.Property(item => item.ProductVariantId).HasColumnName("product_variant_id").IsRequired();
        builder.Property(item => item.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(item => item.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 4).IsRequired();

        builder.HasOne(item => item.ProductVariant).WithMany().HasForeignKey(item => item.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(item => item.SalesOrderId);
        builder.HasIndex(item => item.ProductVariantId);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigureTrackedEntity<TEntity>(EntityTypeBuilder<TEntity> builder)
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
        builder.Property<uint>("RowVersion").HasColumnName("row_version").HasDefaultValueSql("'0'::xid").IsRowVersion();
    }
}
