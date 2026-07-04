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

        builder.Property(product => product.Brand)
            .HasColumnName("brand")
            .HasMaxLength(100);

        builder.Property(product => product.Category)
            .HasColumnName("category")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(product => product.Season)
            .HasColumnName("season")
            .HasMaxLength(50);

        builder.Property(product => product.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(product => product.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(product => product.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.Ignore(product => product.CreatedAt);
        builder.Ignore(product => product.UpdatedAt);
    }
}
