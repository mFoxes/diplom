using Domain.Enums;
using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Validators;

public class DeviceTookValidator : AbstractValidator<BookDeviceCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;
    public DeviceTookValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;

        RuleFor(d => d.DeviceTook.DeviceId)
            .Cascade(CascadeMode.Stop)
            .MustAsync(IsDeviceExists)
            .WithMessage(localization.GetString(LocalizationKey.Device.NotFound))
            .MustAsync(IsDeviceAlreadyBooked)
            .WithMessage(localization.GetString(LocalizationKey.Device.Booked))
            .OverridePropertyName(nameof(BookDeviceCommand.DeviceTook.DeviceId));

        RuleFor(d => d.DeviceTook.UserId)
            .MustAsync(IsUserExists)
            .WithMessage(localization.GetString(LocalizationKey.User.NotFound))
            .OverridePropertyName(nameof(BookDeviceCommand.DeviceTook.UserId));

        RuleFor(d => d.DeviceTook.ReturnAt)
            .GreaterThan(DateTime.Now)
            .WithMessage(localization.GetString(LocalizationKey.Shared.FutureDate))
            .OverridePropertyName(nameof(BookDeviceCommand.DeviceTook.ReturnAt));
    }

    private async Task<bool> IsDeviceExists(Guid id, CancellationToken cancellationToken) => await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(id), cancellationToken) != null;
    
    private async Task<bool> IsUserExists(Guid id, CancellationToken cancellationToken) => await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(id), cancellationToken) != null;

    private async Task<bool> IsDeviceAlreadyBooked(Guid deviceId, CancellationToken cancellationToken)
    {
       var booking = await _grandmaRepository.GetAsync(new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(deviceId)), cancellationToken);
       return booking.State == DeviceStates.Free;
    }
}