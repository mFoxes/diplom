using System;
using System.Linq;
using System.Threading;
using GrandmaApi.Database;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class GetUserByIdQueryValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public GetUserByIdQueryValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void UserDoesntExist()
    {
        var id = Guid.NewGuid();
        var query = new GetUserByIdQuery(id);

        _repoMock.Setup(repo => repo.Get(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id))))
            .Returns((MongoUser)null);

        var validator = new GetUserByIdQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(GetUserByIdQuery.Id)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var id = Guid.NewGuid();
        var query = new GetUserByIdQuery(id);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());

        var validator = new GetUserByIdQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.True(result.IsValid);
    }
}