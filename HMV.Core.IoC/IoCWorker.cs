using System;
using System.Configuration;
using HMV.Core.Framework.Cache;
using HMV.Core.Framework.Log;
using HMV.Core.Interfaces;
using HMV.Core.Services;
using OmarALZabir.AspectF;
using StructureMap;
using StructureMap.Attributes;

namespace HMV.Core.IoC
{
    public static class IoCWorker
    {
        public static void ConfigureWEB()
        {
            ObjectFactory.Configure(i =>
            {
                i.AddRegistry<RepositoryRegistry>();
                i.AddRegistry<ServiceRegistry>();
                i.AddRegistry<DomainRegistry>();
                i.ForRequestedType<IEmailService>()
                 .CacheBy(InstanceScope.PerRequest)
                 .TheDefaultIsConcreteType<EmailService>();

                if (!Convert.ToBoolean(ConfigurationManager.AppSettings["DisableCache"]))
                {
                    i.ForRequestedType<ICache>()
                        .CacheBy(InstanceScope.PerRequest)
                        .TheDefault.Is.OfConcreteType<HMVCache>();
                }
                else
                {
                    i.ForRequestedType<ICache>()
                    .CacheBy(InstanceScope.PerRequest)
                    .TheDefault.Is.OfConcreteType<NoCacheResolver>();
                }

                i.ForRequestedType<ILogger>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<HMVLogger>();
            });
        }

        public static void ConfigureWIN()
        {
            ObjectFactory.Configure(i =>
            {
                i.AddRegistry<RepositoryRegistry>();
                i.AddRegistry<ServiceRegistry>();
                i.AddRegistry<DomainRegistry>();
                i.ForRequestedType<IEmailService>()
                 .CacheBy(InstanceScope.PerRequest)
                 .TheDefaultIsConcreteType<EmailServiceWCF>();

                i.ForRequestedType<ILogger>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<HMVLogger>();
            });
        }

        public static void ConfigureTEST()
        {
            ObjectFactory.Configure(i =>
            {
                i.AddRegistry<FakeRepositoryRegistry>();
                i.AddRegistry<ServiceRegistry>();
            });
        }
    }
}
