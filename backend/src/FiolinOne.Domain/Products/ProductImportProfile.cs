using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Products;

public sealed class ProductImportProfile : Entity
{
    private ProductImportProfile()
    {
    }

    public ProductImportProfile(string profileName, string fileSignature, string mappingJson, string createdBy)
    {
        ProfileName = profileName;
        FileSignature = fileSignature;
        MappingJson = mappingJson;
        CreatedBy = createdBy;
    }

    public string ProfileName { get; private set; } = string.Empty;
    public string FileSignature { get; private set; } = string.Empty;
    public string MappingJson { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;

    public void Update(string profileName, string mappingJson, string updatedBy)
    {
        ProfileName = profileName;
        MappingJson = mappingJson;
        CreatedBy = updatedBy;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
