using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Exception
{
    public class EmailException : System.Exception
    {
        public EmailException(string pmensagem)
            : base(pmensagem)
        {
        }
    }
}
