using System.Linq;
using HMV.Core.Domain.Model.Transporte;
using HMV.Core.Domain.Repository.Transporte;

namespace HMV.Core.Fake.Repository.Transporte
{
    public class RepositorioDeUsuarioFila : RepositoryBase<UsuarioFila>, IRepositorioDeUsuarioFila
    {
        public RepositorioDeUsuarioFila()
            : base()
        {
        }

        public IRepositorioDeUsuarioFila OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }

        public IRepositorioDeUsuarioFila OndeIdUsuarioIgual(string id)
        {
            _list = _list.Where(x => x.Usuario.ID == id).ToList();
            return this;
        }

        public IRepositorioDeUsuarioFila OndeUsuarioEstaNoInicioDaFila()
        {
            throw new System.NotImplementedException();
        }


        public IRepositorioDeUsuarioFila Ordena()
        {
            throw new System.NotImplementedException();
        }
    }
}
