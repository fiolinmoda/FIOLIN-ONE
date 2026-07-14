using FiolinOne.Domain.Commerce;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Products;

namespace FiolinOne.Application.Commerce;

public interface ICommerceRepository
{
    IQueryable<Product> Products { get; }
    IQueryable<Category> Categories { get; }
    IQueryable<CmsMenu> Menus { get; }
    IQueryable<CmsBanner> Banners { get; }
    IQueryable<CmsSlider> Sliders { get; }
    IQueryable<CmsSetting> Settings { get; }
    IQueryable<CmsSeoPage> SeoPages { get; }
}
