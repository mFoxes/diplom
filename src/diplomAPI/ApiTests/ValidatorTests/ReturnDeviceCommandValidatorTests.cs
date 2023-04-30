using System;
using System.Threading;
using ApiTests.Helpers;
using Domain.Enums;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using GrandmaApi.Models.MessageModels;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class ReturnDeviceCommandValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public ReturnDeviceCommandValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void DeviceDoesntExist()
    {
        var inventoryNumber = "d.00.000";
        var command = new ReturnDeviceCommand(new InventoryNumberCheckMessage()
        {
            InventoryNumber = inventoryNumber
        });

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new ReturnDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(InventoryNumberCheckMessage.InventoryNumber)));
    }
    
    [Fact]
    public void DeviceNotBooked()
    {
        var device = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.000"
        };
        var command = new ReturnDeviceCommand(new InventoryNumberCheckMessage()
        {
            InventoryNumber = device.InventoryNumber
        });


        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == device.InventoryNumber))), CancellationToken.None).Result)
            .Returns(device);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                    v => v.CheckSpecification<BookingByDeviceId>(i =>
                        i.DeviceId == device.Id))), CancellationToken.None).Result)
            .Returns(new BookingModel()
            {
                State = DeviceStates.Free
            });
        
        var validator = new ReturnDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(InventoryNumberCheckMessage.InventoryNumber)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var device = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.000"
        };
        var command = new ReturnDeviceCommand(new InventoryNumberCheckMessage()
        {
            InventoryNumber = device.InventoryNumber
        });


        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == device.InventoryNumber))), CancellationToken.None).Result)
            .Returns(device);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(),
                    v => v.CheckSpecification<BookingByDeviceId>(i =>
                        i.DeviceId == device.Id))), CancellationToken.None).Result)
            .Returns(new BookingModel()
            {
                State = DeviceStates.Booked
            });
        
        var validator = new ReturnDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.True(result.IsValid);
    }
}