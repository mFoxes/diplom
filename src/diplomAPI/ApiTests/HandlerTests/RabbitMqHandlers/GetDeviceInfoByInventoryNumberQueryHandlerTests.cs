using System;
using GrandmaApi.Database;
using Domain.Models;
using Moq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;
using Xunit;
using GrandmaApi.Database.Specifications;
using System.Threading;
using ApiTests.Helpers;
using Domain.Enums;
using GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Models.MessageModels;
using Microsoft.Extensions.Logging;

namespace ApiTests.HandlerTests.RabbitMqHandlers
{
    public class GetDeviceInfoByInventoryNumberQueryHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<ILogger<GetDeviceInfoByInventoryNumberQueryHandler>> _loggerMock = new();
        
        [Fact]
        public void DeviceBooked()
        {
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel()
            {
                Id = deviceId,
                Name = "Device",
                InventoryNumber = "d.00.000"
            };

            var booking = new BookingModel()
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                State = DeviceStates.Booked
            };
            
            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o => 
                            o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(), 
                                       v =>v.CheckSpecification<InventoryNumberSpecification>(s => s.InventoryNumber == device.InventoryNumber))), 
                                       CancellationToken.None).Result)
                                       .Returns(device);
            
            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o => 
                        o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(), 
                            v =>v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))), 
                    CancellationToken.None).Result)
                .Returns(booking);
            
            var query = new GetDeviceInfoByInventoryNumberQuery(new InventoryNumberCheckMessage(){InventoryNumber = device.InventoryNumber});
            var handler = new GetDeviceInfoByInventoryNumberQueryHandler(_repoMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;
            
            Assert.Equal(deviceId, result.Result.Id);
            Assert.Equal(device.Name, result.Result.Name);
            Assert.Equal(booking.State, result.Result.State);
        }
        [Fact]
        public void DeviceFree()
        {
            var deviceId = Guid.NewGuid();
            var device = new DeviceModel()
            {
                Id = deviceId,
                Name = "Device",
                InventoryNumber = "d.00.000"
            };

            var booking = new BookingModel()
            {
                Id = Guid.NewGuid(),
                DeviceId = deviceId,
                State = DeviceStates.Free
            };
            
            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(o => 
                        o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(), 
                            v =>v.CheckSpecification<InventoryNumberSpecification>(s => s.InventoryNumber == device.InventoryNumber))), 
                    CancellationToken.None).Result)
                .Returns(device);
            
            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(o => 
                        o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(), 
                            v =>v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == deviceId))), 
                    CancellationToken.None).Result)
                .Returns(booking);
            
            var query = new GetDeviceInfoByInventoryNumberQuery(new InventoryNumberCheckMessage(){InventoryNumber = device.InventoryNumber});
            var handler = new GetDeviceInfoByInventoryNumberQueryHandler(_repoMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;
            
            Assert.Equal(deviceId, result.Result.Id);
            Assert.Equal(device.Name, result.Result.Name);
            Assert.Equal(booking.State, result.Result.State);
        }
    }
}