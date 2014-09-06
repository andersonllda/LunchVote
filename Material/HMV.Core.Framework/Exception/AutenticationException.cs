using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class AutenticationException : System.Exception
    {
        public AutenticationException(string user)
            : base("Usuário ou senha inválidos, Usuário: " + user )
        {
        }
    }
}
