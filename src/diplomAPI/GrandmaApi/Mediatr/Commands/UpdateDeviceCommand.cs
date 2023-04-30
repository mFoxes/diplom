using MediatR;
using DTO;

namespace GrandmaApi.Mediatr.Commands;

public record UpdateDeviceCommand(DeviceDto Device) : IRequest<HttpCommandResponse<Unit>>;