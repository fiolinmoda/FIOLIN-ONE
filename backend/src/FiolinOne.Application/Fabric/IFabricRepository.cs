using FiolinOne.Application.Common.Models;
using FiolinOne.Domain.Fabric;

namespace FiolinOne.Application.Fabric;

public interface IFabricRepository
{
    Task<PagedResult<Domain.Fabric.Fabric>> GetFabricsAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<Domain.Fabric.Fabric?> GetFabricByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> FabricCodeExistsAsync(string fabricCode, Guid? excludedId, CancellationToken cancellationToken);
    Task<decimal> GetReservedQuantityAsync(Guid fabricId, Guid? excludedReservationId, CancellationToken cancellationToken);
    Task AddFabricAsync(Domain.Fabric.Fabric fabric, CancellationToken cancellationToken);

    Task<PagedResult<FabricMovement>> GetMovementsAsync(QueryParameters query, Guid? fabricId, CancellationToken cancellationToken);
    Task AddMovementAsync(FabricMovement movement, CancellationToken cancellationToken);

    Task<PagedResult<FabricReservation>> GetReservationsAsync(QueryParameters query, CancellationToken cancellationToken);
    Task<FabricReservation?> GetReservationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ReservationNumberExistsAsync(string reservationNumber, Guid? excludedId, CancellationToken cancellationToken);
    Task AddReservationAsync(FabricReservation reservation, CancellationToken cancellationToken);

    Task<bool> SupplierExistsAsync(Guid supplierId, CancellationToken cancellationToken);
    Task<bool> PurchaseOrderExistsAsync(Guid purchaseOrderId, CancellationToken cancellationToken);
    Task<bool> ColorExistsAsync(Guid colorId, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
