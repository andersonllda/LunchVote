using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows;

namespace HMV.Core.Framework.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Verifica se o BaseType enviado faz parte da árvore de basetypes do type.
        /// </summary>
        /// <returns>bool</returns>
        public static bool HasBaseType(this Type type, Type BaseType)
        {
            return ((from tys in type.GetBaseTree() where tys.BaseType == BaseType || tys == BaseType select tys).Count() > 0);
        }

        /// <summary>
        /// Pega toda a árvore recursiva de basetypes do type.
        /// </summary>
        /// <returns>IList</returns>
        public static IList<Type> GetBaseTree(this Type type)
        {
            if (type == null)
                return null;
            List<Type> result = new List<Type>();
            Type baseType = type.BaseType;
            do
            {
                result.Add(baseType);
                if (!baseType.BaseType.IsNull())
                    baseType = baseType.BaseType;
            } while (baseType != typeof(object));
            return result;
        }

        public static object FindAncestor(this DependencyObject Element, Type DesiredType)
        {
            DependencyObject e = Element;
            while (!(e.GetType() == DesiredType))
            {
                e = VisualTreeHelper.GetParent(e);
            }
            return e;
        }
    }
}
