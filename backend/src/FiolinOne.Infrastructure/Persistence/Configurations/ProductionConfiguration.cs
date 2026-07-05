using FiolinOne.Domain.Production;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class ProductionOrderConfiguration : IEntityTypeConfiguration<ProductionOrder>
{
    public void Configure(EntityTypeBuilder<ProductionOrder> builder)
    {
        builder.ToTable("production_orders");
        ConfigureTracked(builder);
        builder.Property(order => order.ProductionNumber).HasColumnName("production_number").HasMaxLength(50).IsRequired();
        builder.Property(order => order.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(order => order.PlannedQuantity).HasColumnName("planned_quantity").IsRequired();
        builder.Property(order => order.ProductionReason).HasColumnName("production_reason").HasMaxLength(50).IsRequired();
        builder.Property(order => order.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(order => order.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.HasOne(order => order.Product).WithMany().HasForeignKey(order => order.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(order => order.Items).WithOne(item => item.ProductionOrder).HasForeignKey(item => item.ProductionOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(order => order.TimelineEntries).WithOne(item => item.ProductionOrder).HasForeignKey(item => item.ProductionOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(order => order.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(order => order.TimelineEntries).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(order => order.ProductionNumber).IsUnique();
        builder.HasIndex(order => order.Status);
        builder.HasQueryFilter(order => !order.IsDeleted);
        builder.Ignore(order => order.CreatedAt);
        builder.Ignore(order => order.UpdatedAt);
    }

    private static void ConfigureTracked<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
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

public sealed class ProductionOrderItemConfiguration : IEntityTypeConfiguration<ProductionOrderItem>
{
    public void Configure(EntityTypeBuilder<ProductionOrderItem> builder)
    {
        builder.ToTable("production_order_items");
        ConfigureTracked(builder);
        builder.Property(item => item.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(item => item.ProductVariantId).HasColumnName("product_variant_id").IsRequired();
        builder.Property(item => item.PlannedQuantity).HasColumnName("planned_quantity").IsRequired();
        builder.Property(item => item.BarcodeGenerated).HasColumnName("barcode_generated").IsRequired();
        builder.Property(item => item.BarcodePrinted).HasColumnName("barcode_printed").IsRequired();
        builder.Property(item => item.BarcodeValue).HasColumnName("barcode_value").HasMaxLength(100);
        builder.HasOne(item => item.ProductVariant).WithMany().HasForeignKey(item => item.ProductVariantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(item => item.ProductionOrderId);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigureTracked<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
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

public sealed class CuttingRecordConfiguration : IEntityTypeConfiguration<CuttingRecord>
{
    public void Configure(EntityTypeBuilder<CuttingRecord> builder)
    {
        builder.ToTable("cutting_records");
        ConfigureTracked(builder);
        builder.Property(item => item.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(item => item.FabricId).HasColumnName("fabric_id").IsRequired();
        builder.Property(item => item.ConsumedWeightKg).HasColumnName("consumed_weight_kg").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.WasteWeightKg).HasColumnName("waste_weight_kg").HasPrecision(18, 4).IsRequired();
        builder.Property(item => item.CuttingDate).HasColumnName("cutting_date").IsRequired();
        builder.Property(item => item.OperatorName).HasColumnName("operator_name").HasMaxLength(100);
        builder.Property(item => item.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.HasOne(item => item.ProductionOrder).WithMany().HasForeignKey(item => item.ProductionOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(item => item.Fabric).WithMany().HasForeignKey(item => item.FabricId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigureTracked<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
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

public sealed class WorkshopShipmentConfiguration : IEntityTypeConfiguration<WorkshopShipment>
{
    public void Configure(EntityTypeBuilder<WorkshopShipment> builder)
    {
        builder.ToTable("workshop_shipments");
        ConfigureTracked(builder);
        builder.Property(item => item.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(item => item.Workshop).HasColumnName("workshop").HasMaxLength(150).IsRequired();
        builder.Property(item => item.ShipmentDate).HasColumnName("shipment_date").IsRequired();
        builder.Property(item => item.ExpectedReturnDate).HasColumnName("expected_return_date");
        builder.Property(item => item.SentQuantity).HasColumnName("sent_quantity").IsRequired();
        builder.Property(item => item.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(item => item.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.HasOne(item => item.ProductionOrder).WithMany().HasForeignKey(item => item.ProductionOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigureTracked<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
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

public sealed class WorkshopReturnConfiguration : IEntityTypeConfiguration<WorkshopReturn>
{
    public void Configure(EntityTypeBuilder<WorkshopReturn> builder)
    {
        builder.ToTable("workshop_returns");
        ConfigureTracked(builder);
        builder.Property(item => item.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(item => item.WorkshopShipmentId).HasColumnName("workshop_shipment_id");
        builder.Property(item => item.ReturnedQuantity).HasColumnName("returned_quantity").IsRequired();
        builder.Property(item => item.ExtraQuantity).HasColumnName("extra_quantity").IsRequired();
        builder.Property(item => item.MissingQuantity).HasColumnName("missing_quantity").IsRequired();
        builder.Property(item => item.ReturnDate).HasColumnName("return_date").IsRequired();
        builder.Property(item => item.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.HasOne(item => item.ProductionOrder).WithMany().HasForeignKey(item => item.ProductionOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(item => item.WorkshopShipment).WithMany().HasForeignKey(item => item.WorkshopShipmentId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigureTracked<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
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

public sealed class WarehouseEntryConfiguration : IEntityTypeConfiguration<WarehouseEntry>
{
    public void Configure(EntityTypeBuilder<WarehouseEntry> builder)
    {
        builder.ToTable("production_warehouse_entries");
        ConfigureTracked(builder);
        builder.Property(item => item.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(item => item.ActualQuantity).HasColumnName("actual_quantity").IsRequired();
        builder.Property(item => item.WarehouseDate).HasColumnName("warehouse_date").IsRequired();
        builder.Property(item => item.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.HasOne(item => item.ProductionOrder).WithMany().HasForeignKey(item => item.ProductionOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(item => !item.IsDeleted);
        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }

    private static void ConfigureTracked<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : class
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

public sealed class ProductionTimelineEntryConfiguration : IEntityTypeConfiguration<ProductionTimelineEntry>
{
    public void Configure(EntityTypeBuilder<ProductionTimelineEntry> builder)
    {
        builder.ToTable("production_timeline_entries");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.ProductionOrderId).HasColumnName("production_order_id").IsRequired();
        builder.Property(item => item.EventType).HasColumnName("event_type").HasMaxLength(100).IsRequired();
        builder.Property(item => item.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
        builder.Property(item => item.EventDate).HasColumnName("event_date").IsRequired();
        builder.Property(item => item.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.HasIndex(item => item.ProductionOrderId);
        builder.Ignore(item => item.CreatedAt);
    }
}
