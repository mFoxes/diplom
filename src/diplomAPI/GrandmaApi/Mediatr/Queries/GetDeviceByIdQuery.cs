using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetDeviceByIdQuery(Guid DeviceId) : IRequest<HttpCommandResponse<DeviceDto>>;