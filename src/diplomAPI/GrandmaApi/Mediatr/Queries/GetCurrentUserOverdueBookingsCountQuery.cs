using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetCurrentUserOverdueBookingsCountQuery() : IRequest<HttpCommandResponse<TotalOverdueDeviceDto>>;