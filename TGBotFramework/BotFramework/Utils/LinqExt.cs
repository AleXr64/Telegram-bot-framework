﻿using System;
using System.Collections.Generic;

namespace BotFramework.Utils
{
    public static class LinqExt
    {

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
                                                                     Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            if(keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            return _(); IEnumerable<TSource> _()
            {
                var knownKeys = new HashSet<TKey>(comparer);
                foreach(var element in source)
                {
                    if(knownKeys.Add(keySelector(element)))
                        yield return element;
                }
            }
        }
    }
}
