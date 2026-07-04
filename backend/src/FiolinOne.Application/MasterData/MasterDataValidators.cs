using FluentValidation;

namespace FiolinOne.Application.MasterData;

public sealed class CreateMasterDataRequestValidator : AbstractValidator<CreateMasterDataRequest>
{
    public CreateMasterDataRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(150);
        RuleFor(request => request.Code).NotEmpty().MaximumLength(50);
        RuleFor(request => request.SortOrder).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateMasterDataRequestValidator : AbstractValidator<UpdateMasterDataRequest>
{
    public UpdateMasterDataRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(150);
        RuleFor(request => request.Code).NotEmpty().MaximumLength(50);
        RuleFor(request => request.SortOrder).GreaterThanOrEqualTo(0);
    }
}
