using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Common.Interfaces;
using FiolinOne.Domain.Fabric;

namespace FiolinOne.Application.Fabric;

public sealed class FabricService(
    IFabricRepository fabricRepository,
    IDocumentNumberGenerator documentNumberGenerator) : IFabricService
{
    public async Task<PagedResult<FabricDto>> GetFabricsAsync(FabricQuery query, CancellationToken cancellationToken)
    {
        var result = await fabricRepository.GetFabricsAsync(query.ToParameters(), cancellationToken);
        var items = new List<FabricDto>();

        foreach (var fabric in result.Items)
        {
            items.Add(await ToDtoAsync(fabric, cancellationToken));
        }

        return new PagedResult<FabricDto>(items, result.Page, result.PageSize, result.TotalItems, result.TotalPages);
    }

    public async Task<FabricDto?> GetFabricAsync(Guid id, CancellationToken cancellationToken)
    {
        var fabric = await fabricRepository.GetFabricByIdAsync(id, cancellationToken);

        return fabric is null ? null : await ToDtoAsync(fabric, cancellationToken);
    }

    public async Task<FabricDto> CreateFabricAsync(CreateFabricRequest request, CancellationToken cancellationToken)
    {
        var fabricCode = await GetDocumentNumberAsync(request.FabricCode, DocumentNumberTypes.Fabric, cancellationToken);
        await EnsureFabricCodeIsUniqueAsync(fabricCode, null, cancellationToken);
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsureColorExistsAsync(request.ColorId, cancellationToken);

        var fabric = new Domain.Fabric.Fabric(
            fabricCode,
            request.FabricName.Trim(),
            request.SupplierId,
            request.ColorId,
            NormalizeOptional(request.Composition),
            request.Width,
            request.WeightGsm,
            request.Unit.Trim(),
            request.PurchasePrice,
            request.CurrentStockKg,
            request.MinimumStock,
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await fabricRepository.AddFabricAsync(fabric, cancellationToken);
        await fabricRepository.SaveChangesAsync(cancellationToken);

        return await ToDtoAsync(fabric, cancellationToken);
    }

    public async Task<FabricDto?> UpdateFabricAsync(Guid id, UpdateFabricRequest request, CancellationToken cancellationToken)
    {
        var fabric = await fabricRepository.GetFabricByIdAsync(id, cancellationToken);

        if (fabric is null)
        {
            return null;
        }

        await EnsureFabricCodeIsUniqueAsync(request.FabricCode, id, cancellationToken);
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsureColorExistsAsync(request.ColorId, cancellationToken);

        fabric.Update(
            request.FabricCode.Trim(),
            request.FabricName.Trim(),
            request.SupplierId,
            request.ColorId,
            NormalizeOptional(request.Composition),
            request.Width,
            request.WeightGsm,
            request.Unit.Trim(),
            request.PurchasePrice,
            request.MinimumStock,
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await fabricRepository.SaveChangesAsync(cancellationToken);

        return await ToDtoAsync(fabric, cancellationToken);
    }

    public async Task<bool> DeleteFabricAsync(Guid id, CancellationToken cancellationToken)
    {
        var fabric = await fabricRepository.GetFabricByIdAsync(id, cancellationToken);

        if (fabric is null)
        {
            return false;
        }

        fabric.SoftDelete();
        await fabricRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<PagedResult<FabricMovementDto>> GetMovementsAsync(
        FabricQuery query,
        Guid? fabricId,
        CancellationToken cancellationToken)
    {
        var result = await fabricRepository.GetMovementsAsync(query.ToParameters(), fabricId, cancellationToken);

        return new PagedResult<FabricMovementDto>(
            result.Items.Select(ToDto).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages);
    }

    public async Task<FabricMovementDto> CreatePurchaseMovementAsync(CreateFabricPurchaseMovementRequest request, CancellationToken cancellationToken)
    {
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseOrderExistsAsync(request.PurchaseOrderId, cancellationToken);
        await EnsureColorExistsAsync(request.ColorId, cancellationToken);

        var fabric = await GetRequiredFabricAsync(request.FabricId, cancellationToken);

        if (fabric.SupplierId != request.SupplierId)
        {
            throw new InvalidOperationException("Fabric can only be purchased from its assigned fabric supplier.");
        }

        if (fabric.ColorId != request.ColorId)
        {
            throw new InvalidOperationException("Selected color does not match the fabric card color.");
        }

        fabric.ApplyStockChange(request.TotalWeightKg);
        var movement = new FabricMovement(
            request.FabricId,
            FabricMovementTypes.Purchase,
            request.TotalWeightKg,
            request.UnitPrice,
            request.SupplierId,
            request.PurchaseOrderId,
            NormalizeOptional(request.BatchLot),
            request.Warehouse.Trim(),
            ToUtc(request.ArrivalDate),
            NormalizeOptional(request.Notes));

        await fabricRepository.AddMovementAsync(movement, cancellationToken);
        await fabricRepository.SaveChangesAsync(cancellationToken);

        return ToDto(movement);
    }

    public async Task<FabricMovementDto> CreateMovementAsync(CreateFabricMovementRequest request, CancellationToken cancellationToken)
    {
        await EnsureSupplierExistsAsync(request.SupplierId, cancellationToken);
        await EnsurePurchaseOrderExistsAsync(request.PurchaseOrderId, cancellationToken);

        var fabric = await GetRequiredFabricAsync(request.FabricId, cancellationToken);
        var stockDelta = GetStockDelta(request.MovementType, request.QuantityKg);
        fabric.ApplyStockChange(stockDelta);

        var movement = new FabricMovement(
            request.FabricId,
            request.MovementType.Trim(),
            request.QuantityKg,
            request.UnitPrice,
            request.SupplierId,
            request.PurchaseOrderId,
            NormalizeOptional(request.BatchLot),
            request.Warehouse.Trim(),
            ToUtc(request.MovementDate),
            NormalizeOptional(request.Notes));

        await fabricRepository.AddMovementAsync(movement, cancellationToken);
        await fabricRepository.SaveChangesAsync(cancellationToken);

        return ToDto(movement);
    }

    public async Task<FabricMovementDto> ConsumeFabricAsync(CreateFabricConsumptionRequest request, CancellationToken cancellationToken)
    {
        var fabric = await GetRequiredFabricAsync(request.FabricId, cancellationToken);
        fabric.ApplyStockChange(-request.QuantityKg);

        var movement = new FabricMovement(
            request.FabricId,
            FabricMovementTypes.ProductionConsumption,
            request.QuantityKg,
            0,
            null,
            null,
            null,
            "Production",
            ToUtc(request.ConsumptionDate),
            NormalizeOptional($"{request.ProductionReference} {request.Notes}".Trim()));

        await fabricRepository.AddMovementAsync(movement, cancellationToken);
        await fabricRepository.SaveChangesAsync(cancellationToken);

        return ToDto(movement);
    }

    public async Task<PagedResult<FabricReservationDto>> GetReservationsAsync(FabricQuery query, CancellationToken cancellationToken)
    {
        var result = await fabricRepository.GetReservationsAsync(query.ToParameters(), cancellationToken);

        return new PagedResult<FabricReservationDto>(
            result.Items.Select(ToDto).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages);
    }

    public async Task<FabricReservationDto> CreateReservationAsync(CreateFabricReservationRequest request, CancellationToken cancellationToken)
    {
        var reservationNumber = await GetDocumentNumberAsync(request.ReservationNumber, DocumentNumberTypes.FabricReservation, cancellationToken);
        await EnsureReservationNumberIsUniqueAsync(reservationNumber, null, cancellationToken);
        await EnsureReservationFitsStockAsync(request.FabricId, request.ReservedQuantityKg, null, cancellationToken);

        var reservation = new FabricReservation(
            request.FabricId,
            reservationNumber,
            request.ProductionReference.Trim(),
            request.ReservedQuantityKg,
            ToUtc(request.ReservationDate),
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await fabricRepository.AddReservationAsync(reservation, cancellationToken);
        await fabricRepository.SaveChangesAsync(cancellationToken);

        reservation = await fabricRepository.GetReservationByIdAsync(reservation.Id, cancellationToken) ?? reservation;

        return ToDto(reservation);
    }

    public async Task<FabricReservationDto?> UpdateReservationAsync(Guid id, UpdateFabricReservationRequest request, CancellationToken cancellationToken)
    {
        var reservation = await fabricRepository.GetReservationByIdAsync(id, cancellationToken);

        if (reservation is null)
        {
            return null;
        }

        await EnsureReservationNumberIsUniqueAsync(request.ReservationNumber, id, cancellationToken);
        await EnsureReservationFitsStockAsync(request.FabricId, request.ReservedQuantityKg, id, cancellationToken);

        reservation.Update(
            request.FabricId,
            request.ReservationNumber.Trim(),
            request.ProductionReference.Trim(),
            request.ReservedQuantityKg,
            ToUtc(request.ReservationDate),
            request.Status.Trim(),
            NormalizeOptional(request.Notes));

        await fabricRepository.SaveChangesAsync(cancellationToken);

        reservation = await fabricRepository.GetReservationByIdAsync(id, cancellationToken) ?? reservation;

        return ToDto(reservation);
    }

    public async Task<bool> DeleteReservationAsync(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await fabricRepository.GetReservationByIdAsync(id, cancellationToken);

        if (reservation is null)
        {
            return false;
        }

        reservation.SoftDelete();
        await fabricRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<Domain.Fabric.Fabric> GetRequiredFabricAsync(Guid fabricId, CancellationToken cancellationToken)
    {
        return await fabricRepository.GetFabricByIdAsync(fabricId, cancellationToken)
            ?? throw new InvalidOperationException("Fabric does not exist.");
    }

    private async Task EnsureFabricCodeIsUniqueAsync(string fabricCode, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await fabricRepository.FabricCodeExistsAsync(fabricCode.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Fabric code already exists.");
        }
    }

    private async Task EnsureSupplierExistsAsync(Guid? supplierId, CancellationToken cancellationToken)
    {
        if (supplierId.HasValue && !await fabricRepository.SupplierExistsAsync(supplierId.Value, cancellationToken))
        {
            throw new InvalidOperationException("Supplier does not exist.");
        }
    }

    private async Task EnsurePurchaseOrderExistsAsync(Guid? purchaseOrderId, CancellationToken cancellationToken)
    {
        if (purchaseOrderId.HasValue && !await fabricRepository.PurchaseOrderExistsAsync(purchaseOrderId.Value, cancellationToken))
        {
            throw new InvalidOperationException("Purchase order does not exist.");
        }
    }

    private async Task EnsureColorExistsAsync(Guid colorId, CancellationToken cancellationToken)
    {
        if (!await fabricRepository.ColorExistsAsync(colorId, cancellationToken))
        {
            throw new InvalidOperationException("Color does not exist.");
        }
    }

    private async Task EnsureReservationNumberIsUniqueAsync(string reservationNumber, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (await fabricRepository.ReservationNumberExistsAsync(reservationNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Reservation number already exists.");
        }
    }

    private async Task EnsureReservationFitsStockAsync(
        Guid fabricId,
        decimal reservedQuantity,
        Guid? excludedReservationId,
        CancellationToken cancellationToken)
    {
        var fabric = await GetRequiredFabricAsync(fabricId, cancellationToken);
        var otherReservedQuantity = await fabricRepository.GetReservedQuantityAsync(fabricId, excludedReservationId, cancellationToken);

        if (reservedQuantity > fabric.CurrentStockKg - otherReservedQuantity)
        {
            throw new InvalidOperationException("Reservation cannot exceed available fabric stock.");
        }
    }

    private static decimal GetStockDelta(string movementType, decimal quantityKg)
    {
        return movementType.Trim() switch
        {
            FabricMovementTypes.Purchase => quantityKg,
            FabricMovementTypes.Return => quantityKg,
            FabricMovementTypes.ManualAdjustment => quantityKg,
            FabricMovementTypes.InventoryCount => quantityKg,
            FabricMovementTypes.ProductionConsumption => -Math.Abs(quantityKg),
            _ => throw new InvalidOperationException("Unsupported fabric movement type.")
        };
    }

    private async Task<FabricDto> ToDtoAsync(Domain.Fabric.Fabric fabric, CancellationToken cancellationToken)
    {
        var reservedQuantity = await fabricRepository.GetReservedQuantityAsync(fabric.Id, null, cancellationToken);

        return new FabricDto(
            fabric.Id,
            fabric.FabricCode,
            fabric.FabricName,
            fabric.SupplierId,
            fabric.Supplier?.SupplierName ?? string.Empty,
            fabric.ColorId,
            fabric.Color?.Name ?? string.Empty,
            fabric.Composition,
            fabric.Width,
            fabric.WeightGsm,
            fabric.Unit,
            fabric.PurchasePrice,
            fabric.CurrentStockKg,
            fabric.MinimumStock,
            reservedQuantity,
            fabric.CurrentStockKg - reservedQuantity,
            fabric.Status,
            fabric.Notes,
            fabric.CreatedAt,
            fabric.UpdatedAt);
    }

    private static FabricMovementDto ToDto(FabricMovement movement)
    {
        return new FabricMovementDto(
            movement.Id,
            movement.FabricId,
            movement.Fabric?.FabricCode ?? string.Empty,
            movement.Fabric?.FabricName ?? string.Empty,
            movement.MovementType,
            movement.QuantityKg,
            movement.UnitPrice,
            movement.SupplierId,
            movement.Supplier?.SupplierName,
            movement.PurchaseOrderId,
            movement.PurchaseOrder?.PurchaseNumber,
            movement.BatchLot,
            movement.Warehouse,
            movement.MovementDate,
            movement.Notes,
            movement.CreatedAt);
    }

    private static FabricReservationDto ToDto(FabricReservation reservation)
    {
        return new FabricReservationDto(
            reservation.Id,
            reservation.FabricId,
            reservation.Fabric?.FabricCode ?? string.Empty,
            reservation.Fabric?.FabricName ?? string.Empty,
            reservation.ReservationNumber,
            reservation.ProductionReference,
            reservation.ReservedQuantityKg,
            reservation.ReservationDate,
            reservation.Status,
            reservation.Notes,
            reservation.CreatedAt,
            reservation.UpdatedAt);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static DateTime ToUtc(DateTime value)
    {
        return value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    private async Task<string> GetDocumentNumberAsync(string? requestedNumber, string documentType, CancellationToken cancellationToken)
    {
        return string.IsNullOrWhiteSpace(requestedNumber)
            ? await documentNumberGenerator.GenerateAsync(documentType, cancellationToken)
            : requestedNumber.Trim();
    }
}
