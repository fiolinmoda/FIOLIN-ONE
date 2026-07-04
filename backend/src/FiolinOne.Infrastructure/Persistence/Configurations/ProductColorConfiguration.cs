using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        builder.ToTable("product_colors");

        builder.HasKey(color => color.Id);

        builder.Property(color => color.Id)
            .HasColumnName("id");

        builder.Property(color => color.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(color => color.Name)
            .IsUnique();

        builder.Property(color => color.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(color => color.UpdatedAtUtc)
            .HasColumnName("updated_at");
    }
}
