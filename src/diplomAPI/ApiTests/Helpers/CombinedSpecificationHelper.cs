using System;
using System.Linq;
using Singularis.Internal.Domain.Specification;

namespace ApiTests.Helpers;

public static class CombinedSpecificationHelper
{
    public static bool Validate(this ICombinedSpecification s, params Func<ISpecification, bool>[] validators)
    {
        if (s.Parts.Count != validators.Length)
            return false;

        var isValid = s.Parts
            .Zip(validators, (p, v) => new {Specification = p, Validator = v})
            .Select(x => x.Validator(x.Specification))
            .Aggregate(true, (a, b) => a & b);

        return isValid;
    }
        
    public static bool CheckSpecification<TSpecifciation>(this ISpecification s, Func<TSpecifciation, bool> validator)
        where TSpecifciation : ISpecification
    {
        if (s is TSpecifciation typedSpecification)
            return validator(typedSpecification);

        return false;
    }
        
    public static bool CheckSpecification<TSpecifciation>(this ISpecification s)
        where TSpecifciation : ISpecification
    {
        return s is TSpecifciation;
    }
}