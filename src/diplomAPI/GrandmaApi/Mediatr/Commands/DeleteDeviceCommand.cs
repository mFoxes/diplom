using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record DeleteDeviceCommand(Guid Id) : IRequest<HttpCommandResponse<Unit>>;