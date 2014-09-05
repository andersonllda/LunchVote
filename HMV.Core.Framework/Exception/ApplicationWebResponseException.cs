using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class ApplicationWebResponseException : System.Exception
    {
        public ApplicationWebResponseException(string msg)
            : base(msg)
        {
        }
    }
}
