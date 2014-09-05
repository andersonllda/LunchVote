using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;

namespace HMV.Core.Fake.Repository.HMV
{
    public class RepositorioDeSetor : RepositoryBase<Setor>, IRepositorioDeSetor
    {
        public RepositorioDeSetor()
            : base()
        {
        }

        public IRepositorioDeSetor FiltraAtivos()
        {
            _list = _list.Where(x => x.Ativo == SimNao.Sim).ToList();
            return this;
        }

        public IRepositorioDeSetor OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }

        public IRepositorioDeSetor OrdenaPorDescricao()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeSetor OndeDescricaoContem(string pDescricao)
        {
            throw new System.NotImplementedException();
        }
    }
}
