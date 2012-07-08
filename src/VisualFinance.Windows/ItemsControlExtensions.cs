using System.Collections.Generic;
using System.Windows.Controls;

namespace VisualFinance.Windows
{
    public static class ItemsControlExtensions
    {
        /// <summary>
        /// Safely returns all of the values from the Items property of the <paramref name="itemsControl"/> as their container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemsControl"></param>
        /// <returns></returns>
        public static IEnumerable<T> Children<T>(this ItemsControl itemsControl) where T : class
        {
            if (itemsControl.Items == null || itemsControl.Items.Count == 0)
                yield break;

            foreach (var item in itemsControl.Items)
            {
                var child = item as T ??
                            itemsControl.ItemContainerGenerator.ContainerFromItem(item) as T;

                if (child != null)
                    yield return child;
            }
        }
    }
}