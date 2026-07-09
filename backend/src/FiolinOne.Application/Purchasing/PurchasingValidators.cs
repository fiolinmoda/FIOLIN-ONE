using FluentValidation;

namespace FiolinOne.Application.Purchasing;

public sealed class CreateSupplierRequestValidator : AbstractValidator<CreateSupplierRequest>
{
    public CreateSupplierRequestValidator()
    {
        RuleFor(request => request.SupplierCode).NotEmpty().MaximumLength(50);
        RuleFor(request => request.SupplierName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Phone).MaximumLength(50);
        RuleFor(request => request.Email).EmailAddress().MaximumLength(150).When(request => !string.IsNullOrWhiteSpace(request.Email));
        RuleFor(request => request.Address).MaximumLength(500);
        RuleFor(request => request.TaxNumber).MaximumLength(50);
        RuleFor(request => request.PaymentTerm).MaximumLength(100);
    }
}

public sealed class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest>
{
    public UpdateSupplierRequestValidator()
    {
        RuleFor(request => request.SupplierCode).NotEmpty().MaximumLength(50);
        RuleFor(request => request.SupplierName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Phone).MaximumLength(50);
        RuleFor(request => request.Email).EmailAddress().MaximumLength(150).When(request => !string.IsNullOrWhiteSpace(request.Email));
        RuleFor(request => request.Address).MaximumLength(500);
        RuleFor(request => request.TaxNumber).MaximumLength(50);
        RuleFor(request => request.PaymentTerm).MaximumLength(100);
    }
}

public sealed class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
{
    public CreatePurchaseOrderRequestValidator()
    {
        RuleFor(request => request.PurchaseNumber).MaximumLength(50).WithMessage("Satın alma numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.SupplierId).NotEmpty().WithMessage("Tedarikçi seçiniz.");
        RuleFor(request => request.OrderDate).NotEmpty().WithMessage("Sipariş tarihi zorunludur.");
        RuleFor(request => request.ExpectedDate)
            .GreaterThanOrEqualTo(request => request.OrderDate)
            .When(request => request.ExpectedDate.HasValue);
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Notes).MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");
        RuleFor(request => request.Items).NotNull();
        RuleForEach(request => request.Items).SetValidator(new PurchaseOrderItemRequestValidator());
    }
}

