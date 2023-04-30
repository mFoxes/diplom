using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandmaApi.Notification.MessageServices;
using Scriban;
using GrandmaApi.Extensions;
using DTO;

namespace GrandmaApi.Notification.Notifiers
{
    public class SoonNotifier : ISoonNotifier
    {
        private readonly IMessageService _messageService;
        private readonly Template _messageTemplate;
        public SoonNotifier(IMessageService messageService, Template messageTemplate)
        {
            _messageService = messageService;
            _messageTemplate = messageTemplate;
        }

        public async Task NotifySoon(IEnumerable<BookingInfoDto> devices, string address)
        {
            var message = _messageTemplate.RenderWithRenamer(devices);
            await _messageService.SendAsync(address, "Уведомление о скором возврате", message);
        }
    }
}