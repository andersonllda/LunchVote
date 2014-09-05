using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Linq.Expressions;
using HMV.Core.Framework.Extensions;

namespace HMV.Core.Framework.Helper
{
    public class StringHelper
    {
        public static string getString<T>(object objeto, Expression<Func<T, Object>> expression)
        {
            string value = string.Empty;
            string property = Expression.ExpressionEx.PropertyName<T>(expression);
            if (objeto != null)
            {
                object valueObject = objeto.GetType().GetProperty(property).GetValue(objeto, null);
                if (valueObject != null)
                {
                    if (objeto.GetType().GetProperty(property).GetType() == typeof(DateTime))
                        value = ((DateTime)valueObject).ToShortDateString();
                    else
                        value = valueObject.ToString();
                }
            }
            return value;
        }

        public static string Append<T>(object objeto, Expression<Func<T, Object>> expression)
        {
            string ret = string.Empty;
            return ret.Append<T>(objeto, expression);
        }

        public static string Append<T>(string textoLeft, object objeto, Expression<Func<T, Object>> expression)
        {
            string ret = string.Empty;
            return ret.Append<T>(textoLeft, objeto, expression);
        }

        public static string Append<T>(object objeto, Expression<Func<T, Object>> expression, string textoRight)
        {
            string ret = string.Empty;
            return ret.Append<T>(objeto, expression, textoRight);
        }

    }
}