public sealed class UpdatePurchaseOrderRequestValidator : AbstractValidator<UpdatePurchaseOrderRequest>
{
    public UpdatePurchaseOrderRequestValidator()
    {
        RuleFor(request => request.PurchaseNumber).NotEmpty().WithMessage("Satın alma numarası zorunludur.").MaximumLength(50).WithMessage("Satın alma numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.SupplierId).NotEmpty();
        RuleFor(request => request.OrderDate).NotEmpty();
        RuleFor(request => request.ExpectedDate)
            .GreaterThanOrEqualTo(request => request.OrderDate)
            .When(request => request.ExpectedDate.HasValue);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
        RuleFor(request => request.Items).NotNull();
        RuleForEach(request => request.Items).SetValidator(new PurchaseOrderItemRequestValidator());
    }
}

public sealed class PurchaseOrderItemRequestValidator : AbstractValidator<PurchaseOrderItemRequest>
{
    public PurchaseOrderItemRequestValidator()
    {
        RuleFor(request => request.ItemName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Quantity).GreaterThan(0);
        RuleFor(request => request.Unit).NotEmpty().MaximumLength(30);
        RuleFor(request => request.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.ReceivedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreateGoodsReceiptRequestValidator : AbstractValidator<CreateGoodsReceiptRequest>
{
    public CreateGoodsReceiptRequestValidator()
    {
        RuleFor(request => request.ReceiptNumber).MaximumLength(50).WithMessage("Mal kabul numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.SupplierId).NotEmpty().WithMessage("Tedarikçi seçiniz.");
        RuleFor(request => request.ReceiptDate).NotEmpty().WithMessage("Kabul tarihi zorunludur.");
        RuleFor(request => request.Warehouse).NotEmpty().WithMessage("Depo zorunludur.").MaximumLength(150).WithMessage("Depo en fazla 150 karakter olabilir.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Notes).MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");
        RuleFor(request => request.Items).NotNull();
        RuleForEach(request => request.Items).SetValidator(new GoodsReceiptItemRequestValidator());
    }
}

public sealed class UpdateGoodsReceiptRequestValidator : AbstractValidator<UpdateGoodsReceiptRequest>
{
    public UpdateGoodsReceiptRequestValidator()
    {
        RuleFor(request => request.ReceiptNumber).NotEmpty().WithMessage("Mal kabul numarası zorunludur.").MaximumLength(50).WithMessage("Mal kabul numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.SupplierId).NotEmpty();
        RuleFor(request => request.ReceiptDate).NotEmpty();
        RuleFor(request => request.Warehouse).NotEmpty().MaximumLength(150);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
        RuleFor(request => request.Items).NotNull();
        RuleForEach(request => request.Items).SetValidator(new GoodsReceiptItemRequestValidator());
    }
}

public sealed class GoodsReceiptItemRequestValidator : AbstractValidator<GoodsReceiptItemRequest>
{
    public GoodsReceiptItemRequestValidator()
    {
        RuleFor(request => request.ItemName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.ReceivedQuantity).GreaterThan(0);
        RuleFor(request => request.Unit).NotEmpty().MaximumLength(30);
        RuleFor(request => request.Acceptance).NotEmpty().MaximumLength(50);
    }
}

public sealed class CreatePurchaseInvoiceRequestValidator : AbstractValidator<CreatePurchaseInvoiceRequest>
{
    public CreatePurchaseInvoiceRequestValidator()
    {
        RuleFor(request => request.InvoiceNumber).MaximumLength(50).WithMessage("Fatura numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.InvoiceDate).NotEmpty().WithMessage("Fatura tarihi zorunludur.");
        RuleFor(request => request.SupplierId).NotEmpty().WithMessage("Tedarikçi seçiniz.");
        RuleFor(request => request.InvoiceAmount).GreaterThanOrEqualTo(0).WithMessage("Fatura tutarı negatif olamaz.");
        RuleFor(request => request.Status).NotEmpty().WithMessage("Durum zorunludur.").MaximumLength(50).WithMessage("Durum en fazla 50 karakter olabilir.");
        RuleFor(request => request.Notes).MaximumLength(1000).WithMessage("Not en fazla 1000 karakter olabilir.");
        RuleFor(request => request.Items).NotNull();
        RuleForEach(request => request.Items).SetValidator(new PurchaseInvoiceItemRequestValidator());
    }
}

public sealed class UpdatePurchaseInvoiceRequestValidator : AbstractValidator<UpdatePurchaseInvoiceRequest>
{
    public UpdatePurchaseInvoiceRequestValidator()
    {
        RuleFor(request => request.InvoiceNumber).NotEmpty().WithMessage("Fatura numarası zorunludur.").MaximumLength(50).WithMessage("Fatura numarası en fazla 50 karakter olabilir.");
        RuleFor(request => request.InvoiceDate).NotEmpty();
        RuleFor(request => request.SupplierId).NotEmpty();
        RuleFor(request => request.InvoiceAmount).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Status).NotEmpty().MaximumLength(50);
        RuleFor(request => request.Notes).MaximumLength(1000);
        RuleFor(request => request.Items).NotNull();
        RuleForEach(request => request.Items).SetValidator(new PurchaseInvoiceItemRequestValidator());
    }
}

public sealed class PurchaseInvoiceItemRequestValidator : AbstractValidator<PurchaseInvoiceItemRequest>
{
    public PurchaseInvoiceItemRequestValidator()
    {
        RuleFor(request => request.ItemName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Quantity).GreaterThan(0);
        RuleFor(request => request.Unit).NotEmpty().MaximumLength(30);
        RuleFor(request => request.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(request => request.TotalAmount).GreaterThanOrEqualTo(0);
    }
}
