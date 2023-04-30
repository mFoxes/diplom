using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications;

public class BookingHistoryByDeviceId : Specification<BookingHistory>
{
    public BookingHistoryByDeviceId(Guid deviceId)
    {
        Query = Source().Where(bh => bh.DeviceId == deviceId);
    }
}