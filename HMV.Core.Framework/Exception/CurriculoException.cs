using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class CurriculoException : ApplicationException
    {
        private string CPF;

        public CurriculoException(System.Exception ex, string cpf)
            : base(ex.Message, ex)
        {
            CPF = cpf;
        }

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder("Não foi possivel fazer o download do arquivo de curriculo do profissional com CPF: " + CPF + "\n\r");
                sb.Append(new string('-', 10));
                sb.Append("\n\r");
                sb.Append(base.InnerException);
                return sb.ToString();
            }
        }
    }
}
