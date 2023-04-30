using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.QueryHandlers
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<GetUserByIdQueryHandler>> _loggerMock = new(); 

        [Fact]
        public void EntityReturned()
        {
            var user = new MongoUser
            {
                Id = Guid.NewGuid(),
                CommonName = "Name",
                Email = "test@example.com",
                MattermostName = "test.test",
                PhotoId = Guid.NewGuid(),
            };

            var userDto = new UserCardDto
            {
                Id = user.Id,
                Name = user.CommonName,
                Email = user.Email,
                MattermostName = user.MattermostName,
                PhotoId = user.PhotoId
            };

            _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(user.Id)),
                CancellationToken.None).Result).Returns(user);
            
            _mapperMock.Setup(mapper => mapper.Map<MongoUser, UserCardDto>(user)).Returns(userDto);

            var query = new GetUserByIdQuery(user.Id);
            var handler = new GetUserByIdQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;


            Assert.Equal(userDto, result.Result);

            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None));
            _mapperMock.Verify(mapper => mapper.Map<MongoUser, UserCardDto>(It.IsAny<MongoUser>()));

        }
    }
}