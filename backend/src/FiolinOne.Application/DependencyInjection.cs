using FluentValidation;
using FiolinOne.Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace FiolinOne.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
