using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using MediatR;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Mediatr.Handlers.RabbitMqHandlers;

public class UpdateEmbeddingsCommandHandler : IRequestHandler<UpdateEmbeddingsCommand, BrokerCommandResponse<Unit>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateEmbeddingsCommandHandler> _logger;
    public UpdateEmbeddingsCommandHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<UpdateEmbeddingsCommandHandler> logger)
    {
        _logger = logger;
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
    }

    public async Task<BrokerCommandResponse<Unit>> Handle(UpdateEmbeddingsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Requested update embeddings of user with id {request.EmbeddingsResult.Id}");
        var dbEntry = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(request.EmbeddingsResult.Id));
        _mapper.Map(request.EmbeddingsResult, dbEntry);
        await _grandmaRepository.SaveAsync(dbEntry);
        return new BrokerCommandResponse<Unit>(Unit.Value);
    }
}