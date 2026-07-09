using FluentValidation;

namespace FiolinOne.Application.Production;

public sealed class CreateProductionOrderRequestValidator : AbstractValidator<CreateProductionOrderRequest>
{
    public CreateProductionOrderRequestValidator()
    {
        RuleFor(request => request.ProductionNumber).MaximumLength(50).WithMessage("Üretim numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.ProductId).NotEmpty().WithMessage("Ürün seçiniz.");
        RuleFor(request => request.PlannedQuantity).GreaterThan(0).WithMessage("Planlanan miktar sıfırdan büyük olmalıdır.");
        RuleFor(request => request.ProductionReason).NotEmpty().WithMessage("Üretim nedeni zorunludur.").MaximumLength(50).WithMessage("Üretim nedeni en fazla 50 karakter olabilir.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Notes).MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");
        RuleForEach(request => request.Items).SetValidator(new ProductionOrderItemRequestValidator());
    }
}

public sealed class UpdateProductionOrderRequestValidator : AbstractValidator<UpdateProductionOrderRequest>
{
    public UpdateProductionOrderRequestValidator()
    {
        RuleFor(request => request.ProductionNumber).NotEmpty().WithMessage("Üretim numarası zorunludur.").MaximumLength(50).WithMessage("Üretim numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.ProductId).NotEmpty();
        RuleFor(request => request.PlannedQuantity).GreaterThan(0);
        RuleFor(request => request.ProductionReason).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
        RuleForEach(request => request.Items).SetValidator(new ProductionOrderItemRequestValidator());
    }
}

public sealed class ProductionOrderItemRequestValidator : AbstractValidator<ProductionOrderItemRequest>
{
    public ProductionOrderItemRequestValidator()
    {
        RuleFor(request => request.ProductVariantId).NotEmpty();
        RuleFor(request => request.PlannedQuantity).GreaterThan(0);
    }
}

public sealed class CreateCuttingRecordRequestValidator : AbstractValidator<CreateCuttingRecordRequest>
{
    public CreateCuttingRecordRequestValidator()
    {
        RuleFor(request => request.ProductionOrderId).NotEmpty();
        RuleFor(request => request.FabricId).NotEmpty();
        RuleFor(request => request.ConsumedWeightKg).GreaterThan(0);
        RuleFor(request => request.WasteWeightKg).GreaterThanOrEqualTo(0);
        RuleFor(request => request.OperatorName).MaximumLength(100);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateWorkshopShipmentRequestValidator : AbstractValidator<CreateWorkshopShipmentRequest>
{
    public CreateWorkshopShipmentRequestValidator()
    {
        RuleFor(request => request.ProductionOrderId).NotEmpty();
        RuleFor(request => request.Workshop).NotEmpty().MaximumLength(150);
        RuleFor(request => request.SentQuantity).GreaterThan(0);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateWorkshopReturnRequestValidator : AbstractValidator<CreateWorkshopReturnRequest>
{
    public CreateWorkshopReturnRequestValidator()
    {
        RuleFor(request => request.ProductionOrderId).NotEmpty();
        RuleFor(request => request.ReturnedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(request => request.ExtraQuantity).GreaterThanOrEqualTo(0);
        RuleFor(request => request.MissingQuantity).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}

public sealed class CreateWarehouseEntryRequestValidator : AbstractValidator<CreateWarehouseEntryRequest>
{
    public CreateWarehouseEntryRequestValidator()
    {
        RuleFor(request => request.ProductionOrderId).NotEmpty();
        RuleFor(request => request.ActualQuantity).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Notes).MaximumLength(1000);
    }
}
