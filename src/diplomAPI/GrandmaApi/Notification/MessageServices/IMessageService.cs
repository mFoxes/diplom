using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandmaApi.Notification.MessageServices
{
    public interface IMessageService
    {
        Task SendAsync(string address, string subject, string message);
    }
}