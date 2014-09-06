using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace HMV.Core.Framework.Extensions
{
    /// <summary>
    /// Summary description for CollectionExtensions
    /// </summary>
    public static class CollectionExtensions
    {

        /// <summary>
        /// Faz o orderby ASC de uma Collection de acordo com a propriedade.
        /// </summary>
        /// <returns>ICollection</returns>
        public static void Sort<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        /// <summary>
        /// Faz o orderby DESC de uma Collection de acordo com a propriedade.
        /// </summary>
        /// <returns>ICollection</returns>
        public static void SortDesc<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        /// <summary>
        /// Faz o filtro de uma Collection de acordo com a propriedade.
        /// </summary>
        /// <returns>ICollection</returns>
        public static void FilterBy<TSource>(this Collection<TSource> source, Func<TSource, bool> predicate)
        {
            List<TSource> filteredList = source.Where(predicate).ToList();
            source.Clear();
            foreach (var filteredItem in filteredList)
                source.Add(filteredItem);
        }

        /// <summary>
        /// Converte Ilist to ObservableCollection
        /// </summary>
        /// <returns>ICollection</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> thisCollection)
        {
            if (thisCollection == null) return null;
            var oc = new ObservableCollection<T>();

            foreach (var item in thisCollection)
            {
                oc.Add(item);
            }

            return oc;
        }

    }
}