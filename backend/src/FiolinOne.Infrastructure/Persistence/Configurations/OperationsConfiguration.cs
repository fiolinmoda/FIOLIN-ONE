using FiolinOne.Domain.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class GoodsReceiptOperationConfiguration : IEntityTypeConfiguration<GoodsReceiptOperation>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptOperation> builder)
    {
        builder.ToTable("operation_goods_receipts");

        builder.HasKey(receipt => receipt.Id);

        builder.Property(receipt => receipt.Id)
            .HasColumnName("id");

        builder.Property(receipt => receipt.SupplierId)
            .HasColumnName("supplier_id")
            .IsRequired();

        builder.Property(receipt => receipt.ProductVariantId)
            .HasColumnName("product_variant_id")
            .IsRequired();

        builder.Property(receipt => receipt.TransactionDate)
            .HasColumnName("transaction_date")
            .IsRequired();

        builder.Property(receipt => receipt.MovementType)
            .HasColumnName("movement_type")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(receipt => receipt.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(receipt => receipt.PurchasePrice)
            .HasColumnName("purchase_price")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(receipt => receipt.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(receipt => receipt.Shelf)
            .HasColumnName("shelf")
            .HasMaxLength(80);

        builder.Property(receipt => receipt.Box)
            .HasColumnName("box")
            .HasMaxLength(80);

        builder.Property(receipt => receipt.StockBefore)
            .HasColumnName("stock_before")
            .IsRequired();

        builder.Property(receipt => receipt.StockAfter)
            .HasColumnName("stock_after")
            .IsRequired();

        builder.Property(receipt => receipt.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(receipt => receipt.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.HasOne(receipt => receipt.Supplier)
            .WithMany()
            .HasForeignKey(receipt => receipt.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(receipt => receipt.ProductVariant)
            .WithMany()
            .HasForeignKey(receipt => receipt.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(receipt => receipt.TransactionDate);
        builder.HasIndex(receipt => receipt.MovementType);
        builder.HasIndex(receipt => receipt.SupplierId);
        builder.HasIndex(receipt => receipt.ProductVariantId);
    }
}
