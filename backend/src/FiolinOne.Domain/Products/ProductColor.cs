using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Products;

public sealed class ProductColor : Entity
{
    private ProductColor()
    {
    }

    public ProductColor(string name)
    {
        Name = name;
    }

    public string Name { get; private set; } = string.Empty;
}
