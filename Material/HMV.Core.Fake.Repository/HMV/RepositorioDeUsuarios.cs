using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;

namespace HMV.Core.Fake.Repository.HMV
{
    public class RepositorioDeUsuarios : RepositoryBase<Usuarios>, IRepositorioDeUsuarios
    {
        public RepositorioDeUsuarios()
            : base(false)
        {
            var usuario = new Usuarios();
            usuario.NaoCarregarSenha = true;
            usuario.cd_usuario = "DESEN";
            _list = new List<Usuarios> { usuario };
        }

        public IRepositorioDeUsuarios OndeCodigoIgual(string ID)
        {
            _list = _list.Where(x => x.ID == ID).ToList();
            return this;
        }

        public IRepositorioDeUsuarios OndeSistemaIgual(int seq_sistema)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndePrestadorPermiteTerAcessoRedeSemFio()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndeNomePrestadorIgual(string pNomePrestador)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndeRegistroDoPrestadorIgual(string pRegistro)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndeNomeContem(string pNome)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndeIdUsuarioIgualOuNomeContem(string pPesquisa)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndeUsuariosAtivos()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OndeUsuarioTemSetoresAssociadosSistemaAtivo()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeUsuarios OrdenaPorNome()
        {
            throw new System.NotImplementedException();
        }

        public string BuscaSenhaSistemaMV(string pID)
        {
            throw new System.NotImplementedException();
        }
    }
}
