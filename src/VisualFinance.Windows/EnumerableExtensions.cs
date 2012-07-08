using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace VisualFinance.Windows
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Return<T>(T element)
        {
            yield return element;
        }

        public static IEnumerable<T> StartWith<T>(this IEnumerable<T> list, T element)
        {
            return Return(element).Concat(list);
        }

        public static IEnumerable<T> Flatten<T>(this T element, Func<T, IEnumerable<T>> childSelector)
        {
            return childSelector(element).SelectMany(child => child.Flatten(childSelector))
                .StartWith(element);
        }

        public static int FindLastIndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var lastMatchIndex = -1;
            var idx = 0;
            foreach (var item in source)
            {
                if (predicate(item)) lastMatchIndex = idx;
                idx++;
            }
            return lastMatchIndex;
        }

        public static ReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source)
        {
            var copy = source.ToList();
            return new ReadOnlyCollection<T>(copy);
        }
    }
}