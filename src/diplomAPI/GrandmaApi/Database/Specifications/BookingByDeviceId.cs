using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class BookingByDeviceId : Specification<BookingModel>
    {
        public readonly Guid DeviceId;
        public BookingByDeviceId(Guid deviceId)
        {
            DeviceId = deviceId;
            Query = Source().Where(d => d.DeviceId == deviceId);
        }
        public BookingByDeviceId(IEnumerable<Guid> deviceIdList)
        {
            Query = Source().Where(d => deviceIdList.Contains(d.DeviceId));
        }
    }
}