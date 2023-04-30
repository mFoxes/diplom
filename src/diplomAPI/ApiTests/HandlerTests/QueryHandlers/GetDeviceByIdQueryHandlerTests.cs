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
    public class GetDeviceByIdQueryHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<GetDeviceByIdQueryHandler>> _loggerMock = new(); 

        [Fact]
        public void EntityReturned()
        {
            var device = new DeviceModel
            {
                Id = Guid.NewGuid(),
                InventoryNumber = "d.00.000",
                PhotoId = Guid.NewGuid(),
                Name = "Test",
            };

            var deviceDto = new DeviceDto
            {
                Id = device.Id,
                InventoryNumber = device.InventoryNumber,
                PhotoId = device.PhotoId,
                Name = device.Name
            };

            _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(device.Id)),
                CancellationToken.None).Result).Returns(device);
            
            _mapperMock.Setup(mapper => mapper.Map<DeviceModel, DeviceDto>(device)).Returns(deviceDto);

            var query = new GetDeviceByIdQuery(device.Id);
            var handler = new GetDeviceByIdQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;


            Assert.Equal(deviceDto, result.Result);

            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None));
            _mapperMock.Verify(mapper => mapper.Map<DeviceModel, DeviceDto>(It.IsAny<DeviceModel>()));

        }
    }
}