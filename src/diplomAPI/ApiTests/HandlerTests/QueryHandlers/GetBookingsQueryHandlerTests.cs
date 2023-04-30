using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Models.Enums;
using Microsoft.Extensions.Logging;
using Domain.Enums;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.QueryHandlers;

public class GetBookingsQueryHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<ILogger<GetBookingsQueryHandler>> _loggerMock = new(); 
    private readonly List<MongoUser> _users;
    private readonly List<DeviceModel> _devices;
    private readonly List<BookingModel> _bookings;

    public GetBookingsQueryHandlerTests()
    {
        var user1 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            CommonName = "User 1",
            Email = "user1@example.com",
        };

        var user2 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            CommonName = "User 2",
            Email = "user2@example.com"
        };
        var user3 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            CommonName = "User 3",
            Email = "user3@example.com"
        };

        var device1 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.001",
            Name = "Device 1"
        };
        var device2 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.002",
            Name = "Device 2"
        };
        var device3 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.013",
            Name = "Device 3"
        };
        var device4 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.223",
            Name = "Device 4"
        };
        var booking1 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device1.Id,
            UserId = user1.Id,
            State = DeviceStates.Booked,
            TakeAt = DateTime.Now.AddDays(-5),
            ReturnAt = DateTime.Now.AddDays(5)
        };
        
        var booking2 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device2.Id,
            UserId = user2.Id,
            State = DeviceStates.Booked,
            TakeAt = DateTime.Now.AddDays(-5),
            ReturnAt = DateTime.Now.AddDays(5)
        };
        var booking3 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device3.Id,
            UserId = user1.Id,
            State = DeviceStates.Booked,
            TakeAt = DateTime.Now.AddDays(-10),
            ReturnAt = DateTime.Now.AddDays(-5)
        };
        var booking4 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device4.Id,
            State = DeviceStates.Free
        };
        
        _bookings = new List<BookingModel>() { booking1, booking2, booking3, booking4 };
        _devices = new List<DeviceModel>() { device1, device2, device3, device4 };
        _users = new List<MongoUser>() { user1, user2 };
    }

    [Fact]
    public void SortedByNameAsc()
    {
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_devices);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(_users);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<BookingModel>>(), CancellationToken.None).Result)
            .Returns(_bookings);
        _repoMock.Setup(repo =>
                repo.CountAsync(It.IsAny <NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_bookings.Count());

        var filteredBookings = _bookings.Join(_devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice =>
        {
            if (bookingDevice.UserId.HasValue)
            {
                bookingDevice.TakedBy = _users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;
            }

            return bookingDevice;
        });
        filteredBookings = filteredBookings.OrderBy(b => b.Name);
        var count = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(0).Take(2);

        var query = new GetBookingsQuery()
        {
            OrderBy = BookingsOrderBy.Name,
            OrderDirection = OrderDirections.Asc,
            Skip = 0,
            Take = 2
        };

        var handler = new GetBookingsQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(_bookings.Count(), result.Result.TotalItems);
        Assert.Equal(count, result.Result.TotalItemsFiltered);
        Assert.Equal(filteredBookings, result.Result.Items, new BookingDtoComparer());

    }
    [Fact]
    public void SortedByStateAsc()
    {
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_devices);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(_users);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<BookingModel>>(), CancellationToken.None).Result)
            .Returns(_bookings);
        _repoMock.Setup(repo =>
                repo.CountAsync(It.IsAny <NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_bookings.Count());

        var filteredBookings = _bookings.Join(_devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice =>
        {
            if (bookingDevice.UserId.HasValue)
            {
                bookingDevice.TakedBy = _users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;
            }

            return bookingDevice;
        });
        filteredBookings.OrderBy(d => d.State)
            .ThenBy(d => d.ReturnAt.HasValue ? d.ReturnAt.Value.Date : DateTime.Now);
        var count = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(0).Take(2);

        var query = new GetBookingsQuery()
        {
            OrderBy = BookingsOrderBy.Name,
            OrderDirection = OrderDirections.Asc,
            Skip = 0,
            Take = 2
        };

        var handler = new GetBookingsQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(_bookings.Count(), result.Result.TotalItems);
        Assert.Equal(count, result.Result.TotalItemsFiltered);
        Assert.Equal(filteredBookings, result.Result.Items, new BookingDtoComparer());

    }
    
    [Fact]
    public void SortedByNameDescFilteredByInventoryNumber()
    {
        var devices = _devices.Where(d => d.InventoryNumber.Contains("1"));
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<DeviceModel>>(), CancellationToken.None).Result)
            .Returns(devices.ToList());
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(_users);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<BookingModel>>(), CancellationToken.None).Result)
            .Returns(_bookings);
        _repoMock.Setup(repo =>
                repo.CountAsync(It.IsAny <NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_bookings.Count());

        var filteredBookings = _bookings.Join(devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice =>
        {
            if (bookingDevice.UserId.HasValue)
            {
                bookingDevice.TakedBy = _users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;
            }

            return bookingDevice;
        });
        filteredBookings = filteredBookings.OrderByDescending(b => b.Name);
        var count = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(0).Take(2);

        var query = new GetBookingsQuery()
        {
            OrderBy = BookingsOrderBy.Name,
            OrderDirection = OrderDirections.Desc,
            Skip = 0,
            Take = 2,
            FilterInventoryNumber = "1"
        };

        var handler = new GetBookingsQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(_bookings.Count(), result.Result.TotalItems);
        Assert.Equal(count, result.Result.TotalItemsFiltered);
        Assert.Equal(filteredBookings, result.Result.Items, new BookingDtoComparer());

    }
    
    [Fact]
    public void SortedByNameDescFilteredByTakedBy()
    {
        var users = _users.Where(d => d.CommonName.Contains("1"));
        var bookings = _bookings.Where(b => b.UserId.HasValue && users.Select(u => u.Id).Contains(b.UserId.Value)).ToList();
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_devices);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<MongoUser>>(), CancellationToken.None).Result)
            .Returns(users.ToList());
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<BookingModel>>(), CancellationToken.None).Result)
            .Returns(bookings);
        _repoMock.Setup(repo =>
                repo.CountAsync(It.IsAny <NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_bookings.Count());

        var filteredBookings = bookings.Join(_devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice =>
        {
            if (bookingDevice.UserId.HasValue)
            {
                bookingDevice.TakedBy = users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;
            }

            return bookingDevice;
        });
        filteredBookings = filteredBookings.OrderByDescending(b => b.Name);
        var count = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(0).Take(2);

        var query = new GetBookingsQuery()
        {
            OrderBy = BookingsOrderBy.Name,
            OrderDirection = OrderDirections.Desc,
            Skip = 0,
            Take = 2,
            FilterTakedBy = "1"
        };

        var handler = new GetBookingsQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(_bookings.Count(), result.Result.TotalItems);
        Assert.Equal(count, result.Result.TotalItemsFiltered);
        Assert.Equal(filteredBookings, result.Result.Items, new BookingDtoComparer());

    }
    [Fact]
    public void SortedByStateDescFilteredByInventoryNumber()
    {
        var devices = _devices.Where(d => d.InventoryNumber.Contains("1")).ToList();
        var bookings = _bookings.Where(b => devices.Select(d => d.Id).Contains(b.Id)).ToList();
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<DeviceModel>>(), CancellationToken.None).Result)
            .Returns(devices);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(_users);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<BookingModel>>(), CancellationToken.None).Result)
            .Returns(bookings);
        _repoMock.Setup(repo =>
                repo.CountAsync(It.IsAny <NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_bookings.Count());

        var filteredBookings = bookings.Join(devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice =>
        {
            if (bookingDevice.UserId.HasValue)
            {
                bookingDevice.TakedBy =  _users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;
            }

            return bookingDevice;
        });
        filteredBookings = filteredBookings.OrderByDescending(d => d.State)
            .ThenByDescending(d => d.ReturnAt.HasValue ? d.ReturnAt.Value.Date : DateTime.Now);
        var count = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(0).Take(2);

        var query = new GetBookingsQuery()
        {
            OrderBy = BookingsOrderBy.State,
            OrderDirection = OrderDirections.Desc,
            Skip = 0,
            Take = 2,
            FilterInventoryNumber = "1"
        };

        var handler = new GetBookingsQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(_bookings.Count(), result.Result.TotalItems);
        Assert.Equal(count, result.Result.TotalItemsFiltered);
        Assert.Equal(filteredBookings, result.Result.Items, new BookingDtoComparer());

    }
    [Fact]
    public void SortedByStateDescFilteredByTakedBy()
    {
        var users = _users.Where(d => d.CommonName.Contains("1"));
        var bookings = _bookings.Where(b => b.UserId.HasValue && users.Select(u => u.Id).Contains(b.UserId.Value)).ToList();
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_devices);
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<MongoUser>>(), CancellationToken.None).Result)
            .Returns(users.ToList());
        _repoMock.Setup(repo =>
                repo.ListAsync(It.IsAny <CombinedSpecification<BookingModel>>(), CancellationToken.None).Result)
            .Returns(bookings);
        _repoMock.Setup(repo =>
                repo.CountAsync(It.IsAny <NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(_bookings.Count());

        var filteredBookings = bookings.Join(_devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice =>
        {
            if (bookingDevice.UserId.HasValue)
            {
                bookingDevice.TakedBy = users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;
            }

            return bookingDevice;
        });
        filteredBookings = filteredBookings.OrderByDescending(d => d.State)
            .ThenByDescending(d => d.ReturnAt.HasValue ? d.ReturnAt.Value.Date : DateTime.Now);
        var count = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(0).Take(2);

        var query = new GetBookingsQuery()
        {
            OrderBy = BookingsOrderBy.State,
            OrderDirection = OrderDirections.Desc,
            Skip = 0,
            Take = 2,
            FilterTakedBy = "1"
        };

        var handler = new GetBookingsQueryHandler(_repoMock.Object, _loggerMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(_bookings.Count(), result.Result.TotalItems);
        Assert.Equal(count, result.Result.TotalItemsFiltered);
        Assert.Equal(filteredBookings, result.Result.Items, new BookingDtoComparer());

    }
    
    private class BookingDtoComparer : IEqualityComparer<BookingDto>
    {
        public bool Equals(BookingDto x, BookingDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id.Equals(y.Id) && x.DeviceId.Equals(y.DeviceId) && x.Name == y.Name && x.InventoryNumber == y.InventoryNumber && x.State == y.State && Nullable.Equals(x.UserId, y.UserId) && x.TakedBy == y.TakedBy && Nullable.Equals(x.TakeAt, y.TakeAt) && Nullable.Equals(x.ReturnAt, y.ReturnAt);
        }

        public int GetHashCode(BookingDto obj)
        {
            var hashCode = new HashCode();
            hashCode.Add(obj.Id);
            hashCode.Add(obj.DeviceId);
            hashCode.Add(obj.Name);
            hashCode.Add(obj.InventoryNumber);
            hashCode.Add((int)obj.State);
            hashCode.Add(obj.UserId);
            hashCode.Add(obj.TakedBy);
            hashCode.Add(obj.TakeAt);
            hashCode.Add(obj.ReturnAt);
            return hashCode.ToHashCode();
        }
    }
}