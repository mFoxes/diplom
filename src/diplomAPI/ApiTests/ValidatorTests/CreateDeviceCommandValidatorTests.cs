using System;
using System.Threading;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class CreateDeviceCommandValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public CreateDeviceCommandValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>())).Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }
    [Fact]
    public void NameLengthGreaterThenMax()
    {
        var id = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var device = new CreateDeviceCommand(new DeviceDto()
        {
            Id = id,
            InventoryNumber = "d.00.000",
            Name = new string('a', 300),
            PhotoId = photoId
        });
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<FileModel, Guid>>(), CancellationToken.None).Result).Returns(new FileModel());
        var validator = new CreateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        
        var result = validator.ValidateAsync(device).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.Name)));
    }
    
    [Fact]
    public void InventoryNumberNotMatchesExpression()
    {
        var id = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var device = new CreateDeviceCommand(new DeviceDto()
        {
            Id = id,
            InventoryNumber = "d.00a.000",
            Name = "Name",
            PhotoId = photoId
        });

        
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<FileModel, Guid>>(), CancellationToken.None).Result).Returns(new FileModel());
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<ISpecification<DeviceModel>>(), CancellationToken.None).Result).Returns(new DeviceModel());
        var validator = new CreateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        
        var result = validator.ValidateAsync(device).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.InventoryNumber)));
    }

    [Fact]
    public void InventoryNumberIsUsing()
    {
        var id = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var device = new CreateDeviceCommand(new DeviceDto()
        {
            Id = id,
            InventoryNumber = "d.00.000",
            Name = "Name",
            PhotoId = photoId
        });


        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<FileModel, Guid>>(), CancellationToken.None).Result).Returns(new FileModel());
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<ISpecification<DeviceModel>>(), CancellationToken.None).Result).Returns(new DeviceModel());
        
        var validator = new CreateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        
        var result = validator.ValidateAsync(device).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.InventoryNumber)));
    }
    
    [Fact]
    public void PhotoNotFound()
    {
        var id = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var device = new CreateDeviceCommand(new DeviceDto()
        {
            Id = id,
            InventoryNumber = "d.00.000",
            Name = "Name",
            PhotoId = photoId
        });
        var validator = new CreateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        
        var result = validator.ValidateAsync(device).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.PhotoId)));
    }
    
    [Fact]
    public void NameEmpty()
    {
        var id = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var device = new CreateDeviceCommand(new DeviceDto()
        {
            Id = id,
            InventoryNumber = "d.00.000",
            Name = "",
            PhotoId = photoId
        });


        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<FileModel, Guid>>(), CancellationToken.None).Result).Returns(new FileModel());
        var validator = new CreateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        
        var result = validator.ValidateAsync(device).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeviceDto.Name)));
    }

    [Fact]
    public void ValidFields()
    {
        var id = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var device = new CreateDeviceCommand(new DeviceDto()
        {
            Id = id,
            InventoryNumber = "d.00.000",
            Name = "Name",
            PhotoId = photoId
        });
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<FileModel, Guid>>(), CancellationToken.None).Result).Returns(new FileModel());
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<ISpecification<DeviceModel>>(), CancellationToken.None).Result).Returns((DeviceModel)null);
        
        var validator = new CreateDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        
        var result = validator.ValidateAsync(device).Result;
        Assert.True(result.IsValid);
    }
}