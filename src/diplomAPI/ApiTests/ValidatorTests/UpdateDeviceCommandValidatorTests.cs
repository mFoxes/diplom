using System;
using System.Linq;
using System.Threading;
using ApiTests.Helpers;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class UpdateDeviceCommandValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public UpdateDeviceCommandValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void DeviceNotExists()
    {
        var command = ValidCommand;
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.Id)));
    }
    
    [Fact]
    public void DeviceNameEmpty()
    {
        var command = ValidCommand;
        command.Device.Name = string.Empty;
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){InventoryNumber = inventoryNumber});

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.Name)));
    }
    
    [Fact]
    public void DeviceNameLengthGreaterThenMax()
    {
        var command = ValidCommand;
        command.Device.Name = new string('a', 300);
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){InventoryNumber = inventoryNumber});

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.Name)));
    }
    
    [Fact]
    public void InventoryNumberDoesntMatchTemplate()
    {
        var command = ValidCommand;
        command.Device.InventoryNumber = "d.0d.000";
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){InventoryNumber = inventoryNumber});

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.InventoryNumber)));
    }
    
    [Fact]
    public void InventoryNumberIsInUsing()
    {
        var command = ValidCommand;
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){InventoryNumber = "d.00.001"});
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns(new DeviceModel());

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.InventoryNumber)));
    }
    [Fact]
    public void PhotoNotFound()
    {
        var command = ValidCommand;
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new DeviceModel(){InventoryNumber = inventoryNumber});

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns((FileModel)null);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))), CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.PhotoId)));
    }

    [Fact]
    public void ValidModel()
    {
        var command = ValidCommand;
        var id = command.Device.Id;
        var photoId = command.Device.PhotoId.Value;
        var inventoryNumber = command.Device.InventoryNumber;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)),CancellationToken.None).Result)
            .Returns(new DeviceModel(){InventoryNumber = inventoryNumber});

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o =>
                o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(),
                    v => v.CheckSpecification<InventoryNumberSpecification>(i =>
                        i.InventoryNumber == inventoryNumber))),CancellationToken.None).Result)
            .Returns((DeviceModel)null);

        var validator = new UpdateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(command).Result;
        
        Assert.True(result.IsValid);
    }

    private UpdateDeviceCommand ValidCommand => new (new DeviceDto()
    {
        Id = Guid.NewGuid(),
        Name = "Device",
        InventoryNumber = "d.00.000",
        PhotoId = Guid.NewGuid()
    });
}