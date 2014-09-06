using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class AutorizacaoSistemaException : System.Exception
    {
        public AutorizacaoSistemaException(string user, int sistema)
            : base("Usuario não possui acesso ao sistema: Usuario:" + user + " Sistema: " + sistema)
        {
        }
    }
}
