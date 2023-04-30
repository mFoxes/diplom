using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record UpdateBookingCommand(UpdateBookingDto BookingDto) : IRequest<HttpCommandResponse<Unit>>;