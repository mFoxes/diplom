using System.Net;
using System.Text.Json;
using AutoMapper;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, HttpCommandResponse<Unit>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateDeviceCommandHandler> _logger;

    public UpdateDeviceCommandHandler(IGrandmaRepository grandmaRepository, IMapper mapper,
        ILogger<UpdateDeviceCommandHandler> logger) =>
        (_grandmaRepository, _mapper, _logger) = (grandmaRepository, mapper, logger);

    public async Task<HttpCommandResponse<Unit>> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<DeviceDto, DeviceModel>(request.Device);
        await _grandmaRepository.SaveAsync(entity);
        _logger.LogDebug($"Device updated: {JsonSerializer.Serialize(entity)}");
        return new HttpCommandResponse<Unit>(Unit.Value);
    }
}