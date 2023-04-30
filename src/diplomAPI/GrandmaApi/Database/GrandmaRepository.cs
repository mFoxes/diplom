using Singularis.Internal.Domain.Services.Database;
using MongoDB.Driver;
using Singularis.Internal.Infrastructure.MongoDb;
namespace GrandmaApi.Database
{
    public class GrandmaRepository : Repository<Guid>, IGrandmaRepository
    {
        public GrandmaRepository(Func<IMongoDatabase> databaseResolver, ICollectionMapper collectionMapper, RepositorySettings settings) : base(databaseResolver, collectionMapper, settings)
        {
            
        }
    }
}