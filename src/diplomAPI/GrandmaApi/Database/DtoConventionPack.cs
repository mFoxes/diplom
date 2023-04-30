using MongoDB.Bson.Serialization.Conventions;

namespace GrandmaApi.Database
{
    public class DtoConventionPack : IConventionPack
    {
        private static readonly IConventionPack Pack = new DtoConventionPack();
        private DtoConventionPack() => Conventions = new List<IConvention>()
        {
            new ReadWriteMemberFinderConvention(),
            new NamedIdMemberConvention(),
            new NamedExtraElementsMemberConvention(new [] {"ExtraElements"}),
            new IgnoreExtraElementsConvention(false),
            new ImmutableTypeClassMapConvention(),
            new NamedParameterCreatorMapConvention(),
            new StringObjectIdIdGeneratorConvention(),
            new LookupIdGeneratorConvention()
        };
        public IEnumerable<IConvention> Conventions {get;}
        public static IConventionPack Instance => Pack;
    }
}