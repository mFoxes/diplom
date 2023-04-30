using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetBookingsForNotificationQuery() : IRequest<IEnumerable<BookingsNotificationDto>>;