using FluentValidation;

namespace FiolinOne.Application.Products;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.ModelCode)
            .NotEmpty()
            .WithMessage("Model kodu zorunludur.")
            .MaximumLength(50)
            .WithMessage("Model kodu en fazla 50 karakter olabilir.");

        RuleFor(request => request.ProductCode)
            .MaximumLength(50)
            .WithMessage("Ürün kodu en fazla 50 karakter olabilir.");

        RuleFor(request => request.ProductName)
            .NotEmpty()
            .WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200)
            .WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(request => request.Status)
            .NotEmpty()
            .WithMessage("Durum zorunludur.")
            .MaximumLength(50)
            .WithMessage("Durum en fazla 50 karakter olabilir.");
    }
}

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(request => request.ModelCode)
            .NotEmpty()
            .WithMessage("Model kodu zorunludur.")
            .MaximumLength(50)
            .WithMessage("Model kodu en fazla 50 karakter olabilir.");

        RuleFor(request => request.ProductCode)
            .NotEmpty()
            .WithMessage("Ürün kodu zorunludur.")
            .MaximumLength(50)
            .WithMessage("Ürün kodu en fazla 50 karakter olabilir.");

        RuleFor(request => request.ProductName)
            .NotEmpty()
            .WithMessage("Ürün adı zorunludur.")
            .MaximumLength(200)
            .WithMessage("Ürün adı en fazla 200 karakter olabilir.");

        RuleFor(request => request.Status)
            .NotEmpty()
            .WithMessage("Durum zorunludur.")
            .MaximumLength(50)
            .WithMessage("Durum en fazla 50 karakter olabilir.");
    }
}
