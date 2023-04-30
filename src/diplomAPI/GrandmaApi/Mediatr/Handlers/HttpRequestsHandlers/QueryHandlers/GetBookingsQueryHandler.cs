using System.Net;
using System.Text.Json;
using GrandmaApi.Mediatr.Queries;
using MediatR;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Extensions;
using Domain.Models;
using GrandmaApi.Models.Enums;
using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;
using GrandmaApi.Database.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, HttpCommandResponse<TableDto<BookingDto>>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<GetBookingsQueryHandler> _logger;

    public GetBookingsQueryHandler(IGrandmaRepository grandmaRepository, ILogger<GetBookingsQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<TableDto<BookingDto>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        ISpecification<DeviceModel> deviceSpecification = new NotDeleted<DeviceModel, Guid>();
        ISpecification<BookingModel> bookingSpecification = new NotDeleted<BookingModel, Guid>();
        ISpecification<MongoUser> userSpecification = new NotDeleted<MongoUser, Guid>();
        
        var count = await _grandmaRepository.CountAsync(bookingSpecification);
        _logger.LogDebug($"Requested list of bookings with parameters: {JsonSerializer.Serialize(request)}");

        if(request.FilterName != null)
        {
            deviceSpecification = deviceSpecification.Combine(new FilterDeviceByName(request.FilterName));
        }
        if (request.FilterInventoryNumber != null)
        {
            deviceSpecification = deviceSpecification.Combine(new FilterDeviceByInventoryNumber(request.FilterInventoryNumber));
        }
        if (request.FilterTakedBy != null)
        {
            userSpecification = userSpecification.Combine(new FilterUserByName(request.FilterTakedBy));
        }
        var devices = await _grandmaRepository.ListAsync(deviceSpecification);
        var users = await _grandmaRepository.ListAsync(userSpecification);
        bookingSpecification = bookingSpecification.Combine(new BookingByDeviceId(devices.Select(d => d.Id)));
        bookingSpecification = bookingSpecification.Combine(new BookingByUserId(users.Select(u => u.Id).ToList()));
        var bookings = await _grandmaRepository.ListAsync(bookingSpecification);
        

        var filteredBookings = bookings.Join(devices, b => b.DeviceId, d => d.Id, (booking, device) => new BookingDto
        {
            Id = booking.Id,
            DeviceId = device.Id,
            State = booking.State,
            Name = device.Name,
            InventoryNumber = device.InventoryNumber,
            UserId = booking.UserId,
            TakeAt = booking.TakeAt,
            ReturnAt = booking.ReturnAt
        }).Select(bookingDevice => 
        {
            if(bookingDevice.UserId.HasValue)
            {
                 bookingDevice.TakedBy = users.FirstOrDefault(u => u.Id == bookingDevice.UserId.Value).CommonName;     
            }
            return bookingDevice;
        });
        if(request.FilterTakedBy is not null)
        {
            filteredBookings = filteredBookings.Where(f => f.UserId.HasValue);
        }
        filteredBookings = request.OrderBy switch
        {
            BookingsOrderBy.Name => filteredBookings.OrderBy(d => d.Name, request.OrderDirection),
            BookingsOrderBy.InventoryNumber => filteredBookings.OrderBy(d => d.InventoryNumber, request.OrderDirection),
            _ => filteredBookings.OrderBy(d => d.State, request.OrderDirection)
                .ThenBy(d => d.ReturnAt.HasValue ? d.ReturnAt.Value.Date : DateTime.Now, request.OrderDirection)
        };
        var filteredCount = filteredBookings.Count();
        filteredBookings = filteredBookings.Skip(request.Skip).Take(request.Take);
        _logger.LogDebug($"Total items: {count}; Filtered items filtered: {filteredCount}");
        var devicesListDto = new TableDto<BookingDto>()
        {
            Items = filteredBookings,
            TotalItems = count,
            TotalItemsFiltered = filteredCount
        };
        return new HttpCommandResponse<TableDto<BookingDto>>(devicesListDto);
    }
}