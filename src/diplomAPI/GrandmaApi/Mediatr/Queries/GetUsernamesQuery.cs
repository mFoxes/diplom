using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetUsernamesQuery() : IRequest<HttpCommandResponse<IEnumerable<UsernameDto>>>;