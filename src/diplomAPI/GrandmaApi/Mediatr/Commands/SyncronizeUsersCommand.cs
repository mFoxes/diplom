using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record SyncronizeUsersCommand() : IRequest<HttpCommandResponse<Unit>>;