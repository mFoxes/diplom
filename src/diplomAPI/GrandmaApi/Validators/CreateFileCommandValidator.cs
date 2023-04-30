using FluentValidation;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Models.Configs;
using Microsoft.Extensions.Options;
using GrandmaApi.Localization;

namespace GrandmaApi.Validators;

public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator(IOptions<ImagesConfig> config, ILocalizationService localizationService)
    {
        var configValue = config.Value;

        RuleFor(f => f)
            .Cascade(CascadeMode.Stop)
            .Must(f => f != null && f.File != null)
            .WithMessage(localizationService.GetString(LocalizationKey.File.FileNotFound))
            .Must(f => f.File.Length <= configValue.MaxSize)
            .WithMessage(localizationService.GetString(LocalizationKey.File.MaxSize, configValue.MaxSize / (1024 * 1024)))
            .Must(f => configValue.AllowedExtensions.Contains(Path.GetExtension(f.File.FileName)))
            .WithMessage(localizationService.GetString(LocalizationKey.File.AllowedExtensions, string.Join(", ", configValue.AllowedExtensions)))
            .OverridePropertyName("Photo");
    }
}