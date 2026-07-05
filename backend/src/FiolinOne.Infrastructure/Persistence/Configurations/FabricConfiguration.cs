using FiolinOne.Domain.Fabric;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class FabricConfiguration : IEntityTypeConfiguration<Domain.Fabric.Fabric>
{
    public void Configure(EntityTypeBuilder<Domain.Fabric.Fabric> builder)
    {
        builder.ToTable("fabrics");
        ConfigureTrackedEntity(builder);

        builder.Property(fabric => fabric.FabricCode).HasColumnName("fabric_code").HasMaxLength(50).IsRequired();
        builder.Property(fabric => fabric.FabricName).HasColumnName("fabric_name").HasMaxLength(200).IsRequired();
        builder.Property(fabric => fabric.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(fabric => fabric.ColorId).HasColumnName("color_id").IsRequired();
        builder.Property(fabric => fabric.Composition).HasColumnName("composition").HasMaxLength(200);
        builder.Property(fabric => fabric.Width).HasColumnName("width").HasPrecision(18, 4).IsRequired();
        builder.Property(fabric => fabric.WeightGsm).HasColumnName("weight_gsm").HasPrecision(18, 4).IsRequired();
        builder.Property(fabric => fabric.Unit).HasColumnName("unit").HasMaxLength(30).IsRequired();
        builder.Property(fabric => fabric.PurchasePrice).HasColumnName("purchase_price").HasPrecision(18, 4).IsRequired();
        builder.Property(fabric => fabric.CurrentStockKg).HasColumnName("current_stock_kg").HasPrecision(18, 4).IsRequired();
        builder.Property(fabric => fabric.MinimumStock).HasColumnName("minimum_stock").HasPrecision(18, 4).IsRequired();
        builder.Property(fabric => fabric.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(fabric => fabric.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasOne(fabric => fabric.Supplier).WithMany().HasForeignKey(fabric => fabric.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(fabric => fabric.Color).WithMany().HasForeignKey(fabric => fabric.ColorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(fabric => fabric.FabricCode).IsUnique();
        builder.HasIndex(fabric => fabric.SupplierId);
        builder.HasIndex(fabric => fabric.ColorId);
        builder.HasIndex(fabric => fabric.Status);
        builder.HasQueryFilter(fabric => !fabric.IsDeleted);
        builder.Ignore(fabric => fabric.CreatedAt);
        builder.Ignore(fabric => fabric.UpdatedAt);
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
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class FabricMovementConfiguration : IEntityTypeConfiguration<FabricMovement>
{
    public void Configure(EntityTypeBuilder<FabricMovement> builder)
    {
        builder.ToTable("fabric_movements");
        ConfigureTrackedEntity(builder);

        builder.Property(movement => movement.FabricId).HasColumnName("fabric_id").IsRequired();
        builder.Property(movement => movement.MovementType).HasColumnName("movement_type").HasMaxLength(50).IsRequired();
        builder.Property(movement => movement.QuantityKg).HasColumnName("quantity_kg").HasPrecision(18, 4).IsRequired();
        builder.Property(movement => movement.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 4).IsRequired();
        builder.Property(movement => movement.SupplierId).HasColumnName("supplier_id");
        builder.Property(movement => movement.PurchaseOrderId).HasColumnName("purchase_order_id");
        builder.Property(movement => movement.BatchLot).HasColumnName("batch_lot").HasMaxLength(100);
        builder.Property(movement => movement.Warehouse).HasColumnName("warehouse").HasMaxLength(150).IsRequired();
        builder.Property(movement => movement.MovementDate).HasColumnName("movement_date").IsRequired();
        builder.Property(movement => movement.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasOne(movement => movement.Fabric).WithMany().HasForeignKey(movement => movement.FabricId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(movement => movement.Supplier).WithMany().HasForeignKey(movement => movement.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(movement => movement.PurchaseOrder).WithMany().HasForeignKey(movement => movement.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(movement => movement.FabricId);
        builder.HasIndex(movement => movement.MovementType);
        builder.HasIndex(movement => movement.MovementDate);
        builder.HasQueryFilter(movement => !movement.IsDeleted);
        builder.Ignore(movement => movement.CreatedAt);
        builder.Ignore(movement => movement.UpdatedAt);
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
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}

public sealed class FabricReservationConfiguration : IEntityTypeConfiguration<FabricReservation>
{
    public void Configure(EntityTypeBuilder<FabricReservation> builder)
    {
        builder.ToTable("fabric_reservations");
        ConfigureTrackedEntity(builder);

        builder.Property(reservation => reservation.FabricId).HasColumnName("fabric_id").IsRequired();
        builder.Property(reservation => reservation.ReservationNumber).HasColumnName("reservation_number").HasMaxLength(50).IsRequired();
        builder.Property(reservation => reservation.ProductionReference).HasColumnName("production_reference").HasMaxLength(100).IsRequired();
        builder.Property(reservation => reservation.ReservedQuantityKg).HasColumnName("reserved_quantity_kg").HasPrecision(18, 4).IsRequired();
        builder.Property(reservation => reservation.ReservationDate).HasColumnName("reservation_date").IsRequired();
        builder.Property(reservation => reservation.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(reservation => reservation.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasOne(reservation => reservation.Fabric).WithMany().HasForeignKey(reservation => reservation.FabricId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(reservation => reservation.ReservationNumber).IsUnique();
        builder.HasIndex(reservation => reservation.FabricId);
        builder.HasIndex(reservation => reservation.Status);
        builder.HasQueryFilter(reservation => !reservation.IsDeleted);
        builder.Ignore(reservation => reservation.CreatedAt);
        builder.Ignore(reservation => reservation.UpdatedAt);
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
        builder.Property<uint>("RowVersion").HasColumnName("row_version").IsRowVersion();
    }
}
