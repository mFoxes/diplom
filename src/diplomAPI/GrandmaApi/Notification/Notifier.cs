using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using GrandmaApi.Notification.MessageServices;
using GrandmaApi.Notification.Notifiers;
using LdapConnector;

namespace GrandmaApi.Notification
{
    public class Notifier : INotifier
    {
        private readonly IEmailMessageService _emailMessageService;
        private readonly IMattermostMessageService _mattermostMessageService;
        private readonly TemplateProvider _provider;
        public Notifier(IEmailMessageService emailMessageService, 
                        IMattermostMessageService mattermostMessageService, 
                        TemplateProvider provider)
        {
            _emailMessageService = emailMessageService;
            _mattermostMessageService = mattermostMessageService;
            _provider = provider;
        }
        public async Task NotifyAdminAboutDeletedUsersWithBookingsByEmail(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses)
        {
            var emailTemplate = _provider.GetDeletedUsersTemplateForEmail();

            var emailNotifier = new AdminNotifierAboutDeletedUsers(_emailMessageService, emailTemplate);
            
            await emailNotifier.NotifyAboutDeletedUsers(notifications, adminAddresses);
        }

        public async Task NotifyAdminAboutDeletedUsersWithBookingsByMattermost(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses)
        {
            var template = _provider.GetDeletedUsersTemplateForMattermost();

            var notifier = new AdminNotifierAboutDeletedUsers(_mattermostMessageService, template);
            
            await notifier.NotifyAboutDeletedUsers(notifications, adminAddresses);
        }

        public async Task NotifyAdminsAboutOverduesByEmail(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses)
        {
            var template = _provider.GetOverdueAdminsTemplateForEmail();

            var notifier = new AdminNotifier(_emailMessageService, template);
            
            await notifier.NotifyAdmins(notifications, adminAddresses);
        }

        public async Task NotifyAdminsAboutOverduesByMattermost(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses)
        {
            var template = _provider.GetOverdueAdminsTemplateForMattermost();

            var notifier = new AdminNotifier(_mattermostMessageService, template);
            
            await notifier.NotifyAdmins(notifications, adminAddresses);
        }

        public async Task NotifyOverdueByEmail(IEnumerable<BookingInfoDto> devices, string address)
        {
            var emailTemplate = _provider.GetOverdueTemplateForEmail();

            var emailNotifier = new OverdueNotifier(_emailMessageService, emailTemplate);
            
            await emailNotifier.NotifyOverdue(devices, address);
        }

        public async Task NotifyOverdueByMattermost(IEnumerable<BookingInfoDto> devices, string address)
        {
            var template = _provider.GetOverdueTemplateForMattermost();

            var notifier = new OverdueNotifier(_mattermostMessageService, template);
            
            await notifier.NotifyOverdue(devices, address);
        }

        public async Task NotifySoonByEmail(IEnumerable<BookingInfoDto> devices, string address)
        {
            var template = _provider.GetSoonTemplateForEmail();

            var notifier = new SoonNotifier(_emailMessageService, template);

            await notifier.NotifySoon(devices, address);
        }

        public async Task NotifySoonByMattermost(IEnumerable<BookingInfoDto> devices, string address)
        {
            var template = _provider.GetSoonTemplateForMattermost();

            var notifier = new SoonNotifier(_mattermostMessageService, template);

            await notifier.NotifySoon(devices, address);
        }
    }
}