using System;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.Transporte;
using HMV.Core.Domain.Repository.Transporte;

namespace HMV.Core.Fake.Repository.Transporte
{
    public class RepositorioDeSolicitacaoDeEquipamento : RepositoryBase<SolicitacaoDeEquipamento>, IRepositorioDeSolicitacaoDeEquipamento
    {
        public RepositorioDeSolicitacaoDeEquipamento()
            : base()
        {
        }

        public IRepositorioDeSolicitacaoDeEquipamento OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }

        public IRepositorioDeSolicitacaoDeEquipamento FiltraPorStatus(System.Collections.Generic.IList<StatusDaSolicitacaoTransporte> status)
        {
            _list = _list.Where(x => status.Contains(x.Status)).ToList();
            return this;
        }

        public IRepositorioDeSolicitacaoDeEquipamento FiltraPorPeriodo(DateTime inicio, DateTime fim)
        {
            _list = _list.Where(x => x.Data >= inicio && x.Data <= fim).ToList();
            return this;
        }

        public IRepositorioDeSolicitacaoDeEquipamento OrdenaPorID()
        {
            throw new NotImplementedException();
        }

        public IRepositorioDeSolicitacaoDeEquipamento FiltraNaoAtendida()
        {
            throw new NotImplementedException();
        }
    }
}
