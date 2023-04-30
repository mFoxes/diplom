using System.Text.Json;
using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;
using AutoMapper;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Enums;
using GrandmaApi.Extensions;
using MediatR;
using DTO;
using Domain.Models;
using GrandmaApi.Database.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetDevicesQueryHandler : IRequestHandler<GetDevicesQuery, HttpCommandResponse<TableDto<DeviceDto>>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDevicesQueryHandler> _logger;

    public GetDevicesQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetDevicesQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<TableDto<DeviceDto>>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
    {
        ISpecification<DeviceModel> deviceSpecification = new NotDeleted<DeviceModel, Guid>();
        _logger.LogDebug($"Requested list of devices with parameters: {JsonSerializer.Serialize(request)}");
        
        var count = await _grandmaRepository.CountAsync(deviceSpecification);

        if (request.FilterName != null)
        {
           deviceSpecification = deviceSpecification.Combine(new FilterDeviceByName(request.FilterName));
        }

        if (request.FilterInventoryNumber != null)
        {
            deviceSpecification = deviceSpecification.Combine(new FilterDeviceByInventoryNumber(request.FilterInventoryNumber));
        }
        IEnumerable<DeviceModel> filteredDevices = await _grandmaRepository.ListAsync<DeviceModel>(deviceSpecification);
        
        filteredDevices = request.OrderBy == DeviceOrderBy.Name
            ? filteredDevices.OrderBy(d => d.Name, request.OrderDirection)
            : filteredDevices.OrderBy(d => d.InventoryNumber, request.OrderDirection);

        var filteredCount = filteredDevices.Count();
        filteredDevices = filteredDevices.Skip(request.Skip).Take(request.Take);
        
        var entities = _mapper.Map<IEnumerable<DeviceModel>, IEnumerable<DeviceDto>>(filteredDevices);
        _logger.LogDebug($"Total items count: {count}, total items filtered: {filteredCount}");
        return new HttpCommandResponse<TableDto<DeviceDto>>(new TableDto<DeviceDto>()
            {
                Items = entities,
                TotalItems = count,
                TotalItemsFiltered = filteredCount
            });
    }
}