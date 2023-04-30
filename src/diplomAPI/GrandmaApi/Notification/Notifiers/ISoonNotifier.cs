using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;

namespace GrandmaApi.Notification.Notifiers
{
    public interface ISoonNotifier
    {
        Task NotifySoon(IEnumerable<BookingInfoDto> devices, string address);
    }
}