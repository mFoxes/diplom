using System;
using System.Threading;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiTests.HandlerTests.CommandHandlers;

public class UpdateDeviceCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<UpdateDeviceCommandHandler>> _loggerMock = new(); 
    
    [Fact]
    public void UpdateDevice()
    {
        var deviceId = Guid.NewGuid();
        var photoId = Guid.NewGuid();

        var deviceDto = new DeviceDto()
        {
            Id = deviceId,
            Name = "New Name",
            InventoryNumber = "d.00.000",
            PhotoId = photoId
        };

        var dbEntry = new DeviceModel()
        {
            Id = deviceId,
            Name = "Old Name",
            InventoryNumber = "d.00.001",
            PhotoId = photoId
        };

        _mapperMock.Setup(mapper => mapper.Map<DeviceDto, DeviceModel>(deviceDto))
            .Returns(dbEntry);

        var handler = new UpdateDeviceCommandHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        handler.Handle(new UpdateDeviceCommand(deviceDto), CancellationToken.None);
        
        _repoMock.Verify(repo => repo.SaveAsync(dbEntry, CancellationToken.None));
        _mapperMock.Verify(mapper => mapper.Map<DeviceDto, DeviceModel>(deviceDto));

    }
    
}