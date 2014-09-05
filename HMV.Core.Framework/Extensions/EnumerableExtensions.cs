using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HMV.Core.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Faz o distinct de uma lista Enumerable selecionando a propriedade a ser comparada.
        /// </summary>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Faz o ForEach diretamente na Lista.
        /// </summary>        
        [DebuggerStepThrough]
        public static void Each<T>(this IEnumerable<T> list, Action<T, int> act)
        {
            int counter = 0;
            foreach (T item in list)
                act(item, counter++);
        }

        /// <summary>
        /// Faz o ForEach diretamente na Lista.
        /// </summary>
        [DebuggerStepThrough]
        public static void Each<T>(this IEnumerable<T> list, Action<T> act)
        {
            foreach (T item in list)
                act(item);
        }

        /// <summary>
        /// Verifica se a lista é nula e se tem itens
        /// </summary>
        /// <returns>bool</returns>
        [DebuggerStepThrough]
        public static bool HasItems<T>(this IEnumerable<T> list)
        {
            return (list != null) && (list.Count() > 0);
        }

        /// <summary>
        /// Retorna o Index de um IEnumerable
        /// </summary>
        /// <returns>int</returns>
        public static int ReturnIndexCollection<T>(this IEnumerable<T> enumerable, object pObject)
        {
            int idx = -1;
            foreach (T item in enumerable)
            {
                idx++;
                if (item.Equals(pObject))
                    return idx;
            }
            return idx;
        }

        /// <summary>
        /// Concatena cada ToString() de um IEnumerable[string] e retorna.
        /// </summary>
        /// <returns>string</returns>
        public static string ReturnStringCollection<T>(this IList<T> list, string separador = "NewLine")
        {
            if (separador == "NewLine") separador = Environment.NewLine;
            string ret = string.Empty;
            list.ToList().ForEach(x => ret += x.ToNullSafeString() + separador);
            return ret.TrimEnd(Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Clona um objeto IEnumerable
        /// </summary>
        /// <returns>IEnumerable</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        /// <summary>
        /// Retorna o Tipo do Item da Coleção IEnumerable
        /// </summary>
        /// <returns>Type</returns>
        public static Type GetItemType<T>(this IEnumerable<T> someCollection)
        {
            var type = someCollection.GetType();
            var ienum = type.GetInterface(typeof(IEnumerable<>).Name);
            return ienum != null
              ? ienum.GetGenericArguments()[0]
              : null;
        }

        /// <summary>
        /// Converte um array de bytes para uma string.
        /// </summary>
        /// <returns>string</returns>
        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /// <summary>
        /// Ordena lista ASC pelo nome do campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        /// <summary>
        /// Ordena lista DESC pelo nome do campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        /// <summary>
        /// Proximo campo na ordenacao ASC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        /// <summary>
        /// Proximo campo na ordenacao DESC
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = System.Linq.Expressions.Expression.Parameter(type, "x");
            System.Linq.Expressions.Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = System.Linq.Expressions.Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}
