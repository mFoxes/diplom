using Domain.Enums;
using Domain.Models;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Database.Specifications;
using GrandmaApi.Mediatr.Queries;
using GrandmaApi.Notification.MessageServices;
using LdapConnector;
using MediatR;
using MongoDB.Driver.Linq;
using Quartz;
using Singularis.Internal.Domain.Specification;
using Singularis.Internal.Domain.Specifications;

namespace GrandmaApi.Notification;

[DisallowConcurrentExecution]
public class NotificationService : IJob, IDisposable
{
    private readonly INotifier _notifier;
    private readonly IMediator _mediator;
    private readonly ILdapRepository _ldapRepository;
    private readonly IServiceScope _scope;
    private readonly IGrandmaRepository _grandmaRepository;
    public NotificationService(IServiceScopeFactory _serviceScopeFactory)
    {
        _scope = _serviceScopeFactory.CreateScope();
        
        _notifier = _scope.ServiceProvider.GetService<INotifier>();
        _mediator = _scope.ServiceProvider.GetService<IMediator>();
        _ldapRepository = _scope.ServiceProvider.GetService<ILdapRepository>();
        _grandmaRepository = _scope.ServiceProvider.GetService<IGrandmaRepository>();
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var query = new GetBookingsForNotificationQuery();
        var result = _mediator.Send(query).Result;

        var adminNotification = new List<AdminNotificationDto>();

        foreach (var booking in result)
        {
            var soonReturn =
                booking.Devices.Where(d => d.ReturnAt.Date.Subtract(DateTime.UtcNow.Date) == TimeSpan.FromDays(1));
            if (soonReturn.Any())
            {
                await _notifier.NotifySoonByEmail(soonReturn, booking.Email);
                await _notifier.NotifySoonByMattermost(soonReturn, booking.MattermostName);
            }

            var overdueReturn = booking.Devices.Where(d => d.ReturnAt < DateTime.UtcNow.Date);
            if (overdueReturn.Any())
            {
                await _notifier.NotifyOverdueByEmail(overdueReturn, booking.Email);
                await _notifier.NotifyOverdueByMattermost(overdueReturn, booking.MattermostName);
                adminNotification.Add(new AdminNotificationDto()
                {
                    Name = booking.Name,
                    Devices = overdueReturn
                });
            }
        }
        if (adminNotification.Any())
        {
            var ldapAdmins = _ldapRepository.GetUsers().Where(u => u.Role == Roles.Admin).Select(u => u.LdapId).ToList();
            var admins =
                await _grandmaRepository.ListAsync(
                    new NotDeleted<MongoUser, Guid>().Combine(new UserByLdapId(ldapAdmins)));
            if (admins.Any())
            {
                var adminEmails = admins.Select(a => a.Email);
                var adminMattermosts = admins.Select(a => a.MattermostName);
                await _notifier.NotifyAdminsAboutOverduesByEmail(adminNotification, adminEmails);
                await _notifier.NotifyAdminsAboutOverduesByMattermost(adminNotification, adminMattermosts);
            }
        }
    }

    public void Dispose()
    {
        _scope.Dispose();
    }
}