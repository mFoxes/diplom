using FluentValidation;
using GrandmaApi.Database;
using Domain.Models;
using GrandmaApi.Mediatr.Queries;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;

namespace GrandmaApi.Validators;
public class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    private readonly IGrandmaRepository _grandmaRepository;

    public GetBookingByIdQueryValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        RuleFor(b => b.Id)
            .MustAsync(IsBookingExists)
            .WithMessage(localization.GetString(LocalizationKey.Booking.NotFound))
            .OverridePropertyName(nameof(GetBookingByIdQuery.Id));
    }

    private async Task<bool> IsBookingExists(Guid id, CancellationToken cancellationToken)
    {
        return await _grandmaRepository.GetAsync(new NotDeleted<BookingModel, Guid>(id), cancellationToken) != null;
    }
}