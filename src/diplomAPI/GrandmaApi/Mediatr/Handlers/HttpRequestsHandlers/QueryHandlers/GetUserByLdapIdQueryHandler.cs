using System.Net;
using System.Security.Claims;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Extensions;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Queries;
using MediatR;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetUserByLdapIdQueryHandler : IRequestHandler<GetUserByLdapIdQuery, HttpCommandResponse<ContextUserDto>>
{
    private readonly ILogger<GetUserByLdapIdQueryHandler> _logger;
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IMapper _mapper;
    private readonly ILocalizationService _localizationService;
    private readonly IHttpContextAccessor _contextAccessor;
    
    public GetUserByLdapIdQueryHandler(ILogger<GetUserByLdapIdQueryHandler> logger, IGrandmaRepository grandmaRepository, IMapper mapper, ILocalizationService localizationService, IHttpContextAccessor contextAccessor)
    {
        _logger = logger;
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _localizationService = localizationService;
        _contextAccessor = contextAccessor;
    }
    public async Task<HttpCommandResponse<ContextUserDto>> Handle(GetUserByLdapIdQuery request, CancellationToken cancellationToken)
    {
        var ldapId = _contextAccessor.HttpContext.GetLdapId();
        var role = _contextAccessor.HttpContext.GetUserRole();
        
        
        var user =  await _grandmaRepository.GetAsync(
            new NotDeleted<MongoUser, Guid>().Combine(new UserByLdapId(int.Parse(ldapId))));
        if (user is null)
        {
            _logger.LogWarning("Requested user by ldap id: {0} not found", ldapId);
            var error = new ErrorDto()
            {
                FieldName = "LdapId",
                Message = _localizationService.GetString(LocalizationKey.User.NotSynchronized)
            };

            return new HttpCommandResponse<ContextUserDto>(HttpStatusCode.Forbidden, error);
        }

        var entity = _mapper.Map<MongoUser, ContextUserDto>(user);
        entity.Role = Enum.Parse<Roles>(role);
        _logger.LogDebug("Requested user by ldap id: {0}", ldapId);
        return new HttpCommandResponse<ContextUserDto>(entity);
    }
};