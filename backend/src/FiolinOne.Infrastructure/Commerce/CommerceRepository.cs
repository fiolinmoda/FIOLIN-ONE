using FiolinOne.Application.Commerce;
using FiolinOne.Domain.Commerce;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Products;
using FiolinOne.Infrastructure.Persistence;

namespace FiolinOne.Infrastructure.Commerce;

public sealed class CommerceRepository(ApplicationDbContext dbContext) : ICommerceRepository
{
    public IQueryable<Product> Products => dbContext.Products;
    public IQueryable<Category> Categories => dbContext.Categories;
    public IQueryable<CmsMenu> Menus => dbContext.CmsMenus;
    public IQueryable<CmsBanner> Banners => dbContext.CmsBanners;
    public IQueryable<CmsSlider> Sliders => dbContext.CmsSliders;
    public IQueryable<CmsSetting> Settings => dbContext.CmsSettings;
    public IQueryable<CmsSeoPage> SeoPages => dbContext.CmsSeoPages;
}
