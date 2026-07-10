using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .HasColumnName("id");

        builder.Property(product => product.ProductCode)
            .HasColumnName("product_code")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(product => product.ProductCode)
            .IsUnique();

        builder.Property(product => product.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(product => product.BrandId)
            .HasColumnName("brand_id");

        builder.Property(product => product.CategoryId)
            .HasColumnName("category_id");

        builder.Property(product => product.SeasonId)
            .HasColumnName("season_id");

        builder.Property(product => product.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(product => product.ImageUrl)
            .HasColumnName("image_url")
            .HasMaxLength(1000);

        builder.Property(product => product.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(product => product.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.HasOne(product => product.Brand)
            .WithMany()
            .HasForeignKey(product => product.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(product => product.Category)
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(product => product.Season)
            .WithMany()
            .HasForeignKey(product => product.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(product => product.Variants)
            .WithOne(variant => variant.Product)
            .HasForeignKey(variant => variant.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(product => product.CreatedAt);
        builder.Ignore(product => product.UpdatedAt);
    }
}
