using System;
using System.Collections.Generic;
using System.Linq;

namespace myCycle.Controllers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<K> Squeeze<T,K>(this IEnumerable<T> sources, Func<T,T,K> squeezeFunc)
        {
            var list = sources.ToList();
            for(var index = 0;index<list.Count-1;index++)
            {
                var current = list[index];
                var next = list[index + 1];
                yield return squeezeFunc(next, current);
            }
        }
    }
}