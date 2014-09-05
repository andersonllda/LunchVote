using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Validator.Engine;

namespace HMV.Core.Framework.Exception
{

    public class ValidatorException : System.Exception
    {
        private string _msg;

        public ValidatorException(string pMsg)
        {
            _msg = pMsg;
        }

        public override string Message
        {
            get
            {
                return _msg;
            }
        }
    }
}
