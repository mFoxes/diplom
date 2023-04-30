using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using Domain.Models;
using GrandmaApi.Mediatr.Commands;
using MediatR;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class DeleteDeviceCommandHandler: IRequestHandler<DeleteDeviceCommand, HttpCommandResponse<Unit>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<DeleteDeviceCommandHandler> _logger;

    public DeleteDeviceCommandHandler(IGrandmaRepository grandmaRepository, ILogger<DeleteDeviceCommandHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<Unit>> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(request.Id));
        var booking = await _grandmaRepository.GetAsync(new NotDeleted<BookingModel, Guid>().Combine(new BookingByDeviceId(request.Id)));
        device.IsDeleted = true;
        booking.IsDeleted = true;
        await _grandmaRepository.SaveAsync(booking);
        await _grandmaRepository.SaveAsync(device);
        _logger.LogDebug($"Device with id {device.Id} soft deleted");
        _logger.LogDebug($"Booking with id {booking.Id} soft deleted");
        return new HttpCommandResponse<Unit>(Unit.Value);
    }
}