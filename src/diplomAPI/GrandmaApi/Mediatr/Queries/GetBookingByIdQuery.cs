using MediatR;
using DTO;

namespace GrandmaApi.Mediatr.Queries;

public record GetBookingByIdQuery(Guid Id) : IRequest<HttpCommandResponse<BookingDto>>;