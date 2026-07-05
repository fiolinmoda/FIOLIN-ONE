using FiolinOne.Domain.MasterData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class BrandConfiguration : MasterDataConfiguration<Brand>
{
    protected override string TableName => "brands";
}

public sealed class CategoryConfiguration : MasterDataConfiguration<Category>
{
    protected override string TableName => "categories";
}

public sealed class SeasonConfiguration : MasterDataConfiguration<Season>
{
    protected override string TableName => "seasons";
}

public sealed class ColorConfiguration : MasterDataConfiguration<Color>
{
    protected override string TableName => "colors";
}

public sealed class SizeConfiguration : MasterDataConfiguration<Size>
{
    protected override string TableName => "sizes";
}

public sealed class FabricTypeConfiguration : MasterDataConfiguration<FabricType>
{
    protected override string TableName => "fabric_types";
}

public abstract class MasterDataConfiguration<T> : IEntityTypeConfiguration<T>
    where T : MasterDataEntity
{
    protected abstract string TableName { get; }

    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id)
            .HasColumnName("id");

        builder.Property(item => item.Name)
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(item => item.Code)
            .HasColumnName("code")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(item => item.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        builder.Property(item => item.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();

        builder.Property(item => item.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(item => item.UpdatedAtUtc)
            .HasColumnName("updated_at");

        builder.HasIndex(item => item.Code)
            .IsUnique();

        builder.Ignore(item => item.CreatedAt);
        builder.Ignore(item => item.UpdatedAt);
    }
}
