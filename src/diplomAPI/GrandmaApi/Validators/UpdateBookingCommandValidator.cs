using DTO;
using FluentValidation;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Localization;
using Domain.Enums;

namespace GrandmaApi.Validators;

public class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
{
    private readonly IGrandmaRepository _grandmaRepository;

    public UpdateBookingCommandValidator(IGrandmaRepository grandmaRepository, ILocalizationService localization)
    {
        _grandmaRepository = grandmaRepository;

        RuleFor(bd => bd.BookingDto.Id)
            .MustAsync(IsBookingExists)
            .WithMessage(localization.GetString(LocalizationKey.Booking.NotFound))
            .OverridePropertyName(nameof(UpdateBookingDto.Id));
        
        When(IsBooked, () =>
        {
            RuleFor(bd => bd.BookingDto)
                .Must(td => !td.Equals(default))
                .WithMessage(localization.GetString(LocalizationKey.Shared.NotEmpty))
                .Must(b => b.TakeAt < b.ReturnAt)
                .OverridePropertyName(nameof(UpdateBookingDto.TakeAt))
                .WithMessage(localization.GetString(LocalizationKey.Booking.TakeAtBeforeReturnAt));
            
            RuleFor(bd => bd.BookingDto.ReturnAt)
                .Must(rd => !rd.Equals(default))
                .WithMessage(localization.GetString(LocalizationKey.Shared.NotEmpty))
                .GreaterThan(DateTime.Now)
                .WithMessage(localization.GetString(LocalizationKey.Shared.FutureDate))
                .OverridePropertyName(nameof(UpdateBookingDto.ReturnAt));
            
            RuleFor(bd => bd.BookingDto.UserId)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(localization.GetString(LocalizationKey.User.IsNull))
                .MustAsync(IsUserExists)
                .WithMessage(localization.GetString(LocalizationKey.User.NotFound))
                .OverridePropertyName(nameof(UpdateBookingDto.UserId));
        });
    }

    private bool IsBooked(UpdateBookingCommand command) => command.BookingDto.State == DeviceStates.Booked;
    
    private async Task<bool> IsBookingExists(Guid id, CancellationToken cancellationToken) => await _grandmaRepository.GetAsync(new NotDeleted<BookingModel, Guid>(id), cancellationToken) != null;

    private async Task<bool> IsUserExists(Guid? id, CancellationToken cancellationToken) => id.HasValue && await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(id.Value)) != null;
    
}