using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using ApiTests.Helpers;
using AutoMapper;
using Domain.Enums;
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

public class GetUserByLdapIdQueryHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<GetUserByLdapIdQueryHandler>> _loggerMock = new();
    private readonly Mock<ILocalizationService> _localizationMock = new();
    private readonly Mock<IHttpContextAccessor> _accessorMock = new();

    public GetUserByLdapIdQueryHandlerTests()
    {
        var httpContextMock = new Mock<HttpContext>();
        _accessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContextMock.Object);
        _accessorMock.Setup(accessor => accessor.HttpContext.User).Returns(new ClaimsPrincipal());
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(ClaimTypes.UserData, "1000"));
        identity.AddClaim(new Claim(ClaimTypes.Role, "Employee"));
        
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
                                v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))),
                        CancellationToken.None)
                    .Result)
            .Returns((MongoUser)null);
        
        var query = new GetUserByLdapIdQuery();
        var handler = new GetUserByLdapIdQueryHandler(_loggerMock.Object, _repoMock.Object, _mapperMock.Object, _localizationMock.Object, _accessorMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;

        
        _repoMock.Verify(repo =>
            repo.GetAsync(
                It.Is<CombinedSpecification<MongoUser>>(o =>
                    o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                        v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None));

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        Assert.Equal(1, result.ErrorsList.Errors.Count());
        Assert.Null(result.Result);
        
    }

    [Fact]
    public void EntityReturned()
    {
        var user = new MongoUser
        {
            Id = Guid.NewGuid(),
            CommonName = "Name",
            Email = "test@example.com",
            MattermostName = "test.test",
            PhotoId = Guid.NewGuid(),
        };

        var ldapId = 1000;
        var role = Roles.Employee;

        var userDto = new ContextUserDto
        {
            Id = user.Id,
            Name = user.CommonName,
            Email = user.Email,
            MattermostName = user.MattermostName,
            PhotoId = user.PhotoId,
            Role = role
        };

        _repoMock.Setup(repo =>
                repo.GetAsync(
                        It.Is<CombinedSpecification<MongoUser>>(o =>
                            o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                                v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))),
                        CancellationToken.None)
                    .Result)
            .Returns(user);

        _mapperMock.Setup(mapper => mapper.Map<MongoUser, ContextUserDto>(user)).Returns(userDto);

        var query = new GetUserByLdapIdQuery();
        var handler = new GetUserByLdapIdQueryHandler(_loggerMock.Object, _repoMock.Object, _mapperMock.Object, _localizationMock.Object, _accessorMock.Object);
        var result = handler.Handle(query, CancellationToken.None).Result;


        Assert.Equal(userDto, result.Result);

        _repoMock.Verify(repo =>
            repo.GetAsync(
                It.Is<CombinedSpecification<MongoUser>>(o =>
                    o.Validate(v => v.CheckSpecification<NotDeleted<MongoUser, Guid>>(),
                        v => v.CheckSpecification<UserByLdapId>(s => s.LdapIds.Contains(ldapId)))), CancellationToken.None));
        _mapperMock.Verify(mapper => mapper.Map<MongoUser, ContextUserDto>(It.IsAny<MongoUser>()));
    
    }
}