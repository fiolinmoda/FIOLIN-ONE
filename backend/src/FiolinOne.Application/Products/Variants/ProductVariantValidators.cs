using FluentValidation;

namespace FiolinOne.Application.Products.Variants;

public sealed class CreateProductVariantRequestValidator : AbstractValidator<CreateProductVariantRequest>
{
    public CreateProductVariantRequestValidator()
    {
        RuleFor(request => request.ColorId)
            .NotEmpty()
            .WithMessage("Renk zorunludur.");

        RuleFor(request => request.SizeId)
            .NotEmpty()
            .WithMessage("Beden zorunludur.");

        RuleFor(request => request.Barcode)
            .NotEmpty()
            .WithMessage("Barkod zorunludur.")
            .MaximumLength(100)
            .WithMessage("Barkod en fazla 100 karakter olabilir.");

        RuleFor(request => request.TrendyolSku)
            .MaximumLength(100)
            .WithMessage("Trendyol SKU en fazla 100 karakter olabilir.");

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stok negatif olamaz.");

        RuleFor(request => request.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.PurchasePrice.HasValue)
            .WithMessage("Alış fiyatı negatif olamaz.");

        RuleFor(request => request.SalesPrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.SalesPrice.HasValue)
            .WithMessage("Satış fiyatı negatif olamaz.");

        RuleFor(request => request.Status)
            .NotEmpty()
            .WithMessage("Durum zorunludur.")
            .MaximumLength(50)
            .WithMessage("Durum en fazla 50 karakter olabilir.");
    }
}

public sealed class UpdateProductVariantRequestValidator : AbstractValidator<UpdateProductVariantRequest>
{
    public UpdateProductVariantRequestValidator()
    {
        RuleFor(request => request.ColorId)
            .NotEmpty()
            .WithMessage("Renk zorunludur.");

        RuleFor(request => request.SizeId)
            .NotEmpty()
            .WithMessage("Beden zorunludur.");

        RuleFor(request => request.Barcode)
            .NotEmpty()
            .WithMessage("Barkod zorunludur.")
            .MaximumLength(100)
            .WithMessage("Barkod en fazla 100 karakter olabilir.");

        RuleFor(request => request.TrendyolSku)
            .MaximumLength(100)
            .WithMessage("Trendyol SKU en fazla 100 karakter olabilir.");

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stok negatif olamaz.");

        RuleFor(request => request.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.PurchasePrice.HasValue)
            .WithMessage("Alış fiyatı negatif olamaz.");

        RuleFor(request => request.SalesPrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.SalesPrice.HasValue)
            .WithMessage("Satış fiyatı negatif olamaz.");

        RuleFor(request => request.Status)
            .NotEmpty()
            .WithMessage("Durum zorunludur.")
            .MaximumLength(50)
            .WithMessage("Durum en fazla 50 karakter olabilir.");
    }
}
