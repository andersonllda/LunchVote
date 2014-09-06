using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.Transporte;
using HMV.Core.Domain.Repository.Transporte;

namespace HMV.Core.Fake.Repository.Transporte
{
    public class RepositorioDeVeiculo : RepositoryBase<Veiculo>, IRepositorioDeVeiculo
    {
        public RepositorioDeVeiculo()
            : base()
        {
        }

        public IRepositorioDeVeiculo FiltraAtivos()
        {
            _list = _list.Where(x => x.Status == Status.Ativo).ToList();
            return this;
        }


        public IRepositorioDeVeiculo OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }
    }
}
