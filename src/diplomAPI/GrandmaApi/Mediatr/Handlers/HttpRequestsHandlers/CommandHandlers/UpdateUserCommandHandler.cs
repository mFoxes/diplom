using System.Text.Json;
using Singularis.Internal.Domain.Specifications;
using AutoMapper;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using Domain.Models;
using MediatR;
using DTO;
using GrandmaApi.RabbitMqBus;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, HttpCommandResponse<Unit>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly IRabbitMqMessageService _messageService;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(IGrandmaRepository grandmaRepository, IMapper mapper, IRabbitMqMessageService messageService, ILogger<UpdateUserCommandHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _messageService = messageService;
        _logger = logger;
    }
    public async Task<HttpCommandResponse<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        
        var dbEntry = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(request.User.Id));
        dbEntry = _mapper.Map(request.User, dbEntry);
        await _grandmaRepository.SaveAsync(dbEntry);
        var filename = await _grandmaRepository.GetAsync(new NotDeleted<FileModel, Guid>(dbEntry.PhotoId.Value));
        _logger.LogDebug($"User updated: {JsonSerializer.Serialize(dbEntry)}");
        _messageService.SendMessage(new EmbeddingsRequestDto()
        {
            Id = dbEntry.Id,
            Path = filename.StorredFileName
        },
            MessageTypes.EmbeddingsUpdate);
        _logger.LogDebug($"Requested embeddings with file {filename}");
        return new HttpCommandResponse<Unit>(Unit.Value);
    }
}