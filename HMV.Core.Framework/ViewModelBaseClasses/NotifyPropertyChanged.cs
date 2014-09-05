using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;

namespace HMV.Core.Framework.ViewModelBaseClasses
{    
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        #region Propriedades Privadas
        private const string ERROR_MSG = "{0} não é uma propriedade privada de {1}";
        private static readonly object syncLock = new object();
        internal static Dictionary<string, PropertyChangedEventArgs> eventArgCache;
        #endregion

        #region Métodos Protegidos
        [Conditional("DEBUG")]
        private void VerifyProperty(string propertyName)
        {
            //http://www.codeproject.com/KB/WPF/podder1.aspx?msg=2381272#xx2381272xx

            bool propertyExists = TypeDescriptor.GetProperties(this).Find(propertyName, false) != null;
            if (!propertyExists)
            {
                //Se a propriedade nao puder ser encontrada, então alerta o problema.                
                string msg = string.Format(ERROR_MSG, propertyName, this.GetType().FullName);
                Debug.Fail(msg);
            }
        }
        #endregion

        /// <summary>        
        /// Dispara quando uma propriedade publica deste objeto é setada.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>        
        /// Retorna uma instancia do PropertyChangedEventArgs para uma propriedade específica.
        /// </summary>
        /// <param name="propertyName">        
        /// O nome da propriedade para criar o eventargs.
        /// </param>		
        public static PropertyChangedEventArgs GetPropertyChangedEventArgs(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException("A propriedade propertyName não pode ser nula ou vazia.");

            PropertyChangedEventArgs args;
            lock (ViewModelBase.syncLock)
            {
                if (!eventArgCache.TryGetValue(propertyName, out args))
                {
                    eventArgCache.Add(propertyName, args = new PropertyChangedEventArgs(propertyName));
                }
            }
            return args;
        }

        /// <summary>
        /// As classes derivadas desta, podem dar override neste método para executar
        /// alguma lógica após a propriedade 'x' ser setada.
        /// A implementação base não faz nada.        
        /// </summary>
        /// <param name="propertyName">
        /// O nome da propriedade alterada.
        /// </param>
        protected virtual void AfterPropertyChanged(string propertyName)
        {
        }

        /// <summary>
        /// Tenta disparar o evento PropertyChanged, e após chama o método virtual
        /// AfterPropertyChanged, independente do evento ser disparado ou não.      
        /// </summary>
        /// <param name="propertyName">
        /// O nome da propriedade alterada.
        /// </param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T, Object>> expression)
        {
            OnPropertyChanged(Expression.ExpressionEx.PropertyName(expression));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyProperty(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                // Pega o event args no cach.
                PropertyChangedEventArgs args = GetPropertyChangedEventArgs(propertyName);

                // Dispara o evento PropertyChanged.
                handler(this, args);
            }

            this.AfterPropertyChanged(propertyName);
        }
    }
}
