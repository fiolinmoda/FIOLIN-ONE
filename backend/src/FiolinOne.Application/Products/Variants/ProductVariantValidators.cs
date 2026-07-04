using FluentValidation;

namespace FiolinOne.Application.Products.Variants;

public sealed class CreateProductVariantRequestValidator : AbstractValidator<CreateProductVariantRequest>
{
    public CreateProductVariantRequestValidator()
    {
        RuleFor(request => request.Color)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Size)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(request => request.Barcode)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.TrendyolSku)
            .MaximumLength(100);

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.Status)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class UpdateProductVariantRequestValidator : AbstractValidator<UpdateProductVariantRequest>
{
    public UpdateProductVariantRequestValidator()
    {
        RuleFor(request => request.Color)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Size)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(request => request.Barcode)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.TrendyolSku)
            .MaximumLength(100);

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.Status)
            .NotEmpty()
            .MaximumLength(50);
    }
}
