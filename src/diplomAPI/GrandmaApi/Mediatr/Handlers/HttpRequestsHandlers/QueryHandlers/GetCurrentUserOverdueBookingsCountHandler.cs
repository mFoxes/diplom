using System.Net;
using System.Security.Claims;
using Domain.Models;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Extensions;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Queries;
using MediatR;
using MongoDB.Driver.Linq;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.QueryHandlers;

public class GetCurrentUserOverdueBookingsCountHandler : IRequestHandler<GetCurrentUserOverdueBookingsCountQuery, HttpCommandResponse<TotalOverdueDeviceDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly ILogger<GetCurrentUserOverdueBookingsCountHandler> _logger;
    private readonly IHttpContextAccessor _accessor;
    private readonly ILocalizationService _localizationService;
    
    public GetCurrentUserOverdueBookingsCountHandler(IGrandmaRepository grandmaRepository, ILogger<GetCurrentUserOverdueBookingsCountHandler> logger, IHttpContextAccessor accessor, ILocalizationService localizationService)
    {
        _grandmaRepository = grandmaRepository;
        _logger = logger;
        _accessor = accessor;
        _localizationService = localizationService;
    }

    public async Task<HttpCommandResponse<TotalOverdueDeviceDto>> Handle(GetCurrentUserOverdueBookingsCountQuery request, CancellationToken cancellationToken)
    {
        var ldap = _accessor.HttpContext.GetLdapId();
        var user = await _grandmaRepository.GetAsync(
            new NotDeleted<MongoUser, Guid>().Combine(new UserByLdapId(int.Parse(ldap))));

        if (user is null)
        {
            _logger.LogWarning("Requested user by ldap id: {0} not found", ldap);
            var error = new ErrorDto()
            {
                FieldName = "LdapId",
                Message = _localizationService.GetString(LocalizationKey.User.NotSynchronized)
            };

            return new HttpCommandResponse<TotalOverdueDeviceDto>(HttpStatusCode.Forbidden, error);
        }
        
        var bookings = await _grandmaRepository.ListAsync(
            new NotDeleted<BookingModel, Guid>().Combine(new OverdueBookingsByUserId(user.Id)));

        var overdueBookingsCount = bookings?.Count();

        var result = new TotalOverdueDeviceDto
        {
            TotalOverdueDevice = overdueBookingsCount ?? 0
        };
        return new HttpCommandResponse<TotalOverdueDeviceDto>(result);
    }
}