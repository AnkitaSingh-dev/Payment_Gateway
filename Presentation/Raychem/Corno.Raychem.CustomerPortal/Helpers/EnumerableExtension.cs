using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Corno.Raychem.CustomerPortal.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static ICollection<T> EnsureNotNull<T>(this ICollection<T> source)
        {
            return source ?? new Collection<T>();
        }
    }
}