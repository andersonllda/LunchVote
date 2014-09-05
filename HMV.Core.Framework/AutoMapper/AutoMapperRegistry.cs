using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using HMV.Core.Framework.Extensions;

namespace HMV.Core.Framework.AutoMapper
{
    public class AutoMapperRegistry
    {
        public static void Configure()
        {
            Mapper.Reset();
            Mapper.Initialize(x =>
            {
                x.AddProfile<BasicProfile>();
            });

            Mapper.AssertConfigurationIsValid();
        }

        public static void Configure(params Profile[] profiles)
        {
            Mapper.Reset();

            Mapper.Initialize(x =>
            {
                profiles.Each(z =>
                {
                    x.AddProfile(z);
                });
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
