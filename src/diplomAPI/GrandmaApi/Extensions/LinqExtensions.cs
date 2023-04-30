using GrandmaApi.Models.Enums;

namespace GrandmaApi.Extensions;

public static class EnumerableExtensions 
{
    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, 
                                                                          Func<TSource, TKey> keySelector,
                                                                          OrderDirections orderDirection)
    {
        return orderDirection == OrderDirections.Asc
            ? source.OrderBy(keySelector)
            : source.OrderByDescending(keySelector);
    }
    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source,
        Func<TSource, TKey> keySelector, OrderDirections orderDirection)
    {
        return orderDirection == OrderDirections.Asc
            ? source.ThenBy(keySelector)
            : source.ThenByDescending(keySelector);
    }
}