using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Handlers;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;
using Domain.Enums;

namespace ApiTests.HandlerTests;

public class GetBookingsForNotificationQueryHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<ILogger<GetBookingsForNotificationQueryHandler>> _loggerMock = new(); 
    
    [Fact]
    public void GetBookings()
    {
        var user1 = new MongoUser
        {
            Id = Guid.NewGuid(),
            CommonName = "User 1",
            Email = "user1@example.com",
            MattermostName = "first.user"
        };
        
        var user2 = new MongoUser
        {
            Id = Guid.NewGuid(),
            CommonName = "User 2",
            Email = "user2@example.com",
            MattermostName = "second.user"
        };

        var device1 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.001",
            Name = "First device",
            PhotoId = Guid.NewGuid()
        };
        var device2 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.002",
            Name = "Second device",
            PhotoId = Guid.NewGuid()
        };
        var device3 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.003",
            Name = "Third device",
            PhotoId = Guid.NewGuid()
        };
        var device4 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.004",
            Name = "Fourth device",
            PhotoId = Guid.NewGuid()
        };

        var firstBookingFirstUser = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device1.Id,
            ReturnAt = DateTime.Now.AddDays(1),
            TakeAt = DateTime.Now.AddDays(-1),
            State = DeviceStates.Booked,
            UserId = user1.Id
        };
        
        var secondBookingFirstUser = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device2.Id,
            ReturnAt = DateTime.Now.AddDays(5),
            TakeAt = DateTime.Now.AddDays(-3),
            State = DeviceStates.Booked,
            UserId = user1.Id
        };
        var firstBookingSecondUser = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device3.Id,
            ReturnAt = DateTime.Now.AddDays(10),
            TakeAt = DateTime.Now.AddDays(-4),
            State = DeviceStates.Booked,
            UserId = user2.Id
        };
        var freeBooking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device4.Id,
            State = DeviceStates.Free
        };

        var users = new List<MongoUser>()
        {
            user1, user2
        };
        var devices = new List<DeviceModel>()
        {
            device1, device2, device3, device4
        };
        var bookings = new List<BookingModel>()
        {
            firstBookingFirstUser, secondBookingFirstUser, firstBookingSecondUser, freeBooking
        };
        
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny<NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(bookings);
        
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(devices);
        
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(users);
        
        
        var expectedResult = new List<BookingsNotificationDto>()
        {
            new()
            {
                Email = user1.Email,
                MattermostName = user1.MattermostName,
                Name = user1.CommonName,
                Devices = new List<BookingInfoDto>()
                {
                    new()
                    {
                        Name = device1.Name,
                        ReturnAt = firstBookingFirstUser.ReturnAt.Value,
                        TakeAt = firstBookingFirstUser.TakeAt.Value,
                        InventoryNumber = device1.InventoryNumber
                    },
                    new()
                    {
                        Name = device2.Name,
                        ReturnAt = secondBookingFirstUser.ReturnAt.Value,
                        TakeAt = secondBookingFirstUser.TakeAt.Value,
                        InventoryNumber = device2.InventoryNumber
                    }
                }
            },
            new()
            {
                Email = user2.Email,
                MattermostName = user2.MattermostName,
                Name = user2.CommonName,
                Devices = new List<BookingInfoDto>()
                {
                    new()
                    {
                        Name = device3.Name,
                        ReturnAt = firstBookingSecondUser.ReturnAt.Value,
                        TakeAt = firstBookingSecondUser.TakeAt.Value,
                        InventoryNumber = device3.InventoryNumber
                    }
                }
            }
        };

        var query = new GetBookingsForNotificationQuery();
        var handler = new GetBookingsForNotificationQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result.ToList();
        
        Assert.Equal(expectedResult, result, new BookingNotificationDtoComparer());
    }
    
    [Fact]
    public void AllBookingsAreFree()
    {
        var device1 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.001",
            Name = "First device",
            PhotoId = Guid.NewGuid()
        };
        var device2 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.002",
            Name = "Second device",
            PhotoId = Guid.NewGuid()
        };
        
        var booking1 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device1.Id,
            State = DeviceStates.Free
        };
        
        var booking2 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device2.Id,
            State = DeviceStates.Free
        };
        
        var user1 = new MongoUser
        {
            Id = Guid.NewGuid(),
            CommonName = "User 1",
            Email = "user1@example.com",
            MattermostName = "first.user"
        };
        
        var user2 = new MongoUser
        {
            Id = Guid.NewGuid(),
            CommonName = "User 2",
            Email = "user2@example.com",
            MattermostName = "second.user"
        };

        var users = new List<MongoUser>()
        {
            user1, user2
        };
        var devices = new List<DeviceModel>()
        {
            device1, device2
        };
        var bookings = new List<BookingModel>()
        {
            booking1, booking2
        };
        
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny<NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(bookings);
        
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(devices);
        
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(users);

        var expectedResult = Enumerable.Empty<BookingsNotificationDto>();
        var query = new GetBookingsForNotificationQuery();
        var handler = new GetBookingsForNotificationQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(expectedResult, result);
    }
    

    private class BookingNotificationDtoComparer : IEqualityComparer<BookingsNotificationDto>
    {
        public bool Equals(BookingsNotificationDto x, BookingsNotificationDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            if (x.Devices.Count() != y.Devices.Count()) return false;

            for (var i = 0; i < x.Devices.Count(); i++)
            {
                var xDevice = x.Devices.ElementAt(i);
                var yDevice = y.Devices.ElementAt(i);
                
                if ((xDevice.Name, xDevice.InventoryNumber, xDevice.ReturnAt, xDevice.TakeAt) != 
                    (yDevice.Name, yDevice.InventoryNumber, yDevice.ReturnAt, yDevice.TakeAt))
                {
                    return false;
                }
            }

            return (x.Email, x.Name, x.MattermostName) == (y.Email, y.Name, y.MattermostName);
        }

        public int GetHashCode(BookingsNotificationDto obj)
        {
            return HashCode.Combine(obj.Name, obj.Email, obj.MattermostName);
        }
    }
}