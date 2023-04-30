using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using ApiTests.Helpers;
using Domain.Models;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using GrandmaApi.Mediatr.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.QueryHandlers;

public class GetUserOverdueBookingsQueryHandlerTests
{
    private readonly Mock<ILogger<GetUserOverdueBookingsQueryHandler>> _loggerMock = new(); 
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IHttpContextAccessor> _accessorMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();
    
    public GetUserOverdueBookingsQueryHandlerTests()
    {
        var httpContextMock = new Mock<HttpContext>();
        _accessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContextMock.Object);
        _accessorMock.Setup(accessor => accessor.HttpContext.User).Returns(new ClaimsPrincipal());
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(ClaimTypes.UserData, "1000"));
        _accessorMock.Setup(accessor => accessor.HttpContext.User.Identity).Returns(identity);
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void UserDoesntSynchronized()
    {
        var ldapId = 1000;
        _repoMock.Setup(repo =>
            repo.GetAsync(
                    It.Is<CombinedSpecification<MongoUser>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                            v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None)
                .Result).Returns((MongoUser)null);

        var query = new GetUserOverdueBookingsQuery();
        var handler = new GetUserOverdueBookingsQueryHandler(_repoMock.Object, _accessorMock.Object, _loggerMock.Object,
            _localizationMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.Equal(1, result.ErrorsList.Errors.Count);
        Assert.Null(result.Result);
    }

    [Fact]
    public void UserDoesntHaveBookings()
    {
        var ldapId = 1000;

        var user = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = ldapId
        };
        
        _repoMock.Setup(repo =>
            repo.GetAsync(
                    It.Is<CombinedSpecification<MongoUser>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                            v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None)
                .Result).Returns(user);
        
        _repoMock.Setup(repo =>
            repo.ListAsync(
                    It.Is<CombinedSpecification<BookingModel>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                            v => v.CheckSpecification<OverdueBookingsByUserId>(s => s.UserId == user.Id))), CancellationToken.None)
                .Result).Returns((IReadOnlyCollection<BookingModel>)null);
        
        
        var query = new GetUserOverdueBookingsQuery();
        var handler = new GetUserOverdueBookingsQueryHandler(_repoMock.Object, _accessorMock.Object, _loggerMock.Object,
            _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(0, result.Result.Bookings.Count());
        _repoMock.Verify(repo => repo.ListAsync(It.IsAny<CombinedSpecification<BookingModel>>(), CancellationToken.None));
    }
    [Fact]
    public void UserDoesntHaveOverdueBookings()
    {
        var ldapId = 1000;

        var user = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = ldapId
        };

        var booking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ReturnAt = DateTime.Now.AddDays(10)
        };
        
        _repoMock.Setup(repo =>
            repo.GetAsync(
                    It.Is<CombinedSpecification<MongoUser>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                            v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None)
                .Result).Returns(user);
        
        _repoMock.Setup(repo =>
            repo.ListAsync(
                    It.Is<CombinedSpecification<BookingModel>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                            v => v.CheckSpecification<OverdueBookingsByUserId>(s => s.UserId == user.Id))), CancellationToken.None)
                .Result).Returns((IReadOnlyCollection<BookingModel>)null);
        
        
        var query = new GetUserOverdueBookingsQuery();
        var handler = new GetUserOverdueBookingsQueryHandler(_repoMock.Object, _accessorMock.Object, _loggerMock.Object,
            _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(0, result.Result.Bookings.Count());
        _repoMock.Verify(repo => repo.ListAsync(It.IsAny<CombinedSpecification<BookingModel>>(), CancellationToken.None));
    }
    
    [Fact]
    public void UserHasOverdueBookings()
    {
        var ldapId = 1000;

        var user = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = ldapId
        };

        var booking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DeviceId = Guid.NewGuid(),
            TakeAt = DateTime.UtcNow.Date.AddDays(-10),
            ReturnAt = DateTime.UtcNow.Date.AddDays(-1)
        };
        var device = new DeviceModel()
        {
            Id = booking.DeviceId,
            Name = "Device test",
            InventoryNumber = "d.00.000"
        };
        
        _repoMock.Setup(repo =>
            repo.GetAsync(
                    It.Is<CombinedSpecification<MongoUser>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                            v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None)
                .Result).Returns(user);
        
        _repoMock.Setup(repo =>
            repo.ListAsync(
                    It.Is<CombinedSpecification<BookingModel>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                            v => v.CheckSpecification<OverdueBookingsByUserId>(s => s.UserId == user.Id))), CancellationToken.None)
                .Result).Returns(new List<BookingModel>(){booking});

        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(booking.DeviceId)),
                CancellationToken.None).Result).Returns(device);
        
        var query = new GetUserOverdueBookingsQuery();
        var handler = new GetUserOverdueBookingsQueryHandler(_repoMock.Object, _accessorMock.Object, _loggerMock.Object,
            _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        var resultBookings = new List<BookingInfoDto>()
        {
            new BookingInfoDto()
            {
                InventoryNumber = device.InventoryNumber,
                Name = device.Name,
                ReturnAt = booking.ReturnAt.Value,
                TakeAt = booking.TakeAt.Value
            }
        };
        
        Assert.Equal(1, result.Result.Bookings.Count());
        Assert.Equal(resultBookings, result.Result.Bookings, new BookingInfoDtoComparer());
        _repoMock.Verify(repo => repo.ListAsync(It.IsAny<CombinedSpecification<BookingModel>>(), CancellationToken.None));
    }
    [Fact]
    public void UserHasMoreThanOneOverdueBooking()
    {
        var ldapId = 1000;

        var user = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = ldapId
        };

        var booking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DeviceId = Guid.NewGuid(),
            TakeAt = DateTime.UtcNow.Date.AddDays(-10),
            ReturnAt = DateTime.UtcNow.Date.AddDays(-5)
        };
        var booking2 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DeviceId = Guid.NewGuid(),
            TakeAt = DateTime.UtcNow.Date.AddDays(-10),
            ReturnAt = DateTime.UtcNow.Date.AddDays(-5)
        };

        var device = new DeviceModel()
        {
            Id = booking.DeviceId,
            Name = "Device test",
            InventoryNumber = "d.00.000"
        };
        var device2 = new DeviceModel()
        {
            Id = booking2.DeviceId,
            Name = "Device test 2",
            InventoryNumber = "d.00.001"
        };
        _repoMock.Setup(repo =>
            repo.GetAsync(
                    It.Is<CombinedSpecification<MongoUser>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                            v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None)
                .Result).Returns(user);
        
        _repoMock.Setup(repo =>
            repo.ListAsync(
                    It.Is<CombinedSpecification<BookingModel>>(o =>
                        o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                            v => v.CheckSpecification<OverdueBookingsByUserId>(s => s.UserId==user.Id))), CancellationToken.None)
                .Result).Returns(new List<BookingModel>(){booking, booking2});

        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(booking.DeviceId)),
                CancellationToken.None).Result).Returns(device);
        _repoMock.Setup(repo =>
            repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(booking2.DeviceId)),
                CancellationToken.None).Result).Returns(device2);
        
        var query = new GetUserOverdueBookingsQuery();
        var handler = new GetUserOverdueBookingsQueryHandler(_repoMock.Object, _accessorMock.Object, _loggerMock.Object,
            _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        var resultBookings = new List<BookingInfoDto>()
        {
            new BookingInfoDto()
            {
                InventoryNumber = device.InventoryNumber,
                Name = device.Name,
                ReturnAt = booking.ReturnAt.Value,
                TakeAt = booking.TakeAt.Value
            },
            new BookingInfoDto()
            {
                InventoryNumber = device2.InventoryNumber,
                Name = device2.Name,
                ReturnAt = booking2.ReturnAt.Value,
                TakeAt = booking2.TakeAt.Value
            }
        };
        
        Assert.Equal(2, result.Result.Bookings.Count());
        Assert.Equal(resultBookings, result.Result.Bookings, new BookingInfoDtoComparer());
        _repoMock.Verify(repo => repo.ListAsync(It.IsAny<CombinedSpecification<BookingModel>>(), CancellationToken.None));
    }
    private class BookingInfoDtoComparer : IEqualityComparer<BookingInfoDto>
    {
        public bool Equals(BookingInfoDto x, BookingInfoDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name && x.InventoryNumber == y.InventoryNumber && x.TakeAt.Equals(y.TakeAt) && x.ReturnAt.Equals(y.ReturnAt);
        }

        public int GetHashCode(BookingInfoDto obj)
        {
            return HashCode.Combine(obj.Name, obj.InventoryNumber, obj.TakeAt, obj.ReturnAt);
        }
    }
}