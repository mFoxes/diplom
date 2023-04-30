using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class OverdueBookingsByUserId : Specification<BookingModel>
    {
        public Guid UserId {get;}
        public OverdueBookingsByUserId(Guid userId)
        {
            UserId = userId;
            var dateUtc = DateTime.UtcNow.Date;
            Query = Source().Where(b => b.UserId == userId && b.ReturnAt.Value < dateUtc);
        }
    }
}