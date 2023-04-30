using System;
using System.Linq;
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

public class DeviceTookValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public DeviceTookValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void DeviceDoesntExist()
    {
        var command = ValidCommand;
        var deviceId = command.DeviceTook.DeviceId;
        var userId = command.DeviceTook.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None).Result)
            .Returns((DeviceModel)null);
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None).Result)
            .Returns(new MongoUser());

        var validator = new DeviceTookValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceTookMessage.DeviceId)));
    }

    [Fact]
    public void UserDoesntExist()
    {
        var command = ValidCommand;
        var deviceId = command.DeviceTook.DeviceId;
        var userId = command.DeviceTook.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None).Result)
            .Returns(new DeviceModel());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None).Result)
            .Returns((MongoUser)null);
        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(), 
                    v => v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))), CancellationToken.None).Result)
            .Returns(new BookingModel() { State = DeviceStates.Free });
        
        var validator = new DeviceTookValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceTookMessage.UserId)));
    }

    [Fact]
    public void DeviceBooked()
    {
        var command = ValidCommand;
        var deviceId = command.DeviceTook.DeviceId;
        var userId = command.DeviceTook.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){Id = deviceId, InventoryNumber = "d.00.000"});
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(), 
                    v => v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))), CancellationToken.None).Result)
            .Returns(new BookingModel() { State = DeviceStates.Booked });
        
        var validator = new DeviceTookValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceTookMessage.DeviceId)));
    }
    
    [Fact]
    public void DateInThePast()
    {
        var command = ValidCommand;
        var deviceId = command.DeviceTook.DeviceId;
        var userId = command.DeviceTook.UserId;
        command.DeviceTook.ReturnAt = DateTime.Now;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(deviceId)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){Id = deviceId, InventoryNumber = "d.00.000"});
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(), 
                    v => v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))), CancellationToken.None).Result)
            .Returns(new BookingModel() { State = DeviceStates.Free });
        
        var validator = new DeviceTookValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceTookMessage.ReturnAt)));
    }
    private BookDeviceCommand ValidCommand => new(new DeviceTookMessage()
    {
        DeviceId = Guid.NewGuid(),
        ReturnAt = DateTime.Now.AddDays(1),
        UserId = Guid.NewGuid()
    });
}