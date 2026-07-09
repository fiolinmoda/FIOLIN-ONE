using FluentValidation;

namespace FiolinOne.Application.Sales;

public sealed class CreateSalesOrderRequestValidator : AbstractValidator<CreateSalesOrderRequest>
{
    public CreateSalesOrderRequestValidator()
    {
        RuleFor(request => request.SalesOrderNumber).MaximumLength(50).WithMessage("Satış siparişi numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.CustomerName).NotEmpty().WithMessage("Müşteri adı zorunludur.").MaximumLength(200).WithMessage("Müşteri adı en fazla 200 karakter olabilir.");
        RuleFor(request => request.OrderDate).NotEmpty().WithMessage("Sipariş tarihi zorunludur.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Items).NotEmpty().WithMessage("En az bir ürün satırı ekleyiniz.");
        RuleForEach(request => request.Items).SetValidator(new SalesOrderItemRequestValidator());
    }
}

public sealed class UpdateSalesOrderRequestValidator : AbstractValidator<UpdateSalesOrderRequest>
{
    public UpdateSalesOrderRequestValidator()
    {
        RuleFor(request => request.SalesOrderNumber).NotEmpty().WithMessage("Satış siparişi numarası zorunludur.").MaximumLength(50).WithMessage("Satış siparişi numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.CustomerName).NotEmpty().WithMessage("Müşteri adı zorunludur.").MaximumLength(200).WithMessage("Müşteri adı en fazla 200 karakter olabilir.");
        RuleFor(request => request.OrderDate).NotEmpty().WithMessage("Sipariş tarihi zorunludur.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Items).NotEmpty().WithMessage("En az bir ürün satırı ekleyiniz.");
        RuleForEach(request => request.Items).SetValidator(new SalesOrderItemRequestValidator());
    }
}

public sealed class SalesOrderItemRequestValidator : AbstractValidator<SalesOrderItemRequest>
{
    public SalesOrderItemRequestValidator()
    {
        RuleFor(request => request.ProductVariantId).NotEmpty().WithMessage("Ürün varyantı seçiniz.");
        RuleFor(request => request.Quantity).GreaterThan(0).WithMessage("Satış miktarı sıfırdan büyük olmalıdır.");
        RuleFor(request => request.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("Birim fiyat negatif olamaz.");
    }
}
