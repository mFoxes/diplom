using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Localization;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Validators;

public class GetDeviceInfoQueryValidator : AbstractValidator<GetDeviceInfoByInventoryNumberQuery>
{
    private readonly IGrandmaRepository _grandmaRepository;

    public GetDeviceInfoQueryValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;
        RuleFor(i => i.InventoryNumberMessage.InventoryNumber)
            .MustAsync(IsDeviceExists)
            .OverridePropertyName(nameof(GetDeviceInfoByInventoryNumberQuery.InventoryNumberMessage.InventoryNumber))
            .WithMessage(localization.GetString(LocalizationKey.Device.InventoryNumberNotFound));
    }

    private async Task<bool> IsDeviceExists(string inventoryNumber, CancellationToken cancellationToken)
    {
        return await _grandmaRepository.GetAsync(
            new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(inventoryNumber)), cancellationToken) != null;
    }
}