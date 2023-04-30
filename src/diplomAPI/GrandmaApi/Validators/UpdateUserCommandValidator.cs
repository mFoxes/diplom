using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using DTO;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;

namespace GrandmaApi.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;
    public UpdateUserCommandValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;

        RuleFor(u => u.User.Id)
            .MustAsync(IsUserExists)
            .OverridePropertyName(nameof(UserCardDto.Id))
            .WithMessage(localization.GetString(LocalizationKey.User.NotFound));

        RuleFor(u => u.User.Name)
            .NotEmpty()
            .WithMessage(localization.GetString(LocalizationKey.Shared.NotEmpty))
            .MaximumLength(80)
            .WithMessage(localization.GetString(LocalizationKey.Shared.MaxLength, 80))
            .Matches(@"^[А-яЁё ]+$")
            .WithMessage(localization.GetString(LocalizationKey.User.NameTemplate))
            .OverridePropertyName(nameof(UserCardDto.Name));
        
        RuleFor(u => u.User.Email)
            .EmailAddress()
            .OverridePropertyName(nameof(UserCardDto.Email))
            .WithMessage(localization.GetString(LocalizationKey.User.Email));
        
        RuleFor(u => u.User.PhotoId)
            .MustAsync(IsFileExists)
            .OverridePropertyName(nameof(UserCardDto.PhotoId))
            .WithMessage(localization.GetString(LocalizationKey.File.FileNotFound));
        
        RuleFor(u => u.User.MattermostName)
            .Matches(@"^\w+\.\w+")
            .OverridePropertyName(nameof(UserCardDto.MattermostName))
            .WithMessage(localization.GetString(LocalizationKey.User.MattermostNameTemplate));
    }

    private async Task<bool> IsUserExists(Guid id, CancellationToken cancellationToken) => await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(id), cancellationToken) != null;

    private async Task<bool> IsFileExists(Guid? id, CancellationToken cancellationToken) => id.HasValue && await _grandmaRepository.GetAsync(new NotDeleted<FileModel, Guid>(id.Value), cancellationToken) != null;
}