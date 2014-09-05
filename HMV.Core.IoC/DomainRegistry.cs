using HMV.Core.Domain.Interfaces;
using HMV.Core.Domain.Model;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;

namespace HMV.Core.IoC
{
    public class DomainRegistry : Registry
    {
        public DomainRegistry()
        {
            ForRequestedType<ISumarioAvaliacaoMedicaItens>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<SumarioAvaliacaoMedicaItens>();

            ForRequestedType<ISumarioAvaliacaoMedicaItensDetalhe>()
                .CacheBy(InstanceScope.PerRequest)
                .TheDefaultIsConcreteType<SumarioAvaliacaoMedicaItensDetalhe>();
        }
    }
}
