using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Database.Specifications;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;
using DTO;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Validators;

public class CreateDeviceCommandValidator : AbstractValidator<CreateDeviceCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;
    public CreateDeviceCommandValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        
        RuleFor(x => x.Device.Name)
            .NotEmpty()
            .WithMessage(localization.GetString(LocalizationKey.Shared.NotEmpty))
            .MaximumLength(255)
            .WithMessage(localization.GetString(LocalizationKey.Shared.MaxLength, 255))
            .OverridePropertyName(nameof(DeviceDto.Name));

        RuleFor(d => d.Device.InventoryNumber)
            .Cascade(CascadeMode.Stop)
            .Matches(@"^[a-z]\.[0-9]{2}\.[0-9]{3}$")
            .WithMessage(localization.GetString(LocalizationKey.Device.InventoryNumberTemplate))
            .MustAsync(IsInventoryNumberIsUsing)
            .WithMessage(localization.GetString(LocalizationKey.Device.InventoryNumberIsUsed))
            .OverridePropertyName(nameof(DeviceDto.InventoryNumber));

        RuleFor(d => d.Device.PhotoId)
            .MustAsync(IsFileExists)
            .OverridePropertyName(nameof(DeviceDto.PhotoId))
            .WithMessage(localization.GetString(LocalizationKey.File.FileNotFound));
    }

    private async Task<bool> IsFileExists(Guid? fileId, CancellationToken cancellationToken)
    {
        return fileId.HasValue && await _grandmaRepository.GetAsync(new NotDeleted<FileModel, Guid>(fileId.Value), cancellationToken) != null;
    }

    private async Task<bool> IsInventoryNumberIsUsing(string inventoryNumber, CancellationToken cancellationToken) =>
        await _grandmaRepository.GetAsync(
            new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(inventoryNumber)), cancellationToken) == null;
}