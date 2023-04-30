using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.RabbitMqHandlers
{
    public class UpdateEmbeddingsCommandHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<UpdateEmbeddingsCommandHandler>> _loggerMock = new();
        
        [Fact]
        public void EmbeddingsUpdated()
        {
            var user = new MongoUser()
            {
                Id = Guid.NewGuid(),  
            };

            var embeddsResult = new EmbeddingsResultDto
            {
                Id = user.Id,
                Embeddings = new double[]{0.3, 0.2, 0.44}
            };

            _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(user);
            _mapperMock.Setup(mapper => mapper.Map(embeddsResult, user)).Callback<EmbeddingsResultDto, MongoUser>((e, u) => u.Embeddings = e.Embeddings);

            var handler = new UpdateEmbeddingsCommandHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            handler.Handle(new UpdateEmbeddingsCommand(embeddsResult), CancellationToken.None);

            Assert.NotNull(user.Embeddings);
            Assert.Equal(embeddsResult.Embeddings, user.Embeddings);

            _repoMock.Verify(repo => repo.SaveAsync(user, CancellationToken.None));


        }
    }
}