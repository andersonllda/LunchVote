using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Validator.Engine;

namespace HMV.Core.Framework.Exception
{
    public class FriendlyBusinessValidatorException : BusinessValidatorException
    {
        public FriendlyBusinessValidatorException(params InvalidValue[] perros)
            : base(perros)
        {
        }

        public override string Message
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (InvalidValue iv in _erros)
                {
                    sb.Append(iv.Message);
                    sb.Append("\r\n");
                }

                return sb.ToString();
            }
        }
    }
}
