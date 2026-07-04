using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class ProductSizeConfiguration : IEntityTypeConfiguration<ProductSize>
{
    public void Configure(EntityTypeBuilder<ProductSize> builder)
    {
        builder.ToTable("product_sizes");

        builder.HasKey(size => size.Id);

        builder.Property(size => size.Id)
            .HasColumnName("id");

        builder.Property(size => size.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(size => size.Name)
            .IsUnique();

        builder.Property(size => size.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(size => size.UpdatedAtUtc)
            .HasColumnName("updated_at");
    }
}
