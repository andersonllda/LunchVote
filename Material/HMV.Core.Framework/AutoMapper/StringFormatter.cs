using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.AutoMapper
{
    public class StringFormatter : BaseFormatter<string>
    {
        protected override string FormatValueCore(string value)
        {
            return value;
        }
    }
}
