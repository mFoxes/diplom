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

public class CreateDeviceCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<CreateDeviceCommandHandler>> _loggerMock = new(); 
    
    [Fact]
    public void CreateDevice()
    {
        var deviceDto = new DeviceDto()
        {
            InventoryNumber = "d.00.000",
            Name = "Name",
            PhotoId = Guid.NewGuid()
        };

        _mapperMock.Setup(mapper => mapper.Map<DeviceDto, DeviceModel>(deviceDto)).Returns(new DeviceModel()
        {
            InventoryNumber = deviceDto.InventoryNumber,
            Name = deviceDto.Name,
            PhotoId = deviceDto.PhotoId,
            IsDeleted = false
        });

        var handler = new CreateDeviceCommandHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
        handler.Handle(new CreateDeviceCommand(deviceDto), CancellationToken.None);
        
        _mapperMock.Verify(mapper => mapper.Map<DeviceDto, DeviceModel>(deviceDto));
        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<DeviceModel>(), CancellationToken.None));
        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<BookingModel>(), CancellationToken.None));
    }
}