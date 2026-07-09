using FluentValidation;

namespace FiolinOne.Application.Fabric;

public sealed class CreateFabricRequestValidator : AbstractValidator<CreateFabricRequest>
{
    public CreateFabricRequestValidator()
    {
        RuleFor(request => request.FabricCode).MaximumLength(50).WithMessage("Kumaş kodu en fazla 50 karakter olabilir.");
        RuleFor(request => request.FabricName).NotEmpty().WithMessage("Kumaş adı zorunludur.").MaximumLength(200).WithMessage("Kumaş adı en fazla 200 karakter olabilir.");
        RuleFor(request => request.SupplierId).NotEmpty().WithMessage("Tedarikçi seçiniz.");
        RuleFor(request => request.ColorId).NotEmpty().WithMessage("Renk seçiniz.");
        RuleFor(request => request.Composition).MaximumLength(200).WithMessage("Kompozisyon en fazla 200 karakter olabilir.");
        RuleFor(request => request.Width).GreaterThanOrEqualTo(0).WithMessage("En negatif olamaz.");
        RuleFor(request => request.WeightGsm).GreaterThanOrEqualTo(0).WithMessage("Gramaj negatif olamaz.");
        RuleFor(request => request.Unit).NotEmpty().WithMessage("Birim zorunludur.").MaximumLength(30).WithMessage("Birim en fazla 30 karakter olabilir.");
        RuleFor(request => request.PurchasePrice).GreaterThanOrEqualTo(0).WithMessage("Alış fiyatı negatif olamaz.");
        RuleFor(request => request.CurrentStockKg).GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");
        RuleFor(request => request.MinimumStock).GreaterThanOrEqualTo(0).WithMessage("Minimum stok negatif olamaz.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Notes).MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");
    }
}

public sealed class UpdateFabricRequestValidator : AbstractValidator<UpdateFabricRequest>
{
    public UpdateFabricRequestValidator()
    {
        RuleFor(request => request.FabricCode).NotEmpty().WithMessage("Kumaş kodu zorunludur.").MaximumLength(50).WithMessage("Kumaş kodu en fazla 50 karakter olabilir.");
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
        RuleFor(request => request.ReservationNumber).MaximumLength(50).WithMessage("Rezervasyon numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.ProductionReference).NotEmpty().WithMessage("Üretim referansı zorunludur.").MaximumLength(100).WithMessage("Üretim referansı en fazla 100 karakter olabilir.");
        RuleFor(request => request.ReservedQuantityKg).GreaterThan(0).WithMessage("Rezervasyon miktarı sıfırdan büyük olmalıdır.");
        RuleFor(request => request.ReservationDate).NotEmpty().WithMessage("Rezervasyon tarihi zorunludur.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Notes).MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");
    }
}

public sealed class UpdateFabricReservationRequestValidator : AbstractValidator<UpdateFabricReservationRequest>
{
    public UpdateFabricReservationRequestValidator()
    {
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.ReservationNumber).NotEmpty().WithMessage("Rezervasyon numarası zorunludur.").MaximumLength(50).WithMessage("Rezervasyon numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.ProductionReference).NotEmpty().MaximumLength(100);
        RuleFor(request => request.ReservedQuantityKg).GreaterThan(0);
        RuleFor(request => request.ReservationDate).NotEmpty();
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}
