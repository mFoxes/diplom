using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using DTO;
using Domain.Models;
using GrandmaApi.Database.Specifications;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Validators;

public class UpdateDeviceCommandValidator : AbstractValidator<UpdateDeviceCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;
    public UpdateDeviceCommandValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        
        RuleFor(x => x.Device.Id)
            .MustAsync(IsDeviceExists)
            .OverridePropertyName(nameof(DeviceDto.Id))
            .WithMessage(localization.GetString(LocalizationKey.Device.NotFound));
        
        RuleFor(x => x.Device.Name)
            .NotEmpty()
            .WithMessage(localization.GetString(LocalizationKey.Shared.NotEmpty))
            .MaximumLength(255)
            .WithMessage(localization.GetString(LocalizationKey.Shared.MaxLength, 255))
            .OverridePropertyName(nameof(DeviceDto.Name));

        RuleFor(d => d.Device.InventoryNumber)
            .Matches(@"^[a-z]\.[0-9]{2}\.[0-9]{3}$")
            .WithMessage(localization.GetString(LocalizationKey.Device.InventoryNumberTemplate))
            .OverridePropertyName(nameof(DeviceDto.InventoryNumber));

        RuleFor(d => new { d.Device.InventoryNumber, d.Device.Id })
            .MustAsync(async (d, c) =>
            {
                if (!await IsDeviceExists(d.Id, c))
                {
                    return true;
                }

                var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(d.Id), c);
                return device.InventoryNumber == d.InventoryNumber || await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(d.InventoryNumber)), c) == null;
            })
            .WithMessage(localization.GetString(LocalizationKey.Device.InventoryNumberIsUsed))
            .OverridePropertyName(nameof(DeviceDto.InventoryNumber));

        RuleFor(d => d.Device.PhotoId)
            .MustAsync(IsFileExists)
            .OverridePropertyName(nameof(DeviceDto.PhotoId))
            .WithMessage(localization.GetString(LocalizationKey.File.FileNotFound));
    }

    private async Task<bool> IsDeviceExists(Guid id, CancellationToken cancellationToken) => await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(id), cancellationToken) != null;
    private async Task<bool> IsFileExists(Guid? id, CancellationToken cancellationToken) => id.HasValue && await _grandmaRepository.GetAsync(new NotDeleted<FileModel, Guid>(id.Value), cancellationToken) != null;
    
}