using System;
using System.Linq;
using System.Threading;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class UpdateUserCommandValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public UpdateUserCommandValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void UserDoesntExist()
    {
        var updateCommand = ValidUpdateUserCommand;
        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns((MongoUser)null);
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UserCardDto.Id)));
    }

    [Fact]
    public void UserNameIsEmpty()
    {
        var updateCommand = ValidUpdateUserCommand;
        updateCommand.User.Name = string.Empty;

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(2, result.Errors.Count);
        Assert.True(result.Errors.Where(e => e.PropertyName == nameof(UserCardDto.Name)).Count() == 2);
    }

    [Fact]
    public void UserNameDoesntMatchTemplate()
    {
        var updateCommand = ValidUpdateUserCommand;
        updateCommand.User.Name = new string('R', 20);

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UserCardDto.Name)));
    }

    [Fact]
    public void UserNameLengthGreaterThenMax()
    {
        var updateCommand = ValidUpdateUserCommand;
        updateCommand.User.Name = new string('ё', 100);

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UserCardDto.Name)));
    }

    [Fact]
    public void EmailDoesntMatchTemplate()
    {
        var updateCommand = ValidUpdateUserCommand;
        updateCommand.User.Email = "test.com";

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UserCardDto.Email)));
    }

    [Fact]
    public void FileNotFound()
    {
        var updateCommand = ValidUpdateUserCommand;

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns((FileModel)null);

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UserCardDto.PhotoId)));
    }

    [Fact]
    public void MattermostDoesntMatchTemplate()
    {
        var updateCommand = ValidUpdateUserCommand;
        updateCommand.User.MattermostName = "wrong";

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(UserCardDto.MattermostName)));
    }

    [Fact]
    public void ValidModel()
    {
        var updateCommand = ValidUpdateUserCommand;

        var id = updateCommand.User.Id;
        var photoId = updateCommand.User.PhotoId.Value;

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new MongoUser());
        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(new FileModel());

        var validator = new UpdateUserCommandValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(updateCommand).Result;
        Assert.True(result.IsValid);
    }

    private UpdateUserCommand ValidUpdateUserCommand => new (new UserCardDto()
    {
        Id = Guid.NewGuid(),
        Email = "test@test.com",
        MattermostName = "test.test",
        Name = "Тест",
        PhotoId = Guid.NewGuid()
    });
}