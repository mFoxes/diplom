using System.Text.Json;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using Domain.Enums;
using MediatR;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, HttpCommandResponse<Guid>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateDeviceCommandHandler> _logger;

    public CreateDeviceCommandHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<CreateDeviceCommandHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<Guid>> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<DeviceDto, DeviceModel>(request.Device);
        
        entity.Id = Guid.NewGuid();
        var booking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = entity.Id,
            State = DeviceStates.Free
        };
        
        await _grandmaRepository.SaveAsync(booking);
        await _grandmaRepository.SaveAsync(entity);
        _logger.LogDebug($"Created device:{JsonSerializer.Serialize(entity)}");
        _logger.LogDebug($"Created booking with id: {booking.Id}");
        return new HttpCommandResponse<Guid>(entity.Id);
    }
}