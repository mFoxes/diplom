using Domain.Models;
using Singularis.Internal.Domain.Specification;


namespace GrandmaApi.Database.Specifications
{
    public class BookingByUserId : Specification<BookingModel>
    {
        public IReadOnlyCollection<Guid> UserIds { get; }
        public BookingByUserId(IReadOnlyCollection<Guid> userIdList)
        {
            UserIds = userIdList;
            Query = Source().Where(b => b.UserId == null || userIdList.Contains(b.UserId.Value));
        }

        public BookingByUserId(Guid userId)
        {
            UserIds = new List<Guid>() { userId };
            Query = Source().Where(b => b.UserId == userId);
        }
    }
}