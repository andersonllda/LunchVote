using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Types
{
    public class CustomDisplay : Attribute
    {
        private readonly string _displayName = string.Empty;
        
        public CustomDisplay(string displayName)
        {
            this._displayName = displayName;
        }

        public override string ToString()
        {
            return this._displayName;
        }
    }
}
