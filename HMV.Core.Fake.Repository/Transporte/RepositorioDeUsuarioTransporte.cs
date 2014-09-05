using System.Linq;
using HMV.Core.Domain.Model.Transporte;
using HMV.Core.Domain.Repository.Transporte;

namespace HMV.Core.Fake.Repository.Transporte
{
    public class RepositorioDeUsuarioTransporte : RepositoryBase<UsuarioTransporte>, IRepositorioDeUsuarioTransporte
    {
        public RepositorioDeUsuarioTransporte()
            : base()
        {
        }

        public IRepositorioDeUsuarioTransporte FiltraAtivos()
        {
            //_list = _list.Where(x => x.Usuario.sn_ativo == 'A').ToList();
            _list = _list.Where(x => x.Status == Domain.Enum.Status.Ativo).ToList();
            return this;
        }


        public IRepositorioDeUsuarioTransporte OndeIdIgual(string id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }
    }
}
