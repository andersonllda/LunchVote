using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using NHibernate.Type;
using NHibernate;
using System.Data;

namespace HMV.Core.Framework.Types
{
    #region UserTypes
    /// <summary>
    /// <c>NHibernate</c> type representing an enum that should be persisted using it's
    /// <see cref="DescriptionAttribute"/> values
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    [Serializable]
    public class HMVCustomType<T> : EnumStringType
        where T : struct
    {
        /// <summary>
        /// <c>NHibernate</c> type representing an enum that should be persisted using it's
        /// <see cref="DescriptionAttribute"/> values
        /// </summary>
        public HMVCustomType()
            : base(typeof(T))
        {
        }


        public override object GetInstance(object code)
        {
            try
            {
                return FromString(code as string);
            }
            catch (ArgumentOutOfRangeException ae)
            {
                throw new HibernateException(string.Format("Can't Parse {0} as [{1}]", code, typeof(T)), ae);
            }
        }

        public override object GetValue(object code)
        {
            return GetDescriptionOf((T)code);
        }

        public override void Set(IDbCommand cmd, object value, int index)
        {
            var par = (IDataParameter)cmd.Parameters[index];
            if (value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                par.Value = GetDescriptionOf((T)value);
            }
        }

        static object FromString(string value)
        {
            return Enum<T>.From(value);
        }

        static string GetDescriptionOf(T value)
        {
            return Enum<T>.GetDescriptionOf(value);
        }

    }
    #endregion

    /// <summary>
    /// Provide access to enum helpers
    /// </summary>
    public static class Enum<T>
    {
        private static readonly Type enumType;
        private static readonly DescribedEnumHandler<T> handler;
        private static readonly DisplayedEnumHandler<T> handlerDisplay;

        static Enum()
        {
            enumType = typeof(T);
            if (enumType.IsEnum == false)
                throw new ArgumentException(string.Format("{0} is not an enum", enumType));

            handler = new DescribedEnumHandler<T>();
            handlerDisplay = new DisplayedEnumHandler<T>();
        }

        /// <summary>
        /// Extract the description for a given enum value
        /// </summary>
        /// <param name="value">An enum value</param>
        /// <returns>It's description, or it's name if there's no registered description for the given value</returns>
        public static string GetDescriptionOf(T value)
        {
            return handler.GetDescriptionFrom(value);
        }

        public static string GetCustomDisplayOf(T value)
        {
            return handlerDisplay.GetDisplayFrom(value);
        }

        public static string GetCustomDisplayOf(string descriptionOrName)
        {
            return handlerDisplay.GetDisplayFrom(From(descriptionOrName));
        }
        /// <summary>
        /// Gets the enum value for a given description or value
        /// </summary>
        /// <param name="descriptionOrName">The enum value or description</param>
        /// <returns>An enum value matching the given string value, as description (using <see cref="DescriptionAttribute"/>) or as value</returns>
        public static T From(string descriptionOrName)
        {
            return handler.GetValueFrom(descriptionOrName);
        }

        public static List<String> GetDescriptions()
        {
            List<String> descriptions = new List<string>();
            foreach (T item in Enum.GetValues(typeof(T)))
                descriptions.Add(Enum<T>.GetDescriptionOf(item));

            return descriptions;
        }

        public static List<T> GetAll()
        {
            List<T> descriptions = new List<T>();
            foreach (T item in Enum.GetValues(typeof(T)))
                descriptions.Add(item);

            return descriptions;
        }

        public static List<String> GetCustomDisplay()
        {
            List<String> descriptions = new List<string>();
            foreach (T item in Enum.GetValues(typeof(T)))
                descriptions.Add(Enum<T>.GetCustomDisplayOf(item));

            return descriptions;
        }

        public static string GetDescriptionOfCustomDisplay(string customDisplay)
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (GetCustomDisplayOf(item) == customDisplay)
                    return GetDescriptionOf(item);
            //if (item == customDisplay)
                
            
            }
            return string.Empty;
        
        }
    }

    /// <summary>
    /// Used to cache enum values descriptions mapper
    /// </summary>
    internal class DescribedEnumHandler<T>
    {
        private readonly IDictionary<T, string> toDescription = new Dictionary<T, string>();
        private readonly IDictionary<string, T> fromDescription = new Dictionary<string, T>();

        private const BindingFlags PUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Creates a new DescribedEnumHandler instance for a given enum type
        /// </summary>
        public DescribedEnumHandler()
        {
            var type = typeof(T);
            var enumEntrys = from f in type.GetFields(PUBLIC_STATIC)
                             let attributes = f.GetCustomAttributes(typeof(DescriptionAttribute), false)
                             let description =
                                attributes.Length == 1
                                    ? ((DescriptionAttribute)attributes[0]).Description
                                    : f.Name
                             select new
                             {
                                 Value = (T)Enum.Parse(type, f.Name),
                                 Description = description
                             };

            foreach (var enumEntry in enumEntrys)
            {
                toDescription[enumEntry.Value] = enumEntry.Description;
                fromDescription[enumEntry.Description] = enumEntry.Value;
            }
        }

        /// <summary>
        /// Extracts the description for the given enum value.
        /// <remarks>if no description was set for the given value, it's name will be retrieved</remarks>
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The value's description</returns>
        public string GetDescriptionFrom(T value)
        {
            return toDescription[value];
        }

        /// <summary>
        /// Parse the given string and return the enum value for with the given string acts as description
        /// </summary>
        /// <param name="description">The given description</param>
        /// <returns>A matching enum value</returns>
        public T GetValueFrom(string description)
        {
            return fromDescription[description.Trim()];
        }
    }
    
    internal class DisplayedEnumHandler<T>
    {
        private readonly IDictionary<T, string> toDisplay = new Dictionary<T, string>();
        private readonly IDictionary<string, T> fromDisplay = new Dictionary<string, T>();

        private const BindingFlags PUBLIC_STATIC = BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Creates a new DescribedEnumHandler instance for a given enum type
        /// </summary>
        public DisplayedEnumHandler()
        {
            var type = typeof(T);
            var enumEntrys = from f in type.GetFields(PUBLIC_STATIC)
                             let attributes = f.GetCustomAttributes(typeof(CustomDisplay), false)
                             let description =
                                attributes.Length == 1
                                    ? ((CustomDisplay)attributes[0]).ToString()
                                    : f.Name
                             select new
                             {
                                 Value = (T)Enum.Parse(type, f.Name),
                                 Description = description
                             };

            foreach (var enumEntry in enumEntrys)
            {
                toDisplay[enumEntry.Value] = enumEntry.Description;
                fromDisplay[enumEntry.Description] = enumEntry.Value;
            }
        }

        /// <summary>
        /// Extracts the description for the given enum value.
        /// <remarks>if no description was set for the given value, it's name will be retrieved</remarks>
        /// </summary>
        /// <param name="value">The enum value</param>
        /// <returns>The value's description</returns>
        public string GetDisplayFrom(T value)
        {
            return toDisplay[value];
        }

        /// <summary>
        /// Parse the given string and return the enum value for with the given string acts as description
        /// </summary>
        /// <param name="description">The given description</param>
        /// <returns>A matching enum value</returns>
        public T GetValueFrom(string description)
        {
            return fromDisplay[description];
        }
    }
}
