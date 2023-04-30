using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.QueryHandlers
{
    public class GetUsernamesQueryHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<GetUsernamesQueryHandler>> _loggerMock = new(); 
        
        [Fact]
        public void GetUsers()
        {
            var users = new List<MongoUser>()
            {
                new MongoUser
                {
                    Id = Guid.NewGuid(),
                    CommonName = "User1"
                },
                new MongoUser
                {
                    Id = Guid.NewGuid(),
                    CommonName = "User2"
                }
            };
            var usernames = users.Select(u => new UsernameDto{Id = u.Id, Name = u.CommonName});

            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(users);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MongoUser>, IEnumerable<UsernameDto>>(users)).Returns(usernames);

            var query = new GetUsernamesQuery();
            var handler = new GetUsernamesQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            handler.Handle(query, CancellationToken.None);
            _repoMock.Verify(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None));
            _mapperMock.Verify(mapper => mapper.Map<IEnumerable<MongoUser>, IEnumerable<UsernameDto>>(It.IsAny<IEnumerable<MongoUser>>()));
        }


    }
}