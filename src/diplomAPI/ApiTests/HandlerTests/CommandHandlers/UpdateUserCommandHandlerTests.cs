using System;
using System.Linq;
using System.Threading;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using Domain.Models;
using GrandmaApi.RabbitMqBus;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.CommandHandlers;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IRabbitMqMessageService> _rabbitMock = new();
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock = new(); 
    
    [Fact]
    public void UserUpdated()
    {
        var userId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var user = new MongoUser()
        {
            Id = userId,
            PhotoId = photoId
        };
        var photo = new FileModel()
        {
            Id = photoId,
            StorredFileName = "test"
        };
        
        _repoMock.Setup(repo =>
                repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None).Result)
            .Returns(user);
        
        _repoMock.Setup(repo =>
                repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(photo);

        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<UserCardDto>(), It.IsAny<MongoUser>())).Returns(user);

        var handler = new UpdateUserCommandHandler(_repoMock.Object, _mapperMock.Object, _rabbitMock.Object, _loggerMock.Object);

        handler.Handle(new UpdateUserCommand(new UserCardDto() { Id = userId }), CancellationToken.None);
        
        _repoMock.Verify(repo => repo.SaveAsync(user, CancellationToken.None));
    }
    
    [Fact]
    public void MessageServiceInvoked()
    {
        var userId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var user = new MongoUser()
        {
            Id = userId,
            PhotoId = photoId
        };
        var photo = new FileModel()
        {
            Id = photoId,
            StorredFileName = "test"
        };
        
        _repoMock.Setup(repo =>
                repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(userId)), CancellationToken.None).Result)
            .Returns(user);
        
        _repoMock.Setup(repo =>
                repo.GetAsync(It.Is<NotDeleted<FileModel, Guid>>(s => s.Ids.Contains(photoId)), CancellationToken.None).Result)
            .Returns(photo);

        _mapperMock.Setup(mapper => mapper.Map(It.IsAny<UserCardDto>(), It.IsAny<MongoUser>())).Returns(user);
        

        var handler = new UpdateUserCommandHandler(_repoMock.Object, _mapperMock.Object, _rabbitMock.Object, _loggerMock.Object);
        handler.Handle(new UpdateUserCommand(new UserCardDto() { Id = userId }), CancellationToken.None);
        
        _rabbitMock.Verify(ms => ms.SendMessage(It.IsAny<EmbeddingsRequestDto>(), MessageTypes.EmbeddingsUpdate));
        
    }
}