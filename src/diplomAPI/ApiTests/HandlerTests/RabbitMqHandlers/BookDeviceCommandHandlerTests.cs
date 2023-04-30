using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ApiTests.Helpers;
using AutoMapper;
using Domain.Enums;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;
using Domain.Models;
using DTO;
using GrandmaApi.Models.MessageModels;
using GrandmaApi.SignalR;
using LdapConnector;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.RabbitMqHandlers;

public class BookDeviceCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<ILogger<BookDeviceCommandHandler>> _loggerMock = new(); 
    private readonly Mock<IHubContext<GrandmaHub, IGrandmaHub>> _hubMock = new();
    private readonly Mock<IMapper> _mapperMock = new();

    public BookDeviceCommandHandlerTests()
    {
        var mockClients = new Mock<IHubClients<IGrandmaHub>>();
        var mockAllClients = new Mock<IGrandmaHub>();
        _hubMock.Setup(hub => hub.Clients).Returns(mockClients.Object);
        _hubMock.Setup(hub => hub.Clients.All).Returns(mockAllClients.Object);
    }
    [Fact]
    public void DeviceBooked()
    {
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dbBooking = new BookingModel()
        {
            DeviceId = deviceId,
            ReturnAt = null,
            TakeAt = null,
            State = DeviceStates.Free,
            UserId = null
        };
        var bookingDto = new BookingDto()
        {
          Id  = dbBooking.Id,
          ReturnAt = dbBooking.ReturnAt,
          TakeAt = dbBooking.TakeAt,
          UserId = dbBooking.UserId
        };
        var device = new DeviceModel()
        {
            Id = deviceId,
            Name = "Test device",
            InventoryNumber = "d.00.000"
        };
        var user = new MongoUser
        {
            Id = userId,
            CommonName = "Test user"
        };

        var deviceTookMessage = new DeviceTookMessage()
        {
            DeviceId = deviceId,
            UserId = userId,
            ReturnAt = DateTime.UtcNow.Date.AddDays(1)
        };

        _repoMock.Setup(repo =>
            repo.GetAsync(
                It.Is<CombinedSpecification<BookingModel>>(o =>
                    o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                        v => v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))),
                CancellationToken.None).Result).Returns(dbBooking);
        _repoMock.Setup(repo =>
            repo.ListAsync(
                It.Is<CombinedSpecification<BookingModel>>(o =>
                    o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                        v => v.CheckSpecification<BookingByUserId>(s => s.UserIds.Contains(userId)))),
                CancellationToken.None).Result).Returns(new List<BookingModel>());
        _repoMock.Setup(repo =>
            repo.GetAsync(
                It.IsAny<NotDeleted<DeviceModel, Guid>>(),
                CancellationToken.None).Result).Returns(device);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(
                It.IsAny<NotDeleted<MongoUser, Guid>>(),
                CancellationToken.None).Result).Returns(user);

        _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(dbBooking))
            .Returns(bookingDto);

        
        var handler = new BookDeviceCommandHandler(_repoMock.Object, _loggerMock.Object, _hubMock.Object, _mapperMock.Object);
        handler.Handle(new BookDeviceCommand(deviceTookMessage), CancellationToken.None);

        _repoMock.Verify(repo => repo.SaveAsync(dbBooking, CancellationToken.None));
        _repoMock.Verify(repo => repo.GetAsync( It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None));
        _repoMock.Verify(repo => repo.GetAsync( It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None));
        
        _mapperMock.Verify(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>()));
        
        
        Assert.Equal(deviceTookMessage.UserId, dbBooking.UserId);
        Assert.Equal(deviceTookMessage.ReturnAt.ToUniversalTime(), dbBooking.ReturnAt);
        Assert.Equal(DeviceStates.Booked, dbBooking.State);
    }
    
    [Fact]
    public void UserHasOverdueBookings()
    {
        var deviceId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var overdueBooking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = Guid.NewGuid(),
            UserId = userId,
            State = DeviceStates.Booked,
            TakeAt = DateTime.UtcNow.AddDays(-10),
            ReturnAt = DateTime.UtcNow.AddDays(-3)
        };

        var dbBooking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = deviceId,
            ReturnAt = null,
            TakeAt = null,
            State = DeviceStates.Free,
            UserId = null
        };
        var bookingDto = new BookingDto()
        {
          Id  = dbBooking.Id,
          ReturnAt = dbBooking.ReturnAt,
          TakeAt = dbBooking.TakeAt,
          UserId = dbBooking.UserId
        };
        var device = new DeviceModel()
        {
            Id = deviceId,
            Name = "Test device",
            InventoryNumber = "d.00.000"
        };
        var user = new MongoUser
        {
            Id = userId,
            CommonName = "Test user"
        };

        var deviceTookMessage = new DeviceTookMessage()
        {
            DeviceId = deviceId,
            UserId = userId,
            ReturnAt = DateTime.UtcNow.Date.AddDays(1)
        };

        _repoMock.Setup(repo =>
            repo.GetAsync(
                It.Is<CombinedSpecification<BookingModel>>(o =>
                    o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                        v => v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))),
                CancellationToken.None).Result).Returns(dbBooking);

        _repoMock.Setup(repo =>
            repo.ListAsync(
                It.Is<CombinedSpecification<BookingModel>>(o =>
                    o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                        v => v.CheckSpecification<BookingByUserId>(s => s.UserIds.Contains(userId)))),
                CancellationToken.None).Result).Returns(new List<BookingModel> {overdueBooking});
        
        _repoMock.Setup(repo =>
            repo.GetAsync(
                It.IsAny<NotDeleted<DeviceModel, Guid>>(),
                CancellationToken.None).Result).Returns(device);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(
                It.IsAny<NotDeleted<MongoUser, Guid>>(),
                CancellationToken.None).Result).Returns(user);

        _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(dbBooking))
            .Returns(bookingDto);


        var handler = new BookDeviceCommandHandler(_repoMock.Object, _loggerMock.Object, _hubMock.Object , _mapperMock.Object);
        var result = handler.Handle(new BookDeviceCommand(deviceTookMessage), CancellationToken.None).Result;

        _repoMock.Verify(repo => repo.SaveAsync(dbBooking, CancellationToken.None));
        _repoMock.Verify(repo => repo.GetAsync( It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None));
        _repoMock.Verify(repo => repo.GetAsync( It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None));
        
        _mapperMock.Verify(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>()));
        
        
        Assert.Equal(deviceTookMessage.UserId, dbBooking.UserId);
        Assert.Equal(deviceTookMessage.ReturnAt.ToUniversalTime(), dbBooking.ReturnAt);
        Assert.Equal(DeviceStates.Booked, dbBooking.State);
        Assert.True(result.Result.HasOverdueBookings);
    }
}