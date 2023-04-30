using DTO;
using GrandmaApi.Extensions;
using GrandmaApi.Notification.MessageServices;
using LdapConnector;
using Scriban;

namespace GrandmaApi.Notification.Notifiers
{
    public class AdminNotifierAboutDeletedUsers : IAdminNotifierAboutDeletedUsers
    {
        private readonly IMessageService _messageService;
        private readonly Template _messageTemplate;
        public AdminNotifierAboutDeletedUsers(IMessageService messageService, Template messageTemplate)
        {
            _messageService = messageService;
            _messageTemplate = messageTemplate;
        }

        public async Task NotifyAboutDeletedUsers(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses)
        {
            var message = _messageTemplate.RenderWithRenamer(notifications);
            foreach (var address in adminAddresses)
            {
                await _messageService.SendAsync(address, "Забронированные устройства удаленных пользователей", message);
            }
        }
    }
}