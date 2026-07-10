using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class ProductImportHistoryConfiguration : IEntityTypeConfiguration<ProductImportHistory>
{
    public void Configure(EntityTypeBuilder<ProductImportHistory> builder)
    {
        builder.ToTable("product_import_histories");
        builder.HasKey(history => history.Id);
        builder.Property(history => history.Id).HasColumnName("id");
        builder.Property(history => history.UserName).HasColumnName("user_name").HasMaxLength(150).IsRequired();
        builder.Property(history => history.FileName).HasColumnName("file_name").HasMaxLength(260).IsRequired();
        builder.Property(history => history.TotalRecords).HasColumnName("total_records").IsRequired();
        builder.Property(history => history.InsertedRecords).HasColumnName("inserted_records").IsRequired();
        builder.Property(history => history.ExistingRecords).HasColumnName("existing_records").IsRequired();
        builder.Property(history => history.SkippedRecords).HasColumnName("skipped_records").IsRequired();
        builder.Property(history => history.ErrorRecords).HasColumnName("error_records").IsRequired();
        builder.Property(history => history.DurationMilliseconds).HasColumnName("duration_milliseconds").IsRequired();
        builder.Property(history => history.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(history => history.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(history => history.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(history => history.UpdatedAtUtc).HasColumnName("updated_at");
        builder.Ignore(history => history.ImportedAt);
    }
}

public sealed class ProductImportProfileConfiguration : IEntityTypeConfiguration<ProductImportProfile>
{
    public void Configure(EntityTypeBuilder<ProductImportProfile> builder)
    {
        builder.ToTable("product_import_profiles");
        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.Id).HasColumnName("id");
        builder.Property(profile => profile.ProfileName).HasColumnName("profile_name").HasMaxLength(150).IsRequired();
        builder.Property(profile => profile.FileSignature).HasColumnName("file_signature").HasMaxLength(128).IsRequired();
        builder.Property(profile => profile.MappingJson).HasColumnName("mapping_json").HasColumnType("jsonb").IsRequired();
        builder.Property(profile => profile.CreatedBy).HasColumnName("created_by").HasMaxLength(150).IsRequired();
        builder.Property(profile => profile.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(profile => profile.UpdatedAtUtc).HasColumnName("updated_at");
        builder.HasIndex(profile => profile.FileSignature).IsUnique();
    }
}
