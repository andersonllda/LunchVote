using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Criterion;
using System.Text;
using System.Reflection;

namespace HMV.Core.Framework.Expression
{
    public static class ExpressionEx
    {     
        public static AbstractCriterion Like(string propertyName, string value)
        {
            MatchMode mode = MatchMode.Anywhere;

            if (value.IndexOf("%") != -1)
                mode = MatchMode.Exact;
            else
                mode = MatchMode.Anywhere;

            return NHibernate.Criterion.Restrictions.Like(propertyName, value, mode);
        }

        public static AbstractCriterion InsensitiveLike(string propertyName, string value)
        {
            MatchMode mode = MatchMode.Anywhere;

            if (value.IndexOf("%") != -1)
                mode = MatchMode.Exact;
            else
                mode = MatchMode.Anywhere;

            return NHibernate.Criterion.Restrictions.InsensitiveLike(propertyName, value, mode);
        }

        public static String PropertyName<T>(Expression<Func<T, Object>> expression)
        {
            return typeof(T).PropertyName<T>(expression);
        }

        /// <summary>  
        /// Takes a type and returns the operands name 
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="type">The type.</param>  
        /// <param name="expression">The expression.</param>  
        /// <returns></returns>  
        private static String PropertyName<T>(this Type type, Expression<Func<T, Object>> expression)
        {
            return MapPropertyName<T>(expression);
        }

        /// <summary>  
        /// Takes a type and returns the operands name 
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="type">The type.</param>  
        /// <param name="expressions">The expressions.</param>  
        /// <returns></returns>  
        private static String PropertyName<T>(this Type type, params Expression<Func<T, Object>>[] expressions)
        {
            StringBuilder propertyName = new StringBuilder();
            foreach (Expression<Func<T, Object>> expression in expressions)
                propertyName.Append(MapPropertyName<T>(expression));

            return propertyName.ToString();
        }

        /// <summary>  
        /// Maps the name of the property. 
        /// </summary>  
        /// <param name="expression">The expression.</param>  
        /// <returns></returns>  
        private static String MapPropertyName<T>(Expression<Func<T, Object>> expression)
        {
            LambdaExpression lambdaExpression = (LambdaExpression)expression;
            MemberExpression memberExpression;
            if (lambdaExpression.Body is UnaryExpression)
                memberExpression = (MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand;
            else
                memberExpression = (MemberExpression)lambdaExpression.Body;

            return ((PropertyInfo)memberExpression.Member).Name;
        }

        /// <summary>
        /// Combines the specified strings.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="val2">The val2.</param>
        /// <returns></returns>
        public static String Combine(this string val, string val2)
        {
            return String.Format("{0}{1}", val, val2);
        }

        public static String CombineAndNovaLinha(this string val, string val2)
        {
            return String.Format("{0}{1}", val, val2 + Environment.NewLine);
        }

        public static String NovaLinha(this string val)
        {
            return String.Format("{0}", val + Environment.NewLine);
        }

        /// <summary>
        /// Combines the specified strings if first string not empty.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="val2">The val2.</param>
        /// <returns></returns>
        public static String CombineNotEmpty(this string val, string val2)
        {
            return string.IsNullOrWhiteSpace(val) ? val : val.Combine(val2);
        }

        /// <summary>
        /// Combines seperates the specified strings.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="val2">The val2.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns></returns>
        public static String CombineAndSeperate(this string val, string val2, string seperator)
        {
            return String.Format("{0}{1}{2}", val, seperator, val2);
        }

        /// <summary>
        /// Combines seperates the specified strings with dot for queries.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="val2">The val2.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns></returns>
        public static String CombineAndSeperateInQueries(this string val, string val2)
        {
            return String.Format("{0}{1}{2}", val, ".", val2);
        }

        /// <summary>
        /// Combines seperates the specified strings with dot for queries.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="val2">The val2.</param>
        /// <param name="seperator">The seperator.</param>
        /// <returns></returns>
        public static String CombineAliasIQuery(this string val, string val2)
        {
            return String.Format("{0} as \"{1}\"", val, val2);
        }

        public static DateTime ToDateTimeValue(this string val)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo("pt-BR", true);
            return DateTime.Parse(val, culture, System.Globalization.DateTimeStyles.AssumeLocal);
        }
    }

}


