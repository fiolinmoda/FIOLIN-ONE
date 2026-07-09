using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiolinOne.Infrastructure.Persistence.Configurations;

public sealed class DocumentSequenceConfiguration : IEntityTypeConfiguration<DocumentSequence>
{
    public void Configure(EntityTypeBuilder<DocumentSequence> builder)
    {
        builder.ToTable("document_sequences");

        builder.HasKey(sequence => sequence.Id);

        builder.Property(sequence => sequence.Id).HasColumnName("id");
        builder.Property(sequence => sequence.DocumentType).HasColumnName("document_type").HasMaxLength(80).IsRequired();
        builder.Property(sequence => sequence.Year).HasColumnName("year").IsRequired();
        builder.Property(sequence => sequence.LastNumber).HasColumnName("last_number").IsRequired();
        builder.Property(sequence => sequence.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(sequence => sequence.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(sequence => new { sequence.DocumentType, sequence.Year }).IsUnique();
    }
}

