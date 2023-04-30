using System.Net;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Extensions;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using MediatR;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetDeviceHistoryQueryHandler : IRequestHandler<GetDeviceHistory, HttpCommandResponse<HistoryListDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDeviceHistoryQueryHandler> _logger;

    public GetDeviceHistoryQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetDeviceHistoryQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task<HttpCommandResponse<HistoryListDto>> Handle(GetDeviceHistory request, CancellationToken cancellationToken)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(request.Id));
        
        if (device is null)
        {
            _logger.LogWarning($"Device with id {request.Id} not dound");
            return new HttpCommandResponse<HistoryListDto>()
            {
                StatusCode = HttpStatusCode.NotFound
            };
        }
        var history =
            await _grandmaRepository.ListAsync(new NotDeleted<BookingHistory, Guid>().Combine(new BookingHistoryByDeviceId(request.Id)));
        
        _logger.LogDebug($"Found {history.Count()} taking device with id {request.Id}");
        
        var historyDtos = _mapper.Map<IEnumerable<BookingHistory>, IEnumerable<HistoryDto>>(history);
        var result =  historyDtos.OrderByDescending(h => h.TakeAt).ToList();
        
        return new HttpCommandResponse<HistoryListDto>(new HistoryListDto(device.Name, device.InventoryNumber, result));
    }
}