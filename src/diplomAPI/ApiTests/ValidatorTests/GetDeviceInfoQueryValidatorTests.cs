using System;
using System.Threading;
using ApiTests.Helpers;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Models.MessageModels;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class GetDeviceInfoQueryValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public GetDeviceInfoQueryValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void DeviceByInventoryNumberNotFound()
    {
        var inventoryNumber = "d.00.000";
        var query = new GetDeviceInfoByInventoryNumberQuery(new InventoryNumberCheckMessage()
            { InventoryNumber = inventoryNumber});
        
        _repoMock.Setup(repo => repo.Get(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber)))))
            .Returns((DeviceModel) null);

        var validator = new GetDeviceInfoQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(GetDeviceInfoByInventoryNumberQuery.InventoryNumberMessage.InventoryNumber)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var inventoryNumber = "d.00.000";
        var query = new GetDeviceInfoByInventoryNumberQuery(new InventoryNumberCheckMessage()
            { InventoryNumber = inventoryNumber});
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns(new DeviceModel());

        var validator = new GetDeviceInfoQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.True(result.IsValid);
    }
}