using FluentValidation;

namespace FiolinOne.Application.Fabric;

public sealed class CreateFabricRequestValidator : AbstractValidator<CreateFabricRequest>
{
    public CreateFabricRequestValidator()
    {
        RuleFor(request => request.FabricCode).NotEmpty().MaximumLength(50);
        RuleFor(request => request.FabricName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.SupplierId).NotEmpty();
        RuleFor(request => request.ColorId).NotEmpty();
        RuleFor(request => request.Composition).MaximumLength(200);
        RuleFor(request => request.Width).GreaterThanOrEqualTo(0);
        RuleFor(request => request.WeightGsm).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Unit).NotEmpty().MaximumLength(30);
        RuleFor(request => request.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.CurrentStockKg).GreaterThanOrEqualTo(0);
        RuleFor(request => request.MinimumStock).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class UpdateFabricRequestValidator : AbstractValidator<UpdateFabricRequest>
{
    public UpdateFabricRequestValidator()
    {
        RuleFor(request => request.FabricCode).NotEmpty().MaximumLength(50);
        RuleFor(request => request.FabricName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.SupplierId).NotEmpty();
        RuleFor(request => request.ColorId).NotEmpty();
        RuleFor(request => request.Composition).MaximumLength(200);
        RuleFor(request => request.Width).GreaterThanOrEqualTo(0);
        RuleFor(request => request.WeightGsm).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Unit).NotEmpty().MaximumLength(30);
        RuleFor(request => request.PurchasePrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.MinimumStock).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateFabricPurchaseMovementRequestValidator : AbstractValidator<CreateFabricPurchaseMovementRequest>
{
    public CreateFabricPurchaseMovementRequestValidator()
    {
        RuleFor(request => request.SupplierId).NotEmpty();
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.ColorId).NotEmpty();
        RuleFor(request => request.BatchLot).MaximumLength(100);
        RuleFor(request => request.TotalWeightKg).GreaterThan(0);
        RuleFor(request => request.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Warehouse).NotEmpty().MaximumLength(150);
        RuleFor(request => request.ArrivalDate).NotEmpty();
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateFabricMovementRequestValidator : AbstractValidator<CreateFabricMovementRequest>
{
    public CreateFabricMovementRequestValidator()
    {
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.MovementType).NotEmpty().MaximumLength(50);
        RuleFor(request => request.QuantityKg).NotEqual(0);
        RuleFor(request => request.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.BatchLot).MaximumLength(100);
        RuleFor(request => request.Warehouse).NotEmpty().MaximumLength(150);
        RuleFor(request => request.MovementDate).NotEmpty();
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateFabricConsumptionRequestValidator : AbstractValidator<CreateFabricConsumptionRequest>
{
    public CreateFabricConsumptionRequestValidator()
    {
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.QuantityKg).GreaterThan(0);
        RuleFor(request => request.ProductionReference).NotEmpty().MaximumLength(100);
        RuleFor(request => request.ConsumptionDate).NotEmpty();
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateFabricReservationRequestValidator : AbstractValidator<CreateFabricReservationRequest>
{
    public CreateFabricReservationRequestValidator()
    {
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.ReservationNumber).NotEmpty().MaximumLength(50);
        RuleFor(request => request.ProductionReference).NotEmpty().MaximumLength(100);
        RuleFor(request => request.ReservedQuantityKg).GreaterThan(0);
        RuleFor(request => request.ReservationDate).NotEmpty();
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class UpdateFabricReservationRequestValidator : AbstractValidator<UpdateFabricReservationRequest>
{
    public UpdateFabricReservationRequestValidator()
    {
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.ReservationNumber).NotEmpty().MaximumLength(50);
        RuleFor(request => request.ProductionReference).NotEmpty().MaximumLength(100);
        RuleFor(request => request.ReservedQuantityKg).GreaterThan(0);
        RuleFor(request => request.ReservationDate).NotEmpty();
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}
