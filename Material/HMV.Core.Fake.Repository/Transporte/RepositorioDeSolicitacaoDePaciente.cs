using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.Transporte;
using HMV.Core.Domain.Repository.Transporte;

namespace HMV.Core.Fake.Repository.Transporte
{
    public class RepositorioDeSolicitacaoDePaciente : RepositoryBase<SolicitacaoDePaciente>, IRepositorioDeSolicitacaoDePaciente
    {
        public RepositorioDeSolicitacaoDePaciente()
            : base()
        {
        }

        public IRepositorioDeSolicitacaoDePaciente OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }

        public IRepositorioDeSolicitacaoDePaciente FiltraPorStatus(StatusDaSolicitacaoTransporte status)
        {
            _list = _list.Where(x => x.Status == status).ToList();
            return this;
        }

        public IRepositorioDeSolicitacaoDePaciente FiltraPorPeriodo(System.DateTime inicio, System.DateTime fim)
        {
            _list = _list.Where(x => x.Data >= inicio && x.Data <= fim).ToList();
            return this;
        }

        public IRepositorioDeSolicitacaoDePaciente OrdernaPorID()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeSolicitacaoDePaciente OrdenaPorStatus()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeSolicitacaoDePaciente OrdenaPorID()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeSolicitacaoDePaciente FiltraNaoAtendida()
        {
            throw new System.NotImplementedException();
        }


        public IRepositorioDeSolicitacaoDePaciente OndeIdAtendimentoIgual(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
