using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record UpdateUserCommand(UserCardDto User) : IRequest<HttpCommandResponse<Unit>>;