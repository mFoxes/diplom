using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using Domain.Enums;
using GrandmaApi.Database.Specifications;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using MediatR;
using System.Text.Json;
using AutoMapper;
using DTO;
using GrandmaApi.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;

public class BookDeviceCommandHandler : IRequestHandler<BookDeviceCommand, BrokerCommandResponse<UserHasOverdueBookingDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<BookDeviceCommandHandler> _logger;
    private readonly IHubContext<GrandmaHub, IGrandmaHub> _hubContext;
    private readonly IMapper _mapper;

    public BookDeviceCommandHandler(IGrandmaRepository grandmaRepository, ILogger<BookDeviceCommandHandler> logger, IHubContext<GrandmaHub, IGrandmaHub> hubContext, IMapper mapper)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task<BrokerCommandResponse<UserHasOverdueBookingDto>> Handle(BookDeviceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Device with id { request.DeviceTook.DeviceId } booked");
        var booking = await _grandmaRepository.GetAsync(new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(request.DeviceTook.DeviceId)));
        var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(request.DeviceTook.UserId));

        var history = new BookingHistory()
        {
            Id = Guid.NewGuid(),
            DeviceId = booking.DeviceId,
            TakeAt = DateTime.UtcNow.Date,
            TakedBy = user.CommonName
        };

        booking.UserId = request.DeviceTook.UserId;
        booking.TakeAt = DateTime.UtcNow.Date;
        booking.ReturnAt = request.DeviceTook.ReturnAt.ToUniversalTime().Date;
        booking.State = DeviceStates.Booked;
        
        await _grandmaRepository.SaveAsync(booking);
        await _grandmaRepository.SaveAsync(history);
        
        _logger.LogDebug($"Updated booking: {JsonSerializer.Serialize(booking)}");

        var overdueBookings =
            await _grandmaRepository.ListAsync(
                new NotDeleted<BookingModel, Guid>().Combine(new OverdueBookingsByUserId(booking.UserId.Value)));
        var result = new UserHasOverdueBookingDto
        {
            HasOverdueBookings = overdueBookings.Any()
        };
        await NotifyStateChanged(booking);
        return new BrokerCommandResponse<UserHasOverdueBookingDto>(result);
    }
    private async Task NotifyStateChanged(BookingModel booking)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(booking.DeviceId));
        var bookingDto = _mapper.Map<BookingModel, BookingDto>(booking);
        bookingDto.DeviceId = device.Id;
        bookingDto.Name = device.Name;
        bookingDto.InventoryNumber = device.InventoryNumber;
        var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(booking.UserId.Value)); 
        bookingDto.TakedBy = user?.CommonName;
        await _hubContext.Clients.All.Notify(bookingDto);
    }
}