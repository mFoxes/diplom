using AutoMapper;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;
using Domain.Models;
using GrandmaApi.Mediatr.Commands;
using MediatR;
using Domain.Enums;
using DTO;
using GrandmaApi.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;

public class ReturnDeviceCommandHandler : IRequestHandler<ReturnDeviceCommand, BrokerCommandResponse<Unit>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<ReturnDeviceCommandHandler> _logger;
    private readonly IHubContext<GrandmaHub, IGrandmaHub> _hubContext;
    private readonly IMapper _mapper;
    
    public ReturnDeviceCommandHandler(IGrandmaRepository grandmaRepository, ILogger<ReturnDeviceCommandHandler> logger, IHubContext<GrandmaHub, IGrandmaHub> hubContext, IMapper mapper)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task<BrokerCommandResponse<Unit>> Handle(ReturnDeviceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Device with inventory number {request.InventoryNumberMessage.InventoryNumber} returned");
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(request.InventoryNumberMessage.InventoryNumber)));
        var booking = await _grandmaRepository.GetAsync(new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(device.Id)));
        var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(booking.UserId.Value));

        var history = await _grandmaRepository.GetAsync(new NotDeleted<BookingHistory, Guid>().Combine(new ActiveBookingHistoryByDeviceId(device.Id)));
        history.ReturnedAt = DateTime.UtcNow.Date;
        
        booking.State = DeviceStates.Free;
        booking.UserId = null;
        booking.TakeAt = null;
        booking.ReturnAt = null;
        await _grandmaRepository.SaveAsync(booking);
        await _grandmaRepository.SaveAsync(history);
        await NotifyStateChanged(booking);
        return new BrokerCommandResponse<Unit>(Unit.Value);
    }

    private async Task NotifyStateChanged(BookingModel booking)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(booking.DeviceId));
        var bookingDto = _mapper.Map<BookingModel, BookingDto>(booking);
        bookingDto.Name = device.Name;
        bookingDto.DeviceId = device.Id;
        bookingDto.InventoryNumber = device.InventoryNumber;
        await _hubContext.Clients.All.Notify(bookingDto);
    }
}