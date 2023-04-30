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

public class GetDeviceByIdQueryValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public GetDeviceByIdQueryValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void DeviceDoesnExist()
    {
        var id = Guid.NewGuid();
        var query = new GetDeviceByIdQuery(id);

        _repoMock.Setup(repo => repo.Get(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id))))
            .Returns((DeviceModel)null);

        var validator = new GetDeviceByIdQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(GetDeviceByIdQuery.DeviceId)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var id = Guid.NewGuid();
        var query = new GetDeviceByIdQuery(id);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new DeviceModel());

        var validator = new GetDeviceByIdQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.True(result.IsValid);
    }
}