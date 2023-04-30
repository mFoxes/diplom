using DTO;
using MediatR;

namespace GrandmaApi.Mediatr.Commands;

public record UpdateEmbeddingsCommand(EmbeddingsResultDto EmbeddingsResult) : IRequest<BrokerCommandResponse<Unit>>;