using System.Net;
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

public class GetUserOverdueBookingsQueryHandler : IRequestHandler<GetUserOverdueBookingsQuery, HttpCommandResponse<OverdueBookingsListDto>>
{
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<GetUserOverdueBookingsQueryHandler> _logger;
    public GetUserOverdueBookingsQueryHandler(IGrandmaRepository grandmaRepository, IHttpContextAccessor contextAccessor, ILogger<GetUserOverdueBookingsQueryHandler> logger, ILocalizationService localizationService)
    {
        _grandmaRepository = grandmaRepository;
        _contextAccessor = contextAccessor;
        _logger = logger;
        _localizationService = localizationService;
    }

    public async Task<HttpCommandResponse<OverdueBookingsListDto>> Handle(GetUserOverdueBookingsQuery request, CancellationToken cancellationToken)
    {
        var ldap = _contextAccessor.HttpContext.GetLdapId();
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

            return new HttpCommandResponse<OverdueBookingsListDto>(HttpStatusCode.Forbidden, error);
        }

        var bookings =
            await _grandmaRepository.ListAsync(
                new NotDeleted<BookingModel, Guid>().Combine(new OverdueBookingsByUserId(user.Id)), cancellationToken);
        
        var overdueBookings = await Task.WhenAll(bookings
            ?.Select(async b =>
            {
                var device = await _grandmaRepository.GetAsync(new NotDeleted<DeviceModel, Guid>(b.DeviceId), cancellationToken);
                return new BookingInfoDto()
                {
                    InventoryNumber = device.InventoryNumber,
                    Name = device.Name,
                    TakeAt = b.TakeAt.Value,
                    ReturnAt = b.ReturnAt.Value
                };
            }) ?? Array.Empty<Task<BookingInfoDto>>());
        var result = new OverdueBookingsListDto(overdueBookings);
        return new HttpCommandResponse<OverdueBookingsListDto>(result);
    }
}