using FluentValidation;

namespace FiolinOne.Application.MasterData;

public sealed class CreateMasterDataRequestValidator : AbstractValidator<CreateMasterDataRequest>
{
    public CreateMasterDataRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("İsim boş bırakılamaz.")
            .MaximumLength(150)
            .WithMessage("İsim en fazla 150 karakter olabilir.");

        RuleFor(request => request.Code)
            .MaximumLength(50)
            .WithMessage("Kod en fazla 50 karakter olabilir.");

        RuleFor(request => request.SortOrder)
            .GreaterThanOrEqualTo(0)
            .When(request => request.SortOrder.HasValue)
            .WithMessage("Sıra negatif olamaz.");
    }
}

public sealed class UpdateMasterDataRequestValidator : AbstractValidator<UpdateMasterDataRequest>
{
    public UpdateMasterDataRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("İsim boş bırakılamaz.")
            .MaximumLength(150)
            .WithMessage("İsim en fazla 150 karakter olabilir.");

        RuleFor(request => request.Code)
            .MaximumLength(50)
            .WithMessage("Kod en fazla 50 karakter olabilir.");

        RuleFor(request => request.SortOrder)
            .GreaterThanOrEqualTo(0)
            .When(request => request.SortOrder.HasValue)
            .WithMessage("Sıra negatif olamaz.");
    }
}

public sealed class ReorderMasterDataRequestValidator : AbstractValidator<ReorderMasterDataRequest>
{
    public ReorderMasterDataRequestValidator()
    {
        RuleFor(request => request.ItemIds)
            .NotEmpty()
            .WithMessage("Sıralanacak kayıt bulunamadı.");

        RuleFor(request => request.ItemIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Sıralama listesinde aynı kayıt birden fazla kez yer alıyor.");
    }
}
