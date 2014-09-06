using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class AutorizacaoClinicaException : System.Exception
    {
        public AutorizacaoClinicaException(string user, int clinica)
            : base("Usuário não possui acesso a clinica: Usuário:" + user + " Clinica: " + clinica)
        {
        }
    }
}
