using FiolinOne.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Persistence;

public sealed class DocumentNumberGenerator(ApplicationDbContext dbContext) : IDocumentNumberGenerator
{
    private static readonly IReadOnlyDictionary<string, string> Prefixes = new Dictionary<string, string>
    {
        [DocumentNumberTypes.Product] = "URN",
        [DocumentNumberTypes.Fabric] = "KMS",
        [DocumentNumberTypes.PurchaseOrder] = "SAT",
        [DocumentNumberTypes.GoodsReceipt] = "MK",
        [DocumentNumberTypes.PurchaseInvoice] = "FAT",
        [DocumentNumberTypes.ProductionOrder] = "URT",
        [DocumentNumberTypes.FabricReservation] = "REZ",
        [DocumentNumberTypes.SalesOrder] = "SIP"
    };

    public async Task<string> GenerateAsync(string documentType, CancellationToken cancellationToken)
    {
        if (!Prefixes.TryGetValue(documentType, out var prefix))
        {
            throw new InvalidOperationException("Bilinmeyen belge numarası tipi.");
        }

        var year = DateTime.UtcNow.Year;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var sequence = await dbContext.DocumentSequences
            .FromSqlInterpolated($"""
                SELECT * FROM document_sequences
                WHERE document_type = {documentType} AND year = {year}
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(cancellationToken);

        if (sequence is null)
        {
            sequence = new DocumentSequence(documentType, year);
            dbContext.DocumentSequences.Add(sequence);
        }

        var nextNumber = sequence.Next();
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return $"{prefix}-{year}-{nextNumber:000000}";
    }
}
