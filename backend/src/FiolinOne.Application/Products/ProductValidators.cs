using FluentValidation;

namespace FiolinOne.Application.Products;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.ProductCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(request => request.ProductName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Status)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.ProductCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(request => request.ProductName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Status)
            .NotEmpty()
            .MaximumLength(50);
    }
}
