using System.Net;
using AutoMapper;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using MediatR;
using DTO;
using Domain.Models;
using System.Text.Json;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, HttpCommandResponse<UserCardDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<UserCardDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Requested user by id: {request.Id}");
        var user = await _grandmaRepository.GetAsync(new NotDeleted<MongoUser, Guid>(request.Id));
        var entity = _mapper.Map<MongoUser, UserCardDto>(user);
        _logger.LogDebug($"Found user: {JsonSerializer.Serialize(entity)}");
        return new HttpCommandResponse<UserCardDto>(entity);
    }
}