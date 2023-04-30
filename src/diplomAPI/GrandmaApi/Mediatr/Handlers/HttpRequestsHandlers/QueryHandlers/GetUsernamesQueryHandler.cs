using MediatR;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Database;
using Domain.Models;
using Singularis.Internal.Domain.Specifications;
using AutoMapper;
using DTO;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetUsernamesQueryHandler : IRequestHandler<GetUsernamesQuery, HttpCommandResponse<IEnumerable<UsernameDto>>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsernamesQueryHandler> _logger;
    public GetUsernamesQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetUsernamesQueryHandler> logger)
    {
        _logger = logger;
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
    }

    public async Task<HttpCommandResponse<IEnumerable<UsernameDto>>> Handle(GetUsernamesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Requested list of usersnames");
        var users = await _grandmaRepository.ListAsync(new NotDeleted<MongoUser, Guid>());
        var entity = _mapper.Map<IEnumerable<MongoUser>, IEnumerable<UsernameDto>>(users);
        return new HttpCommandResponse<IEnumerable<UsernameDto>>(entity);
    }
}