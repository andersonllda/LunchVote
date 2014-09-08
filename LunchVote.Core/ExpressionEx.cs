using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;  
    using System.Text;
    using System.Reflection;

    namespace HMV.Core.Framework.Expression
    {
        public static class ExpressionEx
        {
            public static String PropertyName<T>(Expression<Func<T, Object>> expression)
            {
                return typeof(T).PropertyName<T>(expression);
            }
            private static String PropertyName<T>(this Type type, Expression<Func<T, Object>> expression)
            {
                return MapPropertyName<T>(expression);
            }
            private static String PropertyName<T>(this Type type, params Expression<Func<T, Object>>[] expressions)
            {
                StringBuilder propertyName = new StringBuilder();
                foreach (Expression<Func<T, Object>> expression in expressions)
                    propertyName.Append(MapPropertyName<T>(expression));

                return propertyName.ToString();
            }
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
        }
    }
}
