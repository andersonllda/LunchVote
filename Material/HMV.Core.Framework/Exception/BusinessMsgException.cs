using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Validator.Engine;

namespace HMV.Core.Framework.Exception
{
    public class BusinessMsgException : ApplicationException
    {
        protected IList<String> _erros;
        protected Object _MessageImage;

        public Object MessageImage
        {
            get
            {
                return _MessageImage;
            }
        }

        public IList<String> GetErros()
        {
            return _erros;
        }

        public BusinessMsgException(IList<String> perros, Object pMessageImage)
        {
            _erros = perros;
            _MessageImage = pMessageImage;
        }

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder("Falha de Regra(s) de Negocio:\r\n");
                sb.Append(new string('-', 10));
                sb.Append("\r\n");
                foreach (String iv in _erros)
                    sb.AppendLine(iv);         
                
                sb.Append(new string('-', 10));
                sb.Append("\r\n");
                return sb.ToString();
            }
        }
    }
}
