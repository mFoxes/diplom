using System;
using Domain.Enums;
using System.Linq;
using System.Threading;
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
    public class GetBookingByIdQueryHandlerTests
    {
        private readonly Mock<IGrandmaRepository> _repoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<GetBookingByIdQueryHandler>> _loggerMock = new(); 

        [Fact]
        public void DeviceFree()
        {
            var booking = new BookingModel
            {
                Id = Guid.NewGuid(),
                State = DeviceStates.Free,
                DeviceId = Guid.NewGuid(),
                UserId = null,
                TakeAt = null,
                ReturnAt = null
            };

            var device = new DeviceModel
            {
                Id = booking.DeviceId,
                Name = "Test",
                InventoryNumber = "d.00.000",
                PhotoId = Guid.NewGuid()
            };

            var bookingDto = new BookingDto
            {
                Id = device.Id,
                State = DeviceStates.Free
            };

            _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(booking.Id)),
                CancellationToken.None).Result).Returns(booking);
           _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(device.Id)),
                CancellationToken.None).Result).Returns(device);

            _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(booking)).Returns(bookingDto);

            var query = new GetBookingByIdQuery(booking.Id);
            var handler = new GetBookingByIdQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;


            Assert.Equal(DeviceStates.Free, result.Result.State);
            Assert.Null(result.Result.UserId);
            Assert.Null(result.Result.TakeAt);
            Assert.Null(result.Result.ReturnAt);

            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None));
            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<BookingModel, Guid>>(), CancellationToken.None));
            _mapperMock.Verify(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>()));

        }

        [Fact]
        public void DeviceBooked()
        {
            var booking = new BookingModel
            {
                Id = Guid.NewGuid(),
                State = DeviceStates.Booked,
                DeviceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                TakeAt = DateTime.Now.AddDays(-3),
                ReturnAt = DateTime.Now.AddDays(3)
            };

            var device = new DeviceModel
            {
                Id = booking.DeviceId,
                Name = "Test",
                InventoryNumber = "d.00.000",
                PhotoId = Guid.NewGuid()
            };

            var user = new MongoUser()
            {
                Id = booking.UserId.Value,
                CommonName = "User"
            };

            var bookingDto = new BookingDto
            {
                Id = device.Id,
                State = DeviceStates.Booked,
                UserId = booking.UserId,
                ReturnAt = booking.ReturnAt,
                TakeAt = booking.TakeAt
            };

            _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(booking.Id)),
                CancellationToken.None).Result).Returns(booking);
           _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<DeviceModel, Guid>>(s => s.Ids.Contains(device.Id)),
                CancellationToken.None).Result).Returns(device);
            _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<MongoUser, Guid>>(s => s.Ids.Contains(booking.UserId.Value)),
                CancellationToken.None).Result).Returns(user);

            _mapperMock.Setup(mapper => mapper.Map<BookingModel, BookingDto>(booking)).Returns(bookingDto);

            var query = new GetBookingByIdQuery(booking.Id);
            var handler = new GetBookingByIdQueryHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
            var result = handler.Handle(query, CancellationToken.None).Result;


            Assert.Equal(DeviceStates.Booked, result.Result.State);
            Assert.Equal(user.CommonName, result.Result.TakedBy);
            Assert.Equal(booking.TakeAt, result.Result.TakeAt);
            Assert.Equal(booking.ReturnAt, result.Result.ReturnAt);

            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None));
            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<BookingModel, Guid>>(), CancellationToken.None));
            _repoMock.Verify(repo => repo.GetAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None));
            _mapperMock.Verify(mapper => mapper.Map<BookingModel, BookingDto>(It.IsAny<BookingModel>()));

        }
    }
}