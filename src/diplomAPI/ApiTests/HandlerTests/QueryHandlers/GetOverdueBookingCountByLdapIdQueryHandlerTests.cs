using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using ApiTests.Helpers;
using Domain.Models;
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

public class GetOverdueBookingCountByLdapIdQueryHandlerTests
{
    private readonly Mock<ILogger<GetCurrentUserOverdueBookingsCountHandler>> _loggerMock = new(); 
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IHttpContextAccessor> _accessorMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();

    public GetOverdueBookingCountByLdapIdQueryHandlerTests()
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
                .Result).Returns((MongoUser)null);
        
        
        var query = new GetCurrentUserOverdueBookingsCountQuery();
        var handler = new GetCurrentUserOverdueBookingsCountHandler(_repoMock.Object, _loggerMock.Object, _accessorMock.Object, _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.Equal(1, result.ErrorsList.Errors.Count());
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

        var booking = new BookingModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ReturnAt = DateTime.UtcNow.Date.AddDays(10)
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

        
        
        var query = new GetCurrentUserOverdueBookingsCountQuery();
        var handler = new GetCurrentUserOverdueBookingsCountHandler(_repoMock.Object, _loggerMock.Object, _accessorMock.Object, _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(0, result.Result.TotalOverdueDevice);
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
        
        
        var query = new GetCurrentUserOverdueBookingsCountQuery();
        var handler = new GetCurrentUserOverdueBookingsCountHandler(_repoMock.Object, _loggerMock.Object, _accessorMock.Object, _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(0, result.Result.TotalOverdueDevice);
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
            ReturnAt = DateTime.Now.AddDays(-10),
        };
        var booking2 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ReturnAt = DateTime.Now.AddDays(-10)
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
                .Result).Returns(new List<BookingModel>(){booking, booking2});

        var query = new GetCurrentUserOverdueBookingsCountQuery();
        var handler = new GetCurrentUserOverdueBookingsCountHandler(_repoMock.Object, _loggerMock.Object, _accessorMock.Object, _localizationMock.Object);

        var result = handler.Handle(query, CancellationToken.None).Result;
        
        Assert.Equal(2, result.Result.TotalOverdueDevice);
        _repoMock.Verify(repo => repo.ListAsync(It.IsAny<CombinedSpecification<BookingModel>>(), CancellationToken.None));
    }
}