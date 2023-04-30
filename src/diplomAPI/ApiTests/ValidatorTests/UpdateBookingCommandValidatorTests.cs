using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;
using Domain.Enums;

namespace ApiTests.ValidatorTests;

public class UpdateBookingCommandValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public UpdateBookingCommandValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public async Task BookingDoesntExist()
    {
        var updateCommand = ValidCommand;
        var bookingId = updateCommand.BookingDto.Id;
        var userId = updateCommand.BookingDto.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId.Value)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None).Result)
            .Returns((BookingModel)null);

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UpdateBookingDto.Id)));
    }
    
    [Fact]
    public void UserDoesntExist()
    {
        var updateCommand = ValidCommand;
        var bookingId = updateCommand.BookingDto.Id;
        var userId = updateCommand.BookingDto.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId.Value)),CancellationToken.None).Result)
            .Returns((MongoUser) null);
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UpdateBookingDto.UserId)));
    }

    [Fact]
    public void UserIsNull()
    {
        var updateCommand = ValidCommand;
        var bookingId = updateCommand.BookingDto.Id;
        updateCommand.BookingDto.UserId = null;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UpdateBookingDto.UserId)));
    }

    [Fact]
    public void BookingIsFree()
    {
        var updateCommand = ValidCommand;
        updateCommand.BookingDto.UserId = null;
        updateCommand.BookingDto.TakeAt = default;
        updateCommand.BookingDto.ReturnAt = default;
        updateCommand.BookingDto.State = DeviceStates.Free;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(updateCommand.BookingDto.Id)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.True(result.IsValid);
    }
    [Fact]
    public void DateInThePast()
    {
        var updateCommand = ValidCommand;
        updateCommand.BookingDto.ReturnAt = DateTime.Now;
        var bookingId = updateCommand.BookingDto.Id;
        var userId = updateCommand.BookingDto.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId.Value)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UpdateBookingDto.ReturnAt)));
    }

    [Fact]
    public void TakeDateGreaterReturnDate()
    {
        var updateCommand = ValidCommand;
        updateCommand.BookingDto.ReturnAt = DateTime.Now.AddDays(1);
        updateCommand.BookingDto.TakeAt = DateTime.Now.AddDays(5);
        var bookingId = updateCommand.BookingDto.Id;
        var userId = updateCommand.BookingDto.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId.Value)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UpdateBookingDto.TakeAt)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var updateCommand = ValidCommand;
        var bookingId = updateCommand.BookingDto.Id;
        var userId = updateCommand.BookingDto.UserId;
        
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId.Value)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(bookingId)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new UpdateBookingCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        
        Assert.True(result.IsValid);
    }
    
    private UpdateBookingCommand ValidCommand => new(new UpdateBookingDto()
    {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        ReturnAt = DateTime.Now.Add(TimeSpan.FromDays(1)),
        State = DeviceStates.Booked,
        TakeAt = DateTime.Now
    });
}