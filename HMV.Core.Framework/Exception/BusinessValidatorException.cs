using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Validator.Engine;

namespace HMV.Core.Framework.Exception
{

    public class BusinessValidatorException : ApplicationException
    {
        protected List<InvalidValue> _erros;

        public bool HasErrors()
        {
            return _erros.Count > 0;
        }

        public InvalidValue[] GetErros()
        {
            return _erros.ToArray();
        }

        public void AddErros(params InvalidValue[] erros)
        {
            _erros.AddRange(erros);
        }

        public BusinessValidatorException()
        {
            _erros = new List<InvalidValue>();
        }

        public BusinessValidatorException(params InvalidValue[] perros)
        {
            _erros = new List<InvalidValue>();
            _erros.AddRange(perros);
        }

        public BusinessValidatorException(string[] erros)
        {
            _erros = new List<InvalidValue>();
            foreach (var item in erros)
                _erros.Add(new InvalidValue(item, typeof(string), "", null, null, new object[] { Tag.Error }));
        }

        public void AddErros(params string[] erros)
        {
            foreach (var item in erros)
                _erros.Add(new InvalidValue(item, typeof(string), "", null, null, new object[] { Tag.Error }));
        }

        public string Erros
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (InvalidValue iv in _erros)
                    sb.AppendFormat(iv.Message);

                return sb.ToString();
            }
        }

        public string ErrosLine
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (InvalidValue iv in _erros)
                    sb.Append(iv.Message + System.Environment.NewLine);

                return sb.ToString();
            }
        }

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder("Falha de Regra(s) de Negocio:\r\n");
                sb.Append(new string('-', 10));
                sb.Append("\r\n");
                foreach (InvalidValue iv in _erros)
                    sb.AppendFormat("Campo: {0} \r\nValor: {1}\r\nMensagem: {2}\r\n\r\n", iv.PropertyName, iv.Value ?? "", iv.Message);



                sb.Append(new string('-', 10));
                sb.Append("\r\n");
                return sb.ToString();
            }
        }

        public string MessageWeb
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (InvalidValue iv in _erros)
                    sb.AppendFormat("{0}<br/>", iv.Message);

                return sb.ToString();
            }
        }
    }
}
