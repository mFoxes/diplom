using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.Configs;
using Domain.Models;
using GrandmaApi.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.RabbitMqHandlers
{
    public class RecognizeUserQueryHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly IOptions<ThresholdConfig> _config = Options.Create(new ThresholdConfig(){Threshold = 0.56});
        private readonly Mock<ILogger<RecognizeUserQueryHandler>> _loggerMock = new();
        private readonly Mock<ILocalizationService> _localizationMock = new();

        public RecognizeUserQueryHandlerTests()
        {
            _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>())).Returns("invalid");
            _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
        }
        [Fact]
        public void UserRecognized()
        {
            var user = new MongoUser()
            {
                Id = Guid.NewGuid(),
                CommonName = "Test user 4",
                Embeddings = new double[]{0.5, 0.5, 0.5}
            };
            var users = Users;
            users.Add(user);

            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
                     .Returns(users);
            
            var embeddings = new double[]{0.5, 0.5, 0.5};

            var query = new RecognizeUserQuery(new RecognizeUserDto(){Embeddings = embeddings});
            var handler = new RecognizeUserQueryHandler(_repoMock.Object, _config, _loggerMock.Object, _localizationMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.True(result.IsSucceed);
            Assert.Equal(user.Id, result.Result.UserId);
        }

        [Fact]
        public void UserNotRecognized()
        {
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
                     .Returns(Users);
            
            var embeddings = new double[]{0.123, 0.723, 0.423};
            var query = new RecognizeUserQuery(new RecognizeUserDto(){Embeddings = embeddings});
            var handler = new RecognizeUserQueryHandler(_repoMock.Object, _config, _loggerMock.Object, _localizationMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.False(result.IsSucceed);
        }

        private List<MongoUser> Users => new List<MongoUser>
        {
            new MongoUser()
            {
                Id = Guid.NewGuid(),
                CommonName = "Test user 1",
                Embeddings = new double[] {0.932, 0.112, 0.782}
            },
            new MongoUser()
            {
                Id = Guid.NewGuid(),
                CommonName = "Test user 2",
                Embeddings = new double[] {0.877, 0.056, 0.926}
            },
            new MongoUser()
            {
                Id = Guid.NewGuid(),
                CommonName = "Test user 3",
                Embeddings = null
            }
        };
    }
}