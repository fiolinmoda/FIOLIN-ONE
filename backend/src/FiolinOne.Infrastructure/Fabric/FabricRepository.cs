using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Fabric;
using FiolinOne.Domain.Fabric;
using FiolinOne.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FiolinOne.Infrastructure.Fabric;

public sealed class FabricRepository(ApplicationDbContext dbContext) : IFabricRepository
{
    public async Task<PagedResult<Domain.Fabric.Fabric>> GetFabricsAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = dbContext.Fabrics
            .AsNoTracking()
            .Include(fabric => fabric.Supplier)
            .Include(fabric => fabric.Color)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(fabric =>
                fabric.FabricCode.ToLower().Contains(term) ||
                fabric.FabricName.ToLower().Contains(term) ||
                fabric.Supplier!.SupplierName.ToLower().Contains(term) ||
                fabric.Color!.Name.ToLower().Contains(term) ||
                (fabric.Composition != null && fabric.Composition.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            source = source.Where(fabric => fabric.Status == query.Status.Trim());
        }

        source = ApplyFabricSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<Domain.Fabric.Fabric?> GetFabricByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Fabrics
            .Include(fabric => fabric.Supplier)
            .Include(fabric => fabric.Color)
            .FirstOrDefaultAsync(fabric => fabric.Id == id, cancellationToken);
    }

    public Task<bool> FabricCodeExistsAsync(string fabricCode, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.Fabrics.AnyAsync(
            fabric => fabric.FabricCode == fabricCode && (!excludedId.HasValue || fabric.Id != excludedId.Value),
            cancellationToken);
    }

    public Task<decimal> GetReservedQuantityAsync(Guid fabricId, Guid? excludedReservationId, CancellationToken cancellationToken)
    {
        return dbContext.FabricReservations
            .Where(reservation =>
                reservation.FabricId == fabricId &&
                reservation.Status == "Active" &&
                (!excludedReservationId.HasValue || reservation.Id != excludedReservationId.Value))
            .SumAsync(reservation => reservation.ReservedQuantityKg, cancellationToken);
    }

    public async Task AddFabricAsync(Domain.Fabric.Fabric fabric, CancellationToken cancellationToken)
    {
        await dbContext.Fabrics.AddAsync(fabric, cancellationToken);
    }

    public async Task<PagedResult<FabricMovement>> GetMovementsAsync(
        QueryParameters query,
        Guid? fabricId,
        CancellationToken cancellationToken)
    {
        var source = dbContext.FabricMovements
            .AsNoTracking()
            .Include(movement => movement.Fabric)
            .Include(movement => movement.Supplier)
            .Include(movement => movement.PurchaseOrder)
            .AsQueryable();

        if (fabricId.HasValue)
        {
            source = source.Where(movement => movement.FabricId == fabricId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(movement =>
                movement.Fabric!.FabricCode.ToLower().Contains(term) ||
                movement.Fabric.FabricName.ToLower().Contains(term) ||
                movement.MovementType.ToLower().Contains(term) ||
                movement.Warehouse.ToLower().Contains(term) ||
                (movement.BatchLot != null && movement.BatchLot.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            source = source.Where(movement => movement.MovementType == query.Status.Trim());
        }

        source = ApplyMovementSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public async Task AddMovementAsync(FabricMovement movement, CancellationToken cancellationToken)
    {
        await dbContext.FabricMovements.AddAsync(movement, cancellationToken);
    }

    public Task<bool> MovementExistsAsync(
        Guid fabricId,
        string movementType,
        decimal quantityKg,
        Guid? supplierId,
        Guid? purchaseOrderId,
        string? batchLot,
        string warehouse,
        DateTime movementDate,
        string? notes,
        CancellationToken cancellationToken)
    {
        return dbContext.FabricMovements.AnyAsync(
            movement =>
                movement.FabricId == fabricId &&
                movement.MovementType == movementType &&
                movement.QuantityKg == quantityKg &&
                movement.SupplierId == supplierId &&
                movement.PurchaseOrderId == purchaseOrderId &&
                movement.BatchLot == batchLot &&
                movement.Warehouse == warehouse &&
                movement.MovementDate == movementDate &&
                movement.Notes == notes,
            cancellationToken);
    }

    public async Task<PagedResult<FabricReservation>> GetReservationsAsync(QueryParameters query, CancellationToken cancellationToken)
    {
        var source = dbContext.FabricReservations
            .AsNoTracking()
            .Include(reservation => reservation.Fabric)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            source = source.Where(reservation =>
                reservation.ReservationNumber.ToLower().Contains(term) ||
                reservation.ProductionReference.ToLower().Contains(term) ||
                reservation.Fabric!.FabricCode.ToLower().Contains(term) ||
                reservation.Fabric.FabricName.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            source = source.Where(reservation => reservation.Status == query.Status.Trim());
        }

        source = ApplyReservationSorting(source, query.SortBy, query.SortDirection);

        return await ToPagedResultAsync(source, query, cancellationToken);
    }

    public Task<FabricReservation?> GetReservationByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.FabricReservations
            .Include(reservation => reservation.Fabric)
            .FirstOrDefaultAsync(reservation => reservation.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<FabricReservation>> GetActiveReservationsByReferenceAsync(
        Guid fabricId,
        string productionReference,
        CancellationToken cancellationToken)
    {
        return await dbContext.FabricReservations
            .Where(reservation =>
                reservation.FabricId == fabricId &&
                reservation.ProductionReference == productionReference &&
                reservation.Status == "Active")
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ReservationNumberExistsAsync(string reservationNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        return dbContext.FabricReservations.AnyAsync(
            reservation => reservation.ReservationNumber == reservationNumber && (!excludedId.HasValue || reservation.Id != excludedId.Value),
            cancellationToken);
    }

    public async Task AddReservationAsync(FabricReservation reservation, CancellationToken cancellationToken)
    {
        await dbContext.FabricReservations.AddAsync(reservation, cancellationToken);
    }

    public Task<bool> SupplierExistsAsync(Guid supplierId, CancellationToken cancellationToken)
    {
        return dbContext.Suppliers.AnyAsync(supplier => supplier.Id == supplierId, cancellationToken);
    }

    public Task<bool> PurchaseOrderExistsAsync(Guid purchaseOrderId, CancellationToken cancellationToken)
    {
        return dbContext.PurchaseOrders.AnyAsync(order => order.Id == purchaseOrderId, cancellationToken);
    }

    public Task<bool> ColorExistsAsync(Guid colorId, CancellationToken cancellationToken)
    {
        return dbContext.Colors.AnyAsync(color => color.Id == colorId, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Domain.Fabric.Fabric> ApplyFabricSorting(
        IQueryable<Domain.Fabric.Fabric> source,
        string? sortBy,
        string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "fabricname" => descending ? source.OrderByDescending(fabric => fabric.FabricName) : source.OrderBy(fabric => fabric.FabricName),
            "supplier" => descending ? source.OrderByDescending(fabric => fabric.Supplier!.SupplierName) : source.OrderBy(fabric => fabric.Supplier!.SupplierName),
            "stock" => descending ? source.OrderByDescending(fabric => fabric.CurrentStockKg) : source.OrderBy(fabric => fabric.CurrentStockKg),
            "status" => descending ? source.OrderByDescending(fabric => fabric.Status) : source.OrderBy(fabric => fabric.Status),
            _ => descending ? source.OrderByDescending(fabric => fabric.FabricCode) : source.OrderBy(fabric => fabric.FabricCode)
        };
    }

    private static IQueryable<FabricMovement> ApplyMovementSorting(
        IQueryable<FabricMovement> source,
        string? sortBy,
        string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "fabric" => descending ? source.OrderByDescending(movement => movement.Fabric!.FabricCode) : source.OrderBy(movement => movement.Fabric!.FabricCode),
            "movementtype" => descending ? source.OrderByDescending(movement => movement.MovementType) : source.OrderBy(movement => movement.MovementType),
            "quantity" => descending ? source.OrderByDescending(movement => movement.QuantityKg) : source.OrderBy(movement => movement.QuantityKg),
            _ => descending ? source.OrderByDescending(movement => movement.MovementDate) : source.OrderBy(movement => movement.MovementDate)
        };
    }

    private static IQueryable<FabricReservation> ApplyReservationSorting(
        IQueryable<FabricReservation> source,
        string? sortBy,
        string? sortDirection)
    {
        var descending = IsDescending(sortDirection);

        return NormalizeSort(sortBy) switch
        {
            "fabric" => descending ? source.OrderByDescending(reservation => reservation.Fabric!.FabricCode) : source.OrderBy(reservation => reservation.Fabric!.FabricCode),
            "quantity" => descending ? source.OrderByDescending(reservation => reservation.ReservedQuantityKg) : source.OrderBy(reservation => reservation.ReservedQuantityKg),
            "status" => descending ? source.OrderByDescending(reservation => reservation.Status) : source.OrderBy(reservation => reservation.Status),
            _ => descending ? source.OrderByDescending(reservation => reservation.ReservationDate) : source.OrderBy(reservation => reservation.ReservationDate)
        };
    }

    private static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        IQueryable<T> source,
        QueryParameters query,
        CancellationToken cancellationToken)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var totalItems = await source.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await source.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<T>(items, page, pageSize, totalItems, totalPages);
    }

    private static bool IsDescending(string? sortDirection)
    {
        return string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSort(string? sortBy)
    {
        return sortBy?.Trim().Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant() ?? string.Empty;
    }
}
