using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<HttpCommandResponse<UserCardDto>>;