using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
using GrandmaApi.Models.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.QueryHandlers
{
    public class GetDevicesQueryHandlerTests
    {
        private readonly IEnumerable<DeviceModel> _devices;
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<GetDevicesQueryHandler>> _loggerMock = new(); 
        public GetDevicesQueryHandlerTests()
        {
            _devices = Enumerable.Range(1, 30).Select((u, i) =>  
                new DeviceModel
                {
                Id = Guid.NewGuid(),
                Name = $"Test {i}",
                InventoryNumber = $"d.0{i % 10}.0{(i * i) % 10}{i % 10}",
                PhotoId = Guid.NewGuid()
                }).ToList();
        }

        [Fact]
        public void SortedByDefault()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(_devices.Count());
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(_devices.ToList());
            
            IEnumerable<DeviceModel> filtered = _devices.OrderBy(u => u.InventoryNumber);
            var filteredCount = filtered.Count();
            filtered = filtered.Skip(10).Take(10);
            var expectedResult = DeviceDto(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<DeviceModel>, IEnumerable<DeviceDto>>(It.IsAny<IEnumerable<DeviceModel>>())).Returns(expectedResult);
            
            var query = new GetDevicesQuery()
            {
                Take = 10,
                Skip = 10,
                OrderBy = DeviceOrderBy.InventoryNumber,
                OrderDirection = OrderDirections.Asc
            };
            var handler = new GetDevicesQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new DeviceDtoComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_devices.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        [Fact]
        public void SortedWithInventoryNumberFilter()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(_devices.Count());
            
            
            IEnumerable<DeviceModel> filtered = _devices.Where(u => u.InventoryNumber.Contains("1")).OrderBy(u => u.InventoryNumber);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<DeviceModel>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(0).Take(10);
            var expectedResult = DeviceDto(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<DeviceModel>, IEnumerable<DeviceDto>>(It.IsAny<IEnumerable<DeviceModel>>())).Returns(expectedResult);
            
            var query = new GetDevicesQuery()
            {
                Take = 10,
                Skip = 0,
                OrderBy = DeviceOrderBy.InventoryNumber,
                OrderDirection = OrderDirections.Asc,
                FilterInventoryNumber = "1"
            };
            var handler = new GetDevicesQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new DeviceDtoComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_devices.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }
        [Fact]
        public void SortedWithNameFilter()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(_devices.Count());
            
            
            IEnumerable<DeviceModel> filtered = _devices.Where(u => u.Name.Contains("1")).OrderBy(u => u.Name);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<DeviceModel>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(0).Take(10);
            var expectedResult = DeviceDto(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<DeviceModel>, IEnumerable<DeviceDto>>(It.IsAny<IEnumerable<DeviceModel>>())).Returns(expectedResult);
            
            var query = new GetDevicesQuery()
            {
                Take = 8,
                Skip = 4,
                OrderBy = DeviceOrderBy.Name,
                OrderDirection = OrderDirections.Asc,
                FilterInventoryNumber = "1"
            };
            var handler = new GetDevicesQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new DeviceDtoComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_devices.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        [Fact]
        public void SortedWithInventoryNumberAndNameFilter()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(_devices.Count());
            
            
            IEnumerable<DeviceModel> filtered = _devices.Where(u => u.Name.Contains("1") && u.InventoryNumber.Contains("1")).OrderBy(u => u.Name);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<DeviceModel>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(0).Take(10);
            var expectedResult = DeviceDto(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<DeviceModel>, IEnumerable<DeviceDto>>(It.IsAny<IEnumerable<DeviceModel>>())).Returns(expectedResult);
            
            var query = new GetDevicesQuery()
            {
                Take = 7,
                Skip = 3,
                OrderBy = DeviceOrderBy.Name,
                OrderDirection = OrderDirections.Asc,
                FilterInventoryNumber = "1",
                FilterName = "1"
            };
            var handler = new GetDevicesQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new DeviceDtoComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_devices.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        [Fact]
        public void SortedWithInventoryNumberAndNameFilterOrderByInventoryNumberDescending()
        {
            _repoMock.Setup(repo => repo.CountAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(_devices.Count());
            
            
            IEnumerable<DeviceModel> filtered = _devices.Where(u => u.Name.Contains("1") && u.InventoryNumber.Contains("1")).OrderByDescending(u => u.InventoryNumber);
            _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<DeviceModel>>(), CancellationToken.None).Result).Returns(filtered.ToList());

            var filteredCount = filtered.Count();
            filtered = filtered.Skip(2).Take(5);
            var expectedResult = DeviceDto(filtered);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<DeviceModel>, IEnumerable<DeviceDto>>(It.IsAny<IEnumerable<DeviceModel>>())).Returns(expectedResult);
            
            var query = new GetDevicesQuery()
            {
                Take = 5,
                Skip = 2,
                OrderBy = DeviceOrderBy.InventoryNumber,
                OrderDirection = OrderDirections.Desc,
                FilterInventoryNumber = "1",
                FilterName = "1"
            };
            var handler = new GetDevicesQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;

            Assert.Equal(expectedResult, result.Result.Items, new DeviceDtoComparer());
            Assert.Equal(filteredCount, result.Result.TotalItemsFiltered);
            Assert.Equal(_devices.Count(), result.Result.TotalItems);
            Assert.Equal(expectedResult.Count(), result.Result.Items.Count());
        }

        private IEnumerable<DeviceDto> DeviceDto(IEnumerable<DeviceModel> users) => users.Select(d => new DeviceDto{ Id = d.Id, InventoryNumber = d.InventoryNumber, Name = d.Name, PhotoId = d.PhotoId});
        private class DeviceDtoComparer : IEqualityComparer<DeviceDto>
        {
            public bool Equals(DeviceDto? x, DeviceDto? y)
            {
                return (x.Id, x.Name, x.InventoryNumber, x.PhotoId.Value) == (y.Id, y.Name, y.InventoryNumber, y.PhotoId.Value);
            }

            public int GetHashCode([DisallowNull] DeviceDto obj)
            {
                return obj.Id.GetHashCode();
            }

        }
    }
}