using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class AutorizacaoUsuarioSemPrestadorException : System.Exception
    {
        public AutorizacaoUsuarioSemPrestadorException(string user)
            : base("Usuário não possui prestador vinculado: Usuário:" + user)
        {
        }
    }
}
