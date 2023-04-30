using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using LdapConnector;

namespace GrandmaApi.Notification
{
    public interface INotifier
    {
        Task NotifySoonByEmail(IEnumerable<BookingInfoDto> devices, string address);
        Task NotifySoonByMattermost(IEnumerable<BookingInfoDto> devices, string address);
        Task NotifyOverdueByEmail(IEnumerable<BookingInfoDto> devices, string address);
        Task NotifyOverdueByMattermost(IEnumerable<BookingInfoDto> devices, string address);
        Task NotifyAdminsAboutOverduesByEmail(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses);
        Task NotifyAdminsAboutOverduesByMattermost(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses);
        Task NotifyAdminAboutDeletedUsersWithBookingsByEmail(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses);
        Task NotifyAdminAboutDeletedUsersWithBookingsByMattermost(IEnumerable<AdminNotificationDto> notifications, IEnumerable<string> adminAddresses);
    }
}