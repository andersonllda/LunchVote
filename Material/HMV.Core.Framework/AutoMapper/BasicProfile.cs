using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

namespace HMV.Core.Framework.AutoMapper
{
    public class BasicProfile: Profile
    {
        protected override void Configure()
        {
            Mapper.AddFormatter<StringFormatter>();
        }
    }
}
