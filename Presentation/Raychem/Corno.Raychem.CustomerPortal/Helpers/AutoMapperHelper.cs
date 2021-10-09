using System.Collections.Generic;
using AutoMapper;
using PagedList;

namespace Corno.Raychem.CustomerPortal.Helpers
{
    public static class AutoMapperExtensions
    {
        public static IPagedList<TDestination> ToMappedPagedList<TSource, TDestination>(this IPagedList<TSource> list)
        {
            var sourceList = Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            IPagedList<TDestination> pagedResult = new StaticPagedList<TDestination>(sourceList.EnsureNotNull(),
                list.GetMetaData());
            return pagedResult;
        }

        public static List<TDestination> ToMappedList<TSource, TDestination>(this List<TSource> list)
        {
            var sourceList = Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            var result = new List<TDestination>(sourceList.EnsureNotNull());
            return result;
        }

        public static IEnumerable<TDestination> ToMappedList<TSource, TDestination>(this IEnumerable<TSource> list)
        {
            var sourceList = Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(list);
            IEnumerable<TDestination> result = new List<TDestination>(sourceList.EnsureNotNull());
            return result;
        }
    }
}