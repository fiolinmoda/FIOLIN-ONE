using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");

        builder.HasKey(variant => variant.Id);

        builder.Property(variant => variant.Id)
            .HasColumnName("id");

        builder.Property(variant => variant.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(variant => variant.ColorId)
            .HasColumnName("color_id")
            .IsRequired();

        builder.Property(variant => variant.SizeId)
            .HasColumnName("size_id")
            .IsRequired();

        builder.Property(variant => variant.Barcode)
            .HasColumnName("barcode")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(variant => variant.TrendyolSku)
            .HasColumnName("trendyol_sku")
            .HasMaxLength(100);

        builder.Property(variant => variant.Stock)
            .HasColumnName("stock")
            .IsRequired();

        builder.Property(variant => variant.PurchasePrice)
            .HasColumnName("purchase_price")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(variant => variant.SalesPrice)
            .HasColumnName("sales_price")
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(variant => variant.LastSupplierId)
            .HasColumnName("last_supplier_id");

        builder.Property(variant => variant.Shelf)
            .HasColumnName("shelf")
            .HasMaxLength(80);

        builder.Property(variant => variant.Box)
            .HasColumnName("box")
            .HasMaxLength(80);

        builder.Property(variant => variant.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(variant => variant.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(variant => variant.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.HasOne(variant => variant.Product)
            .WithMany(product => product.Variants)
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(variant => variant.Color)
            .WithMany()
            .HasForeignKey(variant => variant.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(variant => variant.Size)
            .WithMany()
            .HasForeignKey(variant => variant.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(variant => variant.LastSupplier)
            .WithMany()
            .HasForeignKey(variant => variant.LastSupplierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(variant => new { variant.ProductId, variant.ColorId, variant.SizeId })
            .IsUnique();

        builder.HasIndex(variant => variant.Barcode)
            .IsUnique();

        builder.HasIndex(variant => variant.TrendyolSku)
            .IsUnique();

        builder.HasIndex(variant => variant.LastSupplierId);

        builder.Ignore(variant => variant.CreatedAt);
        builder.Ignore(variant => variant.UpdatedAt);
    }
}
