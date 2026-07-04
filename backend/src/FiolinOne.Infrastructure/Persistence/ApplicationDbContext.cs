using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductColor> ProductColors => Set<ProductColor>();
    public DbSet<ProductSize> ProductSizes => Set<ProductSize>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
