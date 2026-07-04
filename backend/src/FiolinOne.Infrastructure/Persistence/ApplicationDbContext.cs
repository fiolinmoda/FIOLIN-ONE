using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.MasterData;
using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Color> Colors => Set<Color>();
    public DbSet<Size> Sizes => Set<Size>();
    public DbSet<FabricType> FabricTypes => Set<FabricType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
