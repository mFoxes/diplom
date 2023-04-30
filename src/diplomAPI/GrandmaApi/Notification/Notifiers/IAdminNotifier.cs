using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using LdapConnector;

namespace GrandmaApi.Notification.Notifiers
{
    public interface IAdminNotifier
    {
        Task NotifyAdmins(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses);
    }
}