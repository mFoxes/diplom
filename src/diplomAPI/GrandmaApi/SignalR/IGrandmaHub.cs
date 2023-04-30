using DTO;

namespace GrandmaApi.SignalR;

public interface IGrandmaHub
{
    Task Notify(BookingDto device);
}