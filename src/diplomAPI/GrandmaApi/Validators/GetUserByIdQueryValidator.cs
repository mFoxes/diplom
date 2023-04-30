using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;

namespace GrandmaApi.Validators;

public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    private readonly IGrandmaRepository _grandmaRepository;

    public GetUserByIdQueryValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        RuleFor(u => u.Id)
            .MustAsync(IsUserExists)
            .OverridePropertyName(nameof(GetUserByIdQuery.Id))
            .WithMessage(localization.GetString(LocalizationKey.User.NotFound));
    }

    private async Task<bool> IsUserExists(Guid id, CancellationToken cancellationToken)
    {
        return await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(id), cancellationToken) != null;
    }
}