using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class ArquivoException : System.Exception
    {
        public ArquivoException()
            : base("Arquivo não encontrado")
        {            
        }
    }
}
