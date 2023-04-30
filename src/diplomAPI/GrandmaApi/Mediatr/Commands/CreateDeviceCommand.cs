using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record CreateDeviceCommand(DeviceDto Device) : IRequest<HttpCommandResponse<Guid>>;