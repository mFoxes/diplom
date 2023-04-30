using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GrandmaApi.Models.Enums;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using Domain.Models;
using Moq;
using GrandmaApi.Extensions;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using Xunit;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Singularis.Internal.Domain.Specification;

namespace ApiTests.HandlerTests.QueryHandlers
{
    public class GetUsersQueryHandlerTests
    {
        private readonly IEnumerable<MongoUser> _users;

        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<GetUsersQueryHandler>> _loggerMock = new(); 
        
        public GetUsersQueryHandlerTests()
        {
            _users = Enumerable.Range(1, 15).Select((u, i) =>  
                new MongoUser
                {
                Id = Guid.NewGuid(),
                CommonName = $"Test {i}",
                PhotoId = Guid.NewGuid()
                }).ToList();
        }

        [Fact]
        public void SortedByDefault()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(_users.Count());
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(_users.ToList());
            
            IEnumerable<MongoUser> filtered = _users.OrderBy(u => u.Id);
            var filteredCount = filtered.Count();
            filtered = filtered.Skip(10).Take(10);
            var expectedResult = UserCards(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MongoUser>, IEnumerable<UserTableItemDto>>(It.IsAny<IEnumerable<MongoUser>>())).Returns(expectedResult);
            
            var query = new GetUsersQuery()
            {
                Take = 10,
                Skip = 10,
                OrderBy = UsersOrderBy.Id,
                OrderDirection = OrderDirections.Asc
            };
            var handler = new GetUsersQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new UserTableItemComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_users.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        [Fact]
        public void SortedWithFilter()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(_users.Count());
            
            
            IEnumerable<MongoUser> filtered = _users.Where(u => u.CommonName.Contains("1")).OrderBy(u => u.Id);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<MongoUser>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(0).Take(10);
            var expectedResult = UserCards(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MongoUser>, IEnumerable<UserTableItemDto>>(It.IsAny<IEnumerable<MongoUser>>())).Returns(expectedResult);
            
            var query = new GetUsersQuery()
            {
                Take = 10,
                Skip = 0,
                OrderBy = UsersOrderBy.Id,
                OrderDirection = OrderDirections.Asc,
                FilterName = "1"
            };
            var handler = new GetUsersQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new UserTableItemComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_users.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        [Fact]
        public void SortedWithFilterSortedByName()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(_users.Count());
            
            
            IEnumerable<MongoUser> filtered = _users.Where(u => u.CommonName.Contains("1")).OrderBy(u => u.CommonName);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<MongoUser>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(0).Take(10);
            var expectedResult = UserCards(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MongoUser>, IEnumerable<UserTableItemDto>>(It.IsAny<IEnumerable<MongoUser>>())).Returns(expectedResult);
            
            var query = new GetUsersQuery()
            {
                Take = 10,
                Skip = 0,
                OrderBy = UsersOrderBy.Name,
                OrderDirection = OrderDirections.Asc,
                FilterName = "1"
            };
            var handler = new GetUsersQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new UserTableItemComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_users.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        [Fact]
        public void SortedWithFilterSortedByNameDescending()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(_users.Count());
            
            
            IEnumerable<MongoUser> filtered = _users.Where(u => u.CommonName.Contains("1")).OrderByDescending(u => u.CommonName);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<MongoUser>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(0).Take(10);
            var expectedResult = UserCards(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<MongoUser>, IEnumerable<UserTableItemDto>>(It.IsAny<IEnumerable<MongoUser>>())).Returns(expectedResult);
            
            var query = new GetUsersQuery()
            {
                Take = 10,
                Skip = 0,
                OrderBy = UsersOrderBy.Name,
                OrderDirection = OrderDirections.Asc,
                FilterName = "1"
            };
            var handler = new GetUsersQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new UserTableItemComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_users.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }
        
        private IEnumerable<UserTableItemDto> UserCards(IEnumerable<MongoUser> users) => users.Select(u => new UserTableItemDto{ Id = u.Id, Name = u.CommonName, PhotoId = u.PhotoId});
        private class UserTableItemComparer : IEqualityComparer<UserTableItemDto>
        {
            public bool Equals(UserTableItemDto? x, UserTableItemDto? y)
            {
                return (x.Id, x.Name, x.PhotoId.Value) == (y.Id, y.Name, y.PhotoId.Value);
            }

            public int GetHashCode([DisallowNull] UserTableItemDto obj)
            {
                return obj.Id.GetHashCode();
            }

        }
    }
}