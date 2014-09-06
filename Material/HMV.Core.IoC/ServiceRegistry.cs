using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;
using HMV.Core.Interfaces;
using StructureMap.Attributes;
using HMV.Core.Services;
using OmarALZabir.AspectF;
using HMV.Core.Framework.Cache;
using HMV.Core.Interfaces.EAD.CorpoClinico;
using HMV.Core.Services.EAD.CorpoClinico;

namespace HMV.Core.IoC
{
    public class ServiceRegistry : Registry
    {
        public ServiceRegistry()
        {

            ForRequestedType<IMenuService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<MenuService>();
             
            ForRequestedType<IConvenioService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<ConvenioService>();

            ForRequestedType<IEspecialidadeService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<EspecialidadeService>();

            ForRequestedType<ISistemaService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<SistemaService>();

            ForRequestedType<IUsuariosService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<UsuariosService>();
            
            ForRequestedType<IAcessoSistemaLogService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<AcessoSistemaLogService>();

            ForRequestedType<IClinicaService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<ClinicaService>();

            ForRequestedType<IPrestadorService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<PrestadorService>();

            ForRequestedType<IAgendaMedicaService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<AgendaMedicaService>();

            ForRequestedType<IPacienteService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<PacienteService>();

            ForRequestedType<IGenericSecurityService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<GenericSecurityService>();

            ForRequestedType<ISecurityService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<SecurityService>();

            ForRequestedType<IImageService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<ImageService>();

            ForRequestedType<IReunioesCientificasService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<ReunioesCientificasService>();

            ForRequestedType<IGrandRoundService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<GrandRoundService>();

            ForRequestedType<IAtendimentoService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<AtendimentoService>();

            ForRequestedType<IAreaAtuacaoService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<AreaAtuacaoService>();

            ForRequestedType<IMoodleService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<MoodleService>();

            ForRequestedType<IAuditoriaFechamentoService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<AuditoriaFechamentoService>();

            #region EAD CC

            ForRequestedType<IImagemTreinamentoService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<ImagemTreinamentoService>();

            ForRequestedType<ITreinamentoService>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefault.Is.OfConcreteType<TreinamentoService>();

            #endregion           
        }
    }
}
