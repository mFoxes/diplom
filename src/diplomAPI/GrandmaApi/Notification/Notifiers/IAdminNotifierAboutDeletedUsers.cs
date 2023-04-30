using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using LdapConnector;

namespace GrandmaApi.Notification.Notifiers
{
    public interface IAdminNotifierAboutDeletedUsers
    {
        Task NotifyAboutDeletedUsers(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses);
    }
}