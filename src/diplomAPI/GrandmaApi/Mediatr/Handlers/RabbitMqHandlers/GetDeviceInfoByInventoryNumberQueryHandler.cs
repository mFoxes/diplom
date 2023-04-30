using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Domain.Models;
using GrandmaApi.Mediatr.Queries;
using MediatR;

namespace GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;

public class GetDeviceInfoByInventoryNumberQueryHandler : IRequestHandler<GetDeviceInfoByInventoryNumberQuery, BrokerCommandResponse<DeviceInfoDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<GetDeviceInfoByInventoryNumberQueryHandler> _logger;

    public GetDeviceInfoByInventoryNumberQueryHandler(IGrandmaRepository grandmaRepository, ILogger<GetDeviceInfoByInventoryNumberQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
    }

    public async Task<BrokerCommandResponse<DeviceInfoDto>> Handle(GetDeviceInfoByInventoryNumberQuery request, CancellationToken cancellationToken)
    {
        var number = request.InventoryNumberMessage.InventoryNumber;
        _logger.LogDebug($"Requested device info with inventory number: {number}");
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>().Combine(new InventoryNumberSpecification(number)));
        var booking =
            await _grandmaRepository.GetAsync(
                new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(device.Id)));
        
        _logger.LogDebug($"Found name: {device.Name}");
        return new BrokerCommandResponse<DeviceInfoDto>(new DeviceInfoDto()
        {
            Id = device.Id,
            Name = device.Name,
            State = booking.State
        });
    }
}