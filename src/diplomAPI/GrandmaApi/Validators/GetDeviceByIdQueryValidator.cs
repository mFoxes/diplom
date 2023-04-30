using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;

namespace GrandmaApi.Validators;

public class GetDeviceByIdQueryValidator : AbstractValidator<GetDeviceByIdQuery>
{
    private readonly IGrandmaRepository _grandmaRepository;

    public GetDeviceByIdQueryValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        RuleFor(d => d.DeviceId)
            .MustAsync(IsDeviceExists)
            .OverridePropertyName(nameof(GetDeviceByIdQuery.DeviceId))
            .WithMessage(localization.GetString(LocalizationKey.Device.NotFound));
    }

    private async Task<bool> IsDeviceExists(Guid deviceId, CancellationToken cancellationToken)
    {
        return await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(deviceId), cancellationToken) != null;
    }
}