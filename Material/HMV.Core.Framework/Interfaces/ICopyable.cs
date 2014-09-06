using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Interfaces
{
    public interface ICopyable
    {
        object Clone();
        object CloneDeep();
    }
}
