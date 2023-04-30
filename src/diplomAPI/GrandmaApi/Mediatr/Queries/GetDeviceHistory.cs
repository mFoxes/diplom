using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Queries;

public record GetDeviceHistory(Guid Id) : IRequest<HttpCommandResponse<HistoryListDto>>;