using LunchVote.Domain.Repository;
using StructureMap.Configuration.DSL;

namespace LunchVote.IoC
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
            //ForRequestedType<IRepositorioDeVeiculo>()
            //   .CacheBy(InstanceScope.PerRequest)
            //   .TheDefaultIsConcreteType<RepositorioDeVeiculo>();            
        }    
    }
}
