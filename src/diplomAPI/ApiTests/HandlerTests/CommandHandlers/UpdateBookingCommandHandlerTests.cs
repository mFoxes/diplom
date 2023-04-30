using System;
using System.Linq;
using System.Threading;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using Domain.Models;
using GrandmaApi.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;
using Domain.Enums;
using Singularis.Internal.Domain.Specification;
using ApiTests.Helpers;
using GrandmaApi.Database.Specifications;

namespace ApiTests.HandlerTests.CommandHandlers;

public class UpdateBookingCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<UpdateBookingCommandHandler>> _loggerMock = new(); 
    private readonly Mock<IHubContext<GrandmaHub, IGrandmaHub>> _hubMock = new();

    [Fact]
    public void DeviceBecomeFree()
    {
        var deviceId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new MongoUser()
        {
            Id = userId,
            CommonName = "Test"
        };
        var deviceDbEntry = new DeviceModel()
        {
            Id = deviceId,
            Name = "Name",
            InventoryNumber = "d.00.000",
        };
        
        var bookingDbEntry = new BookingModel()
        {
            Id = bookingId,
            DeviceId = deviceId,
            State = DeviceStates.Booked,
            UserId = userId,
            TakeAt = DateTime.Now,
            ReturnAt = DateTime.Now
        };
        
        var bookingToUpdate = new UpdateBookingDto()
        {
            Id = bookingId,
            State = DeviceStates.Free,
            UserId = userId,
            ReturnAt = DateTime.Now,
            TakeAt = DateTime.Now
        };

        var updatedBooking = new BookingModel()
        {
            Id = bookingId,
            DeviceId = deviceId,
            State = DeviceStates.Free,
            UserId = userId,
            ReturnAt = DateTime.UtcNow.Date,
            TakeAt = DateTime.UtcNow.Date
        };
        var updatedMappedBooking = new BookingDto()
        {
            Id = bookingId,
            State = DeviceStates.Free
        };
        var history = new BookingHistory()
        {
            Id = Guid.NewGuid(),
            DeviceId = bookingDbEntry.DeviceId,
            TakedBy = user.CommonName,
            TakeAt = bookingDbEntry.TakeAt.Value
        };
        _mapperMock.Setup(mapper => mapper.Map<UpdateBookingDto, BookingModel>(bookingToUpdate, bookingDbEntry)).Returns(updatedBooking);
        _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(updatedBooking)).Returns(updatedMappedBooking);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None)
                .Result).Returns(bookingDbEntry);

        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None)
                .Result).Returns(deviceDbEntry);
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(user);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingHistory>>(
                o => o.Validate(v => v.CheckSpecification<NotDeleted<BookingHistory, Guid>>(), 
                                v => v.CheckSpecification<ActiveBookingHistoryByDeviceId>(s => s.DeviceId == bookingDbEntry.DeviceId)))
                            , CancellationToken.None).Result)
                        .Returns(history);    

        _hubMock.Setup(hub => hub.Clients.All.Notify(It.IsAny<BookingDto>()));
        
        var handler = new UpdateBookingCommandHandler(_mapperMock.Object, _hubMock.Object, _repoMock.Object, _loggerMock.Object);
        var command = new UpdateBookingCommand(bookingToUpdate);
        handler.Handle(command, CancellationToken.None);
        
        Assert.Null(updatedBooking.UserId);
        Assert.Null(updatedBooking.ReturnAt);
        Assert.Null(updatedBooking.TakeAt);
        Assert.Equal(DeviceStates.Free, updatedBooking.State);
        Assert.NotNull(history.ReturnedAt);
        
        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<BookingHistory>(), CancellationToken.None));
        _repoMock.Verify(repo => repo.SaveAsync(updatedBooking, CancellationToken.None));

    }
    [Fact]
    public void UpdatedBook()
    {
        var deviceId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var booking = new BookingModel()
        {
            Id = bookingId,
            DeviceId = deviceId,
            UserId = userId,
            State = DeviceStates.Booked,
            ReturnAt = DateTime.UtcNow.Date,
            TakeAt = DateTime.UtcNow.Date
        };
        var updateBookingDto = new UpdateBookingDto()
        {
            Id = bookingId,
            UserId = userId,
            TakeAt = DateTime.UtcNow.Date.AddDays(-1),
            ReturnAt = DateTime.UtcNow.Date.AddDays(3),
            State = DeviceStates.Booked
        };
        var device = new DeviceModel()
        {
            Id = deviceId,
            InventoryNumber = "d.00.000",
            Name = "Name"
        };

        var user = new MongoUser()
        {
            Id = userId,
            CommonName = "User"
        };
        var history = new BookingHistory()
        {
            Id = Guid.NewGuid(),
            DeviceId = device.Id,
            TakedBy = user.CommonName,
            TakeAt = updateBookingDto.TakeAt
        };
        var bookingDto = new BookingDto()
        {
            Id = bookingId,
            ReturnAt = updateBookingDto.ReturnAt,
            TakeAt = updateBookingDto.TakeAt,
            State = DeviceStates.Booked,
            UserId = userId
        };
        _mapperMock.Setup(mapper => mapper.Map<UpdateBookingDto, BookingModel>(It.IsAny<UpdateBookingDto>(), It.IsAny<BookingModel>())).Returns(booking);
        _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>())).Returns(bookingDto);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None)
                .Result).Returns(booking);

        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None)
                .Result).Returns(device);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None)
                .Result).Returns(user);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingHistory>>(
                o => o.Validate(v => v.CheckSpecification<NotDeleted<BookingHistory, Guid>>(), 
                                v => v.CheckSpecification<ActiveBookingHistoryByDeviceId>(s => s.DeviceId == booking.DeviceId)))
                            , CancellationToken.None).Result)
                        .Returns(history); 
        
        var handler = new UpdateBookingCommandHandler(_mapperMock.Object, _hubMock.Object, _repoMock.Object, _loggerMock.Object);
        var command = new UpdateBookingCommand(updateBookingDto);
        handler.Handle(command, CancellationToken.None);
        
        _repoMock.Verify(repo => repo.SaveAsync(booking, CancellationToken.None));
        _repoMock.Verify(repo => repo.SaveAsync(history, CancellationToken.None));
        Assert.Equal(device.InventoryNumber, bookingDto.InventoryNumber);
        Assert.Equal(user.CommonName, bookingDto.TakedBy);
        Assert.Equal(updateBookingDto.TakeAt, history.TakeAt);
    }

    [Fact]
    public void DeviceBecomeBooked()
    {
        var deviceId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var booking = new BookingModel()
        {
            Id = bookingId,
            DeviceId = deviceId,
            State = DeviceStates.Free
        };
        var updateBookingDto = new UpdateBookingDto()
        {
            Id = bookingId,
            UserId = userId,
            TakeAt = DateTime.UtcNow.Date.AddDays(-1),
            ReturnAt = DateTime.UtcNow.Date.AddDays(3),
            State = DeviceStates.Booked
        };
        var device = new DeviceModel()
        {
            Id = deviceId,
            InventoryNumber = "d.00.000",
            Name = "Name"
        };

        var user = new MongoUser()
        {
            Id = userId,
            CommonName = "User"
        };
        var bookingDto = new BookingDto()
        {
            Id = bookingId,
            ReturnAt = updateBookingDto.ReturnAt,
            TakeAt = updateBookingDto.TakeAt,
            State = DeviceStates.Booked,
            UserId = userId
        };
        _mapperMock.Setup(mapper => mapper.Map<UpdateBookingDto, BookingModel>(It.IsAny<UpdateBookingDto>(), It.IsAny<BookingModel>()))
        .Returns<UpdateBookingDto, BookingModel>((u, b) =>
                {
                    b.UserId = u.UserId;
                    b.State = u.State;
                    b.TakeAt = u.TakeAt;
                    b.ReturnAt = u.ReturnAt;
                    return b;
                });
        _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>())).Returns(bookingDto);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None)
                .Result).Returns(booking);

        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None)
                .Result).Returns(device);
        
        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None)
                .Result).Returns(user);
        
        var handler = new UpdateBookingCommandHandler(_mapperMock.Object, _hubMock.Object, _repoMock.Object, _loggerMock.Object);
        var command = new UpdateBookingCommand(updateBookingDto);
        handler.Handle(command, CancellationToken.None);
        
        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<BookingHistory>(), CancellationToken.None));
        _repoMock.Verify(repo => repo.SaveAsync(booking, CancellationToken.None));

        Assert.Equal(device.InventoryNumber, bookingDto.InventoryNumber);
        Assert.Equal(user.CommonName, bookingDto.TakedBy);
        Assert.NotNull(booking.TakeAt);
        Assert.NotNull(booking.ReturnAt);
        Assert.NotNull(booking.UserId);
        Assert.Equal(DeviceStates.Booked, booking.State);
    }
}