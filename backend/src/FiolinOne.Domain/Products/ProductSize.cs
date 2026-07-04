using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Products;

public sealed class ProductSize : Entity
{
    private ProductSize()
    {
    }

    public ProductSize(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
}
