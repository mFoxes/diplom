using System;
using System.Linq;
using System.Threading;
using Domain.Enums;
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

public class DeleteDeviceCommandValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public DeleteDeviceCommandValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>())).Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }
    [Fact]
    public void DeviceDoesntExist()
    {
        var id = Guid.NewGuid();
        var deleteCommand = new DeleteDeviceCommand(id);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result).Returns((DeviceModel)null);

        var validator = new DeleteDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(deleteCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeleteDeviceCommand.Id)));
    }

    [Fact]
    public void DeviceBooked()
    {
        var id = Guid.NewGuid();
        var deleteCommand = new DeleteDeviceCommand(id);
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result).Returns(new DeviceModel());
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<ISpecification<BookingModel>>(),CancellationToken.None).Result).Returns(new BookingModel(){State = DeviceStates.Booked});
        
        var validator = new DeleteDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(deleteCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(DeleteDeviceCommand.Id)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var id = Guid.NewGuid();
        var deleteCommand = new DeleteDeviceCommand(id);
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result).Returns(new DeviceModel());
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<ISpecification<BookingModel>>(), CancellationToken.None).Result).Returns(new BookingModel(){State = DeviceStates.Free});
        
        var validator = new DeleteDeviceCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(deleteCommand).Result;
        
        Assert.True(result.IsValid);
    }
}