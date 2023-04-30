using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Localization;
using Domain.Enums;

namespace GrandmaApi.Validators;

public class DeleteDeviceCommandValidator : AbstractValidator<DeleteDeviceCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;
    public DeleteDeviceCommandValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        RuleFor(d => d.Id)
            .Cascade(CascadeMode.Stop)
            .MustAsync(IsDeviceExists)
            .OverridePropertyName(nameof(DeleteDeviceCommand.Id))
            .WithMessage(localization.GetString(LocalizationKey.Device.NotFound))
            .MustAsync(IsDeviceFree)
            .OverridePropertyName(nameof(DeleteDeviceCommand.Id))
            .WithMessage(localization.GetString(LocalizationKey.Device.Booked));
    }

    private async Task<bool> IsDeviceExists(Guid id, CancellationToken cancellationToken)
    {
        return await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(id), cancellationToken) != null;
    }
    private async Task<bool> IsDeviceFree(Guid deviceId, CancellationToken cancellationToken)
    {
        var booking = await _grandmaRepository
            .GetAsync(new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(deviceId)), cancellationToken);
        return booking.State == DeviceStates.Free;
    }
}