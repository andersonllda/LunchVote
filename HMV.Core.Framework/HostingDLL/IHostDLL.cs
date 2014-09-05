using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.HostingDLL
{
    public interface IHostDLL
    {
        void Inicializa(object pParametros);
        void Show(object pParametros);
    }
}
