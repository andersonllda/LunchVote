using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using HMV.Core.Framework.Types;

namespace HMV.Core.Framework.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Retorna o valor do CustomDisplay de um Enum.
        /// </summary>
        /// <returns>string</returns>
        public static string GetEnumCustomDisplay<T>(this T self) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T deve ser do tipo ENUM");
            }
            return Enum<T>.GetCustomDisplayOf(self); 
        }

        /// <summary>
        /// Retorna o valor da Description de um Enum.
        /// </summary>
        /// <returns>string</returns>
        public static string GetEnumDescription<T>(this T self) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T deve ser do tipo ENUM");
            }
            return Enum<T>.GetDescriptionOf(self); 
        }

        
    }
}

