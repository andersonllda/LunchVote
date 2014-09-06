using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HMV.Core.Framework.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Copia o objeto (serializado) por valor e não por referência.
        /// </summary>
        /// <returns>T</returns>
        public static T DeepClone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                return source;
            }

            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Converte um objeto em outro tipo, caso a conversão não possa ser feita retorna o valor default de T.
        /// </summary>
        /// <returns>T</returns>
        public static T CastSafeTo<T>(this object target)
        {
            try
            {
                return (T)target;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Verifica se o Objeto é nulo.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsNull(this object obj)
        {
            return (obj == null);
        }

        /// <summary>
        /// Verifica se o Objeto NÃO é nulo.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsNotNull(this object obj)
        {
            return !(obj == null);
        }

        /// <summary>
        /// Retorna uma string com as propriedades do objeto.
        /// </summary>
        /// <returns>string</returns>
        public static string GetPropertiesInfo(this object instance)
        {
            Type instanceType = instance.GetType();
            IEnumerable<PropertyInfo> readableProperties = instanceType.GetProperties().Where(property => property.CanRead);
            List<string> properties = new List<string>();

            foreach (var item in readableProperties)
            {
                string name = item.Name;
                string value = item.GetValue(instance, null) + "";
                properties.Add(string.Format("{0} = {1}", item.Name, value));
            }

            string output = string.Format("{0} : [{1}] ", instanceType.Name, string.Join(",", properties.ToArray()));

            return output;
        }
    }
}

