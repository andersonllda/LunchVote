using System;
using System.Collections.Generic;

namespace HMV.Core.Framework.Exception
{
    public class InvalidObjectException : ApplicationException
    {
        public InvalidObjectException(String Mensagem)
            : base(Mensagem)
        {

        }
    }
}
