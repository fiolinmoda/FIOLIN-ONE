using FiolinOne.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
