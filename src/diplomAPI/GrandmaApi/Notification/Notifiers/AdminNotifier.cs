using GrandmaApi.Extensions;
using DTO;
using GrandmaApi.Notification.MessageServices;
using LdapConnector;
using Scriban;

namespace GrandmaApi.Notification.Notifiers
{
    public class AdminNotifier : IAdminNotifier
    {
        private readonly IMessageService _messageService;
        private readonly Template _messageTemplate;
        public AdminNotifier(IMessageService messageService, Template messageTemplate)
        {
            _messageService = messageService;
            _messageTemplate = messageTemplate;
        }

        public async Task NotifyAdmins(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses)
        {
            var message = _messageTemplate.RenderWithRenamer(notifications);
            foreach (var address in adminAddresses)
            {
                await _messageService.SendAsync(address, "Уведомление о просроченных возвратах", message);
            }
        }
    }
}