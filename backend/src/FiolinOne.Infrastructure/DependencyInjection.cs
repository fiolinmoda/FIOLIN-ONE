using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Application.MasterData;
using FiolinOne.Application.Products;
using FiolinOne.Application.Products.Variants;
using FiolinOne.Application.Purchasing;
using FiolinOne.Infrastructure.MasterData;
using FiolinOne.Infrastructure.Persistence;
using FiolinOne.Infrastructure.Products;
using FiolinOne.Infrastructure.Purchasing;
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
        services.AddScoped<IMasterDataRepository, MasterDataRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
        services.AddScoped<IPurchasingRepository, PurchasingRepository>();

        return services;
    }
}
