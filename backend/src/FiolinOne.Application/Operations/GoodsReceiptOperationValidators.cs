using FluentValidation;

namespace FiolinOne.Application.Operations;

public sealed class CreateGoodsReceiptOperationRequestValidator : AbstractValidator<CreateGoodsReceiptOperationRequest>
{
    public CreateGoodsReceiptOperationRequestValidator()
    {
        RuleFor(request => request.SupplierId)
            .NotEmpty()
            .WithMessage("Tedarikçi seçiniz.");

        RuleFor(request => request.ProductVariantId)
            .NotEmpty()
            .WithMessage("Ürün seçiniz.");

        RuleFor(request => request.TransactionDate)
            .NotEmpty()
            .WithMessage("İşlem tarihi zorunludur.");

        RuleFor(request => request.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Alış fiyatı negatif olamaz.");

        RuleFor(request => request.Quantity)
            .GreaterThan(0)
            .WithMessage("Gelen adet 0'dan büyük olmalıdır.");

        RuleFor(request => request.Shelf)
            .MaximumLength(80)
            .WithMessage("Raf bilgisi en fazla 80 karakter olabilir.");

        RuleFor(request => request.Box)
            .MaximumLength(80)
            .WithMessage("Koli bilgisi en fazla 80 karakter olabilir.");
    }
}
