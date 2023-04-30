using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;

namespace GrandmaApi.Notification.Notifiers
{
    public interface IOverdueNotifier
    {
        Task NotifyOverdue(IEnumerable<BookingInfoDto> devices, string address);
    }
}