using System.Collections.Generic;
using System.Linq;

namespace CustomFilters.Shared;

internal static class IEnumerableExtensions
{
    internal static string JoinAsString<T>(this IEnumerable<T> @this) where T : class
    {
        return string.Join(", ", @this.Select(x => x.ToString()).ToArray());
    }
}