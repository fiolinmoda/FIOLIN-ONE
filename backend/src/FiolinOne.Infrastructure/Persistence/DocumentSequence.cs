namespace FiolinOne.Infrastructure.Persistence;

public sealed class DocumentSequence
{
    private DocumentSequence()
    {
    }

    public DocumentSequence(string documentType, int year)
    {
        Id = Guid.NewGuid();
        DocumentType = documentType;
        Year = year;
        LastNumber = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public int LastNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public int Next()
    {
        LastNumber++;
        UpdatedAt = DateTime.UtcNow;
        return LastNumber;
    }
}

