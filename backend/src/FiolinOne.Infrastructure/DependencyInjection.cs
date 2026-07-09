using FiolinOne.Application.Fabric;
using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Application.MasterData;
using FiolinOne.Application.Products;
using FiolinOne.Application.Products.Variants;
using FiolinOne.Application.Production;
using FiolinOne.Application.Purchasing;
using FiolinOne.Infrastructure.MasterData;
using FiolinOne.Infrastructure.Persistence;
using FiolinOne.Infrastructure.Products;
using FiolinOne.Infrastructure.Production;
using FiolinOne.Infrastructure.Purchasing;
using FiolinOne.Infrastructure.Sales;
using FiolinOne.Infrastructure.Fabric;
using FiolinOne.Application.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiolinOne.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IDocumentNumberGenerator, DocumentNumberGenerator>();
        services.AddScoped<IMasterDataRepository, MasterDataRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IPurchasingRepository, PurchasingRepository>();
        services.AddScoped<IFabricRepository, FabricRepository>();
        services.AddScoped<IProductionRepository, ProductionRepository>();
        services.AddScoped<ISalesRepository, SalesRepository>();

        return services;
    }
}
