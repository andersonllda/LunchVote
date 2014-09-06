using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class HMVApplicationException : System.Exception
    {
        public HMVApplicationException(string msg)
            : base(msg)
        {
        }
    }
}
