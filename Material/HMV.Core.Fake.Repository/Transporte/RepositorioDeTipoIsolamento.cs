using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.Transporte;
using HMV.Core.Domain.Repository.Transporte;

namespace HMV.Core.Fake.Repository.Transporte
{
    public class RepositorioDeTipoIsolamento : RepositoryBase<TipoIsolamento>, IRepositorioDeTipoIsolamento
    {
        public RepositorioDeTipoIsolamento()
            : base()
        {
        }

        public IRepositorioDeTipoIsolamento FiltraAtivos()
        {
            _list = _list.Where(x => x.Status == Status.Ativo).ToList();
            return this;
        }


        public IRepositorioDeTipoIsolamento OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }
    }
}
