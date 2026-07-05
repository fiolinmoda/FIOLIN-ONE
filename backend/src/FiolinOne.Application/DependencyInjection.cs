using FluentValidation;
using FiolinOne.Application.Fabric;
using FiolinOne.Application.MasterData;
using FiolinOne.Application.Products;
using FiolinOne.Application.Products.Variants;
using FiolinOne.Application.Production;
using FiolinOne.Application.Purchasing;
using Microsoft.Extensions.DependencyInjection;

namespace FiolinOne.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IMasterDataService, MasterDataService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductVariantService, ProductVariantService>();
        services.AddScoped<IPurchasingService, PurchasingService>();
        services.AddScoped<IFabricService, FabricService>();
        services.AddScoped<IProductionService, ProductionService>();

        return services;
    }
}
