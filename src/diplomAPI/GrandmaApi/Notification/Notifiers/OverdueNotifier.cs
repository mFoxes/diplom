using DTO;
using GrandmaApi.Extensions;
using GrandmaApi.Notification.MessageServices;
using Scriban;

namespace GrandmaApi.Notification.Notifiers
{
    public class OverdueNotifier : IOverdueNotifier
    {
        private readonly IMessageService _messageService;
        private readonly Template _messageTemplate;
        public OverdueNotifier(IMessageService messageService, Template messageTemplate)
        {
            _messageService = messageService;
            _messageTemplate = messageTemplate;
        }

        public async Task NotifyOverdue(IEnumerable<BookingInfoDto> devices, string address)
        {
            var message = _messageTemplate.RenderWithRenamer(devices);
            await _messageService.SendAsync(address, "Уведомление о просроченном возврате", message);
        }
    }
}