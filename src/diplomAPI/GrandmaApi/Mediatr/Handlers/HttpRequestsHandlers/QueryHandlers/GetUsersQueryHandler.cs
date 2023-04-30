using System.Text.Json;
using Singularis.Internal.Domain.Specifications;
using Singularis.Internal.Domain.Specification;
using GrandmaApi.Database.Specifications;
using AutoMapper;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Models.Enums;
using GrandmaApi.Extensions;
using MediatR;
using DTO;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, HttpCommandResponse<TableDto<UserTableItemDto>>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(IGrandmaRepository grandmaRepository, IMapper mapper, ILogger<GetUsersQueryHandler> logger)
    {
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<HttpCommandResponse<TableDto<UserTableItemDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Requested list of users with parameters: {JsonSerializer.Serialize(request)}");
        ISpecification<MongoUser> userSpecification = new NotDeleted<MongoUser, Guid>();
        var count = await _grandmaRepository.CountAsync(userSpecification);
        if (request.FilterName != null)
        {
            userSpecification = userSpecification.Combine(new FilterUserByName(request.FilterName));
        }
        IEnumerable<MongoUser> filteredUsers = await _grandmaRepository.ListAsync(userSpecification);

        filteredUsers = request.OrderBy == UsersOrderBy.Name
            ? filteredUsers.OrderBy(d => d.CommonName, request.OrderDirection)
            : filteredUsers.OrderBy(d => d.Id, request.OrderDirection);
        var filteredCount = filteredUsers.Count();
        filteredUsers = filteredUsers.Skip(request.Skip).Take(request.Take);
        
        var entities = _mapper.Map<IEnumerable<MongoUser>, IEnumerable<UserTableItemDto>>(filteredUsers);
        _logger.LogDebug($"Total items count: {count}. Total items filtered: {filteredCount}");
        return new HttpCommandResponse<TableDto<UserTableItemDto>>(new TableDto<UserTableItemDto>()
        {
            Items = entities,
            TotalItems = count,
            TotalItemsFiltered = filteredCount
        });
    }
}