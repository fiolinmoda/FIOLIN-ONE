using FiolinOne.Application.Common.Models;

namespace FiolinOne.Application.Fabric;

public interface IFabricService
{
    Task<PagedResult<FabricDto>> GetFabricsAsync(FabricQuery query, CancellationToken cancellationToken);
    Task<FabricDto?> GetFabricAsync(Guid id, CancellationToken cancellationToken);
    Task<FabricDto> CreateFabricAsync(CreateFabricRequest request, CancellationToken cancellationToken);
    Task<FabricDto?> UpdateFabricAsync(Guid id, UpdateFabricRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteFabricAsync(Guid id, CancellationToken cancellationToken);

    Task<PagedResult<FabricMovementDto>> GetMovementsAsync(FabricQuery query, Guid? fabricId, CancellationToken cancellationToken);
    Task<FabricMovementDto> CreatePurchaseMovementAsync(CreateFabricPurchaseMovementRequest request, CancellationToken cancellationToken);
    Task<FabricMovementDto> CreateMovementAsync(CreateFabricMovementRequest request, CancellationToken cancellationToken);
    Task<FabricMovementDto> ConsumeFabricAsync(CreateFabricConsumptionRequest request, CancellationToken cancellationToken);

    Task<PagedResult<FabricReservationDto>> GetReservationsAsync(FabricQuery query, CancellationToken cancellationToken);
    Task<FabricReservationDto> CreateReservationAsync(CreateFabricReservationRequest request, CancellationToken cancellationToken);
    Task<FabricReservationDto?> UpdateReservationAsync(Guid id, UpdateFabricReservationRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteReservationAsync(Guid id, CancellationToken cancellationToken);
}
