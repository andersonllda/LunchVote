using System.Collections;
using System.ComponentModel;

namespace HMV.Core.Framework.Extensions
{
    public static class HashtableExtensions
    {
        /// <summary>
        /// Verifica se o Campo enviado esta dentro do Hashtable e devolve seu valor.
        /// </summary>
        /// <returns>bool</returns>
        public static T Valor<T>(this Hashtable self, string campo)
        {
            object valor = null;

            if (self.Contains(campo) && self[campo] != null)
                valor = self[campo];

            if (valor == null)
                return default(T);

            TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
            var resultado = (T)conv.ConvertFrom(valor.ToString());

            return resultado;
        }
    }
}
