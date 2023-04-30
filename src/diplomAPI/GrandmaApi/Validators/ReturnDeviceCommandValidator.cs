using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using GrandmaApi.Database.Specifications;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;
using Domain.Enums;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Validators;

public class ReturnDeviceCommandValidator : AbstractValidator<ReturnDeviceCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;
    
    public ReturnDeviceCommandValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        RuleFor(d => d.InventoryNumberMessage.InventoryNumber)
            .Cascade(CascadeMode.Stop)
            .MustAsync(IsDeviceExists)
            .WithMessage(localization.GetString(LocalizationKey.Device.InventoryNumberNotFound))
            .MustAsync(IsDeviceBooked)
            .WithMessage(localization.GetString(LocalizationKey.Device.NotBooked))
            .OverridePropertyName(nameof(ReturnDeviceCommand.InventoryNumberMessage.InventoryNumber));
    }

    private async Task<bool> IsDeviceBooked(string inventoryNumber, CancellationToken cancellationToken)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(inventoryNumber)), cancellationToken);
        var booking =
            await _grandmaRepository.GetAsync(
                new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(device.Id)), cancellationToken);
        return booking.State == DeviceStates.Booked;
    }

    private async Task<bool> IsDeviceExists(string inventoryNumber, CancellationToken cancellationToken)
    {
        return await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(inventoryNumber)), cancellationToken) != null;
    }
}