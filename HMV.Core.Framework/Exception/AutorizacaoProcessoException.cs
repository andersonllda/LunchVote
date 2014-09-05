using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class AutorizacaoProcessoException : System.Exception
    {
        public AutorizacaoProcessoException(string user, int sistema)
            : base("Usuário não possui acesso a nenhum processo: Usuário:" + user + " Sistema: " + sistema)
        {
        }
    }
}
