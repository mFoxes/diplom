using System.Text.Json;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using Domain.Enums;
using GrandmaApi.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;
using GrandmaApi.Database.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, HttpCommandResponse<Unit>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<GrandmaHub, IGrandmaHub> _hubContext;
    private readonly ILogger<UpdateBookingCommandHandler> _logger;
    
    public UpdateBookingCommandHandler(IMapper mapper, IHubContext<GrandmaHub, IGrandmaHub> hubContext, IGrandmaRepository grandmaRepository, ILogger<UpdateBookingCommandHandler> logger) =>
                                                        (_mapper, _hubContext, _grandmaRepository, _logger) = (mapper, hubContext, grandmaRepository, logger);

    public async Task<HttpCommandResponse<Unit>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var updateBooking = request.BookingDto;
        
        var booking = await _grandmaRepository.GetAsync( new NotDeleted<BookingModel, Guid>(updateBooking.Id));
        var bookingState = booking.State;
        booking = _mapper.Map(updateBooking, booking);

        if(bookingState == DeviceStates.Free && updateBooking.State == DeviceStates.Booked)
        {
            var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(updateBooking.UserId.Value));

            var history = new BookingHistory()
            {
                Id = Guid.NewGuid(),
                DeviceId = booking.DeviceId,
                TakedBy = user.CommonName,
                TakeAt = updateBooking.TakeAt.ToUniversalTime().Date
            };
            booking.TakeAt = updateBooking.TakeAt.ToUniversalTime().Date;
            booking.ReturnAt = updateBooking.ReturnAt.ToUniversalTime().Date;

            await _grandmaRepository.SaveAsync(history);
        }
        else if (bookingState == DeviceStates.Booked && updateBooking.State == DeviceStates.Free)
        {
            booking.UserId = null;
            booking.TakeAt = null;
            booking.ReturnAt = null;

            var history = await _grandmaRepository.GetAsync(new NotDeleted<BookingHistory, Guid>().Combine(new ActiveBookingHistoryByDeviceId(booking.DeviceId)));
            history.ReturnedAt = DateTime.UtcNow.Date;
            await _grandmaRepository.SaveAsync(history);
        }
        else if(bookingState == DeviceStates.Booked && updateBooking.State == DeviceStates.Booked)
        {
            var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(updateBooking.UserId.Value));
            var history = await _grandmaRepository.GetAsync(new NotDeleted<BookingHistory, Guid>().Combine(new ActiveBookingHistoryByDeviceId(booking.DeviceId)));
            history.TakeAt = updateBooking.TakeAt.ToUniversalTime().Date;
            history.TakedBy = user.CommonName;
            await _grandmaRepository.SaveAsync(history);
        }
        
        await _grandmaRepository.SaveAsync(booking);
        
        await NotifyStateChanged(booking);
        _logger.LogDebug($"Booking updated: {JsonSerializer.Serialize(booking)}");
        return new HttpCommandResponse<Unit>(Unit.Value);
    }

    private async Task NotifyStateChanged(BookingModel booking)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(booking.DeviceId));
        var bookingDto = _mapper.Map<BookingModel, BookingDto>(booking);
        bookingDto.DeviceId = device.Id;
        bookingDto.Name = device.Name;
        bookingDto.InventoryNumber = device.InventoryNumber;
        if(booking.State == DeviceStates.Booked)
        {
            var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(booking.UserId.Value));
            bookingDto.TakedBy = user?.CommonName;
        }
        await _hubContext.Clients.All.Notify(bookingDto);
    }
}