using LunchVote.Domain.Repository;
using LunchVote.Repository.Fake;
using StructureMap.Attributes;
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

            ForRequestedType<IRepositoryProfissional>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoryProfissional>();

            ForRequestedType<IRepositoryRestaurante>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoryRestaurante>();

            ForRequestedType<IRepositoryVotacao>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoryVotacao>();

            ForRequestedType<IRepositoryVoto>()
               .CacheBy(InstanceScope.PerRequest)
               .TheDefaultIsConcreteType<RepositoryVoto>();  
        }    
    }
}
