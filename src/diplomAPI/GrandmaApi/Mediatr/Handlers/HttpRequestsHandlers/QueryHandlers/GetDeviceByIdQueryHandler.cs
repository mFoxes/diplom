using Singularis.Internal.Domain.Specifications;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using MediatR;
using System.Text.Json;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, HttpCommandResponse<DeviceDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDeviceByIdQueryHandler> _logger;

    public GetDeviceByIdQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetDeviceByIdQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<DeviceDto>> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(request.DeviceId));
        var entity = _mapper.Map<DeviceModel, DeviceDto>(device);
        _logger.LogDebug($"Requested device by id {request.DeviceId}. Found: {JsonSerializer.Serialize(entity)}");
        return new HttpCommandResponse<DeviceDto>(entity);
    }
}