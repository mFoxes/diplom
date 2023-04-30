using System.Text.Json;
using MediatR;
using DTO;
using AutoMapper;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Database;
using Domain.Enums;
using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, HttpCommandResponse<BookingDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBookingByIdQueryHandler> _logger;

    public GetBookingByIdQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetBookingByIdQueryHandler> logger)
    {
        _logger = logger;
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
    }

    public async Task<HttpCommandResponse<BookingDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _grandmaRepository.GetAsync( new NotDeleted<BookingModel, Guid>(request.Id));
        var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(booking.DeviceId));
        var bookingDto = _mapper.Map<BookingModel, BookingDto>(booking);
        bookingDto.DeviceId = device.Id;
        bookingDto.Name = device.Name;
        bookingDto.InventoryNumber = device.InventoryNumber;
        if(bookingDto.State == DeviceStates.Booked)
        {
            var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(booking.UserId.Value));
            bookingDto.TakedBy = user.CommonName;
            bookingDto.UserId = user.Id;
        }
        _logger.LogDebug($"Requested booking by id: {request.Id}. Found: {JsonSerializer.Serialize(bookingDto)}");
        return new HttpCommandResponse<BookingDto>(bookingDto);
    }
}