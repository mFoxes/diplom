using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ApiTests.Helpers;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Operations;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.QueryHandlers;

public class GetDeviceHistoryQueryHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<ILogger<GetDeviceHistoryQueryHandler>> _loggerMock = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public void DeviceNotFound()
    {
        var handler = new GetDeviceHistoryQueryHandler(_repoMock.Object, _mapper.Object, _loggerMock.Object);
        var query = new GetDeviceHistory(Guid.NewGuid());
        var result = handler.Handle(query, CancellationToken.None).Result;

        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Fact]
    public void CorrectDeviceHistory()
    {
        var deviceId = Guid.NewGuid();
        var histories = new List<BookingHistory>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                TakeAt = DateTime.Now.AddDays(-5),
                ReturnedAt = DateTime.Now.AddDays(-3),
                TakedBy = "User 1"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                TakeAt = DateTime.Now.AddDays(-15),
                ReturnedAt = DateTime.Now.AddDays(-6),
                TakedBy = "User 2"
            },
            new()
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                TakeAt = DateTime.Now.AddDays(-25),
                ReturnedAt = DateTime.Now.AddDays(-10),
                TakedBy = "User 3"
            }
        };
        var device = new DeviceModel()
        {
            Id = deviceId,
            InventoryNumber = "d.00.000",
            Name = "Test device"
        };
        
        var mapperResult = histories.Select(h => new HistoryDto()
            { ReturnedAt = h.ReturnedAt, TakeAt = h.TakeAt, TakedBy = h.TakedBy });
        
        _repoMock.Setup(repo => repo.ListAsync(It.IsAny<CombinedSpecification<BookingHistory>>(), CancellationToken.None).Result).Returns(histories);
        _repoMock.Setup(repo => repo.GetAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result).Returns(device);
        
        _mapper.Setup(mapper => mapper.Map<IEnumerable<BookingHistory>, IEnumerable<HistoryDto>>(histories))
            .Returns(mapperResult);
        
        mapperResult = mapperResult.OrderByDescending(h => h.TakeAt).ToList();
        var handler = new GetDeviceHistoryQueryHandler(_repoMock.Object, _mapper.Object, _loggerMock.Object);
        var result = handler.Handle(new GetDeviceHistory(deviceId), CancellationToken.None).Result;
        
        Assert.Equal(mapperResult, result.Result.History, new HistoryDtoComparer());
        Assert.Equal(device.Name, result.Result.Name);
        Assert.Equal(device.InventoryNumber, result.Result.InventoryNumber);
    }
    
    
    private class HistoryDtoComparer : IEqualityComparer<HistoryDto>
    {
        public bool Equals(HistoryDto x, HistoryDto y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.TakedBy == y.TakedBy && x.TakeAt.Equals(y.TakeAt) && x.ReturnedAt.Equals(y.ReturnedAt);
        }

        public int GetHashCode(HistoryDto obj)
        {
            return HashCode.Combine(obj.TakedBy, obj.TakeAt, obj.ReturnedAt);
        }
    }
}