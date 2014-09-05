using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using HMV.Core.Framework.Extensions;

namespace HMV.Core.Framework.AutoMapper
{
    public abstract class BaseFormatter<T> : IValueFormatter
    {
        public string FormatValue(ResolutionContext context)
        {
            if (context.SourceValue == null)
                return string.Empty;

            if (!(context.SourceValue is T))
                return context.SourceValue.ToNullSafeString();

            return FormatValueCore((T)context.SourceValue);
        }

        protected abstract string FormatValueCore(T value);
    }

}
