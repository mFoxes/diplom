using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class ActiveBookingHistoryByDeviceId : Specification<BookingHistory>
    {
        public readonly Guid DeviceId;
        public ActiveBookingHistoryByDeviceId(Guid deviceId)
        {
            DeviceId = deviceId;

            Query = Source().Where(bh => bh.ReturnedAt.Value == null && bh.DeviceId == deviceId);
        }
    }
}