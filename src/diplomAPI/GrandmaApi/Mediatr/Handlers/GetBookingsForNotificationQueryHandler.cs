using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using MediatR;

namespace GrandmaApi.Mediatr.Handlers;

public class GetBookingsForNotificationQueryHandler : IRequestHandler<GetBookingsForNotificationQuery, IEnumerable<BookingsNotificationDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<GetBookingsForNotificationQueryHandler> _logger;
    
    public GetBookingsForNotificationQueryHandler(IGrandmaRepository grandmaRepository, ILogger<GetBookingsForNotificationQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingsNotificationDto>> Handle(GetBookingsForNotificationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Requested bookings for notifications");
        var bookings = await _grandmaRepository.ListAsync(new NotDeleted<BookingModel, Guid>());
        var users = await _grandmaRepository.ListAsync(new NotDeleted<MongoUser, Guid>());
        var devices = await _grandmaRepository.ListAsync(new NotDeleted<DeviceModel, Guid>());

        var result = bookings.Join(devices, bd => bd.DeviceId, d => d.Id, (bookedModel, deviceModel) => new
        {
            bookedModel.ReturnAt,
            deviceModel.Name,
            bookedModel.UserId,
            bookedModel.TakeAt,
            deviceModel.InventoryNumber
        }).Join(users, bd => bd.UserId, u => u.Id, (bookedModel, userModel) => new
        {
            userModel.Email,
            userModel.CommonName,
            userModel.MattermostName,
            bookedModel.Name,
            bookedModel.ReturnAt,
            bookedModel.TakeAt,
            bookedModel.InventoryNumber
        }).GroupBy(res => new
        {
            res.Email,
            res.CommonName,
            res.MattermostName
        }).Select(res => new BookingsNotificationDto()
        {
            Email = res.Key.Email,
            Name = res.Key.CommonName,
            MattermostName = res.Key.MattermostName,
            Devices = res.Select(d => new BookingInfoDto()
            {
                Name = d.Name,
                ReturnAt = d.ReturnAt.Value.ToLocalTime(),
                TakeAt = d.TakeAt.Value.ToLocalTime(),
                InventoryNumber = d.InventoryNumber
            })
        });
        _logger.LogDebug($"Found {result.Count()} active bookings");
        return result;
    }
}