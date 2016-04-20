using System;
using System.Collections.Generic;

namespace HandyCollections.Extensions
{
    public static class IReadOnlyListExtensions
    {
        public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
        {
            for (var i = 0; i < list.Count; i++)
                if (predicate(list[i]))
                    return i;

            return -1;
        }
    }
}
