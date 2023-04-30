using System;
using GrandmaApi.Database;
using Domain.Models;
using Moq;
using Singularis.Internal.Domain.Specification;
using Xunit;
using GrandmaApi.Database.Specifications;
using Singularis.Internal.Domain.Specifications;
using System.Threading;
using ApiTests.Helpers;
using AutoMapper;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Models.MessageModels;
using GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;
using Domain.Enums;
using DTO;
using GrandmaApi.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ApiTests.HandlerTests.RabbitMqHandlers
{
    public class ReturnDeviceCommandHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<ILogger<ReturnDeviceCommandHandler>> _loggerMock = new();
        private readonly Mock<IHubContext<GrandmaHub, IGrandmaHub>> _hubMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        
        [Fact]
        public void DeviceReturned()
        {
            var device = new DeviceModel()
            {
                Id = Guid.NewGuid(),
                Name = "Test device",
                InventoryNumber = "d.00.000"
            };

            var booking = new BookingModel()
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                UserId = Guid.NewGuid(),
                ReturnAt = DateTime.Now.AddDays(5),
                TakeAt = DateTime.Now.AddDays(-3),
                State = DeviceStates.Booked
            };
            var bookingDto = new BookingDto()
            {
                Id = booking.Id,
                State = booking.State,
                TakeAt = booking.TakeAt,
                ReturnAt = booking.ReturnAt,
                UserId = booking.UserId
            };
            var user = new MongoUser()
            {
                Id = booking.UserId.Value,
                CommonName = "Test"
            };
            var history = new BookingHistory()
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                TakedBy = user.CommonName,
                TakeAt = booking.TakeAt.Value
            };
            
            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<DeviceModel>>(
                o => o.Validate(v => v.CheckSpecification<NotDeleted<DeviceModel, Guid>>(), 
                                v => v.CheckSpecification<InventoryNumberSpecification>(s => s.InventoryNumber == device.InventoryNumber)))
                            , CancellationToken.None).Result)
                        .Returns(device);

            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingModel>>(
                o => o.Validate(v => v.CheckSpecification<NotDeleted<BookingModel, Guid>>(), 
                                v => v.CheckSpecification<BookingByDeviceId>(s => s.DeviceId == device.Id)))
                            , CancellationToken.None).Result)
                        .Returns(booking);
            
            _repoMock.Setup(repo => repo.GetAsync(It.Is<CombinedSpecification<BookingHistory>>(
                o => o.Validate(v => v.CheckSpecification<NotDeleted<BookingHistory, Guid>>(), 
                                v => v.CheckSpecification<ActiveBookingHistoryByDeviceId>(s => s.DeviceId == device.Id)))
                            , CancellationToken.None).Result)
                        .Returns(history);

            _repoMock.Setup(repo =>
                repo.GetAsync(
                    It.IsAny<NotDeleted<DeviceModel, Guid>>(),
                    CancellationToken.None).Result).Returns(device);
        
            _repoMock.Setup(repo =>
                repo.GetAsync(
                    It.IsAny<NotDeleted<MongoUser, Guid>>(),
                    CancellationToken.None).Result).Returns(user);
            

            _repoMock.Setup(repo =>
                repo.GetAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result).Returns(user);
            _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(booking))
                .Returns(bookingDto);
            
            var command = new ReturnDeviceCommand(new InventoryNumberCheckMessage(){ InventoryNumber = device.InventoryNumber});
            var handler = new ReturnDeviceCommandHandler(_repoMock.Object, _loggerMock.Object, _hubMock.Object, _mapperMock.Object);
            
            handler.Handle(command, CancellationToken.None);

            Assert.Null(booking.UserId);
            Assert.Null(booking.ReturnAt);
            Assert.Null(booking.TakeAt);
            Assert.Equal(DeviceStates.Free, booking.State);
            Assert.NotNull(history.ReturnedAt);
            _mapperMock.Verify(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>()));
            
            _repoMock.Verify(repo => repo.SaveAsync(booking, CancellationToken.None));
            _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<BookingHistory>(), CancellationToken.None));
            
            _repoMock.Verify(repo => repo.GetAsync( It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None));
            _repoMock.Verify(repo => repo.GetAsync( It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None));
        }
    }
}