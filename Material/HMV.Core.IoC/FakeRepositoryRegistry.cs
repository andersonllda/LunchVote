using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Repository.Transporte;
using HMV.Core.Fake.Repository.HMV;
using HMV.Core.Fake.Repository.Transporte;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;

namespace HMV.Core.IoC
{
    public class FakeRepositoryRegistry : Registry
    {
        public FakeRepositoryRegistry()
        {
            Scan(assemblyScanner =>
            {
                assemblyScanner.TheCallingAssembly();
                assemblyScanner.AddAllTypesOf(typeof(IRepositoryFilter<>));
                assemblyScanner.AssemblyContainingType(typeof(IRepositoryFilter<>));
            });

            ForRequestedType<IRepositorioDeVeiculo>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositorioDeVeiculo>();

            ForRequestedType<IRepositorioDeTipoIsolamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeTipoIsolamento>();

            ForRequestedType<IRepositorioDeSolicitacaoDeEquipamento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacaoDeEquipamento>();

            ForRequestedType<IRepositorioDeSolicitacaoDePaciente>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSolicitacaoDePaciente>();

            ForRequestedType<IRepositorioDeUsuarioFila>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuarioFila>();

            ForRequestedType<IRepositorioDeUsuarioTransporte>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuarioTransporte>();

            ForRequestedType<IRepositorioDeSetor>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeSetor>();

            ForRequestedType<IRepositorioDeAtendimento>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeAtendimento>();

            ForRequestedType<IRepositorioDeUsuarios>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<RepositorioDeUsuarios>();
        }
    }
}
