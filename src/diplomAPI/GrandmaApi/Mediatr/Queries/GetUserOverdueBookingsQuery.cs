using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetUserOverdueBookingsQuery() : IRequest<HttpCommandResponse<OverdueBookingsListDto>>;