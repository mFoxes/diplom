using DTO;
using Microsoft.AspNetCore.SignalR;

namespace GrandmaApi.SignalR;

public class GrandmaHub : Hub<IGrandmaHub>
{
    public async Task Notify(BookingDto device)
    {
        await Clients.All.Notify(device);
    }
}