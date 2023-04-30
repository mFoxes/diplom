using System;
using System.Threading;
using ApiTests.Helpers;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.CommandHandlers;

public class DeleteDeviceCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new ();
    private readonly Mock<ILogger<DeleteDeviceCommandHandler>> _loggerMock = new(); 
    
    [Fact]
    public void DeleteDevice()
    {
        var deviceId = Guid.NewGuid();
        var command = new DeleteDeviceCommand(deviceId);
        var device = new DeviceModel()
        {
            Id = deviceId,
            InventoryNumber = "d.00.000",
            IsDeleted = false,
            Name = "Name",
            PhotoId = Guid.NewGuid()
        };
        var booking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            IsDeleted = false
        };
        
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(device);
        _repoMock.Setup(repo =>
                repo.GetAsync(
                    It.Is<CombinedSpecification<BookingModel>>(o =>
                        o.Validate(s => s.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                            s => s.CheckSpecification<BookingByDeviceId>(d => d.DeviceId == deviceId))),
                    CancellationToken.None).Result)
            .Returns(booking);

        var handler = new DeleteDeviceCommandHandler(_repoMock.Object, _loggerMock.Object);
        handler.Handle(command, CancellationToken.None);

        Assert.True(device.IsDeleted);
        Assert.True(booking.IsDeleted);
        
        _repoMock.Verify(repo => repo.SaveAsync(device, CancellationToken.None));
        _repoMock.Verify(repo => repo.SaveAsync(booking, CancellationToken.None));

    }
}