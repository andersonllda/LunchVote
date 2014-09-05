using System.Linq;
using FizzWare.NBuilder;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;

namespace HMV.Core.Fake.Repository.HMV
{
    public class RepositorioDeAtendimento : RepositoryBase<Atendimento>, IRepositorioDeAtendimento
    {
        public RepositorioDeAtendimento()
        {
            var paciente = Builder<Paciente>.CreateNew().With(p => p.Nome = "Teste").Build();

            _list = Builder<Atendimento>.CreateListOfSize(10)
                .All()
                .And(x => x.Paciente = paciente)
                .Build();             
        }

        public IRepositorioDeAtendimento OndeIdIgual(int id)
        {
            _list = _list.Where(x => x.ID == id).ToList();
            return this;
        }

        public IRepositorioDeAtendimento OndeCodigoAtendimentoIgual(int? pcd_atendimento)
        {
            _list = _list.Where(x => x.ID == pcd_atendimento.Value).ToList();
            return this;
        }

        public IRepositorioDeAtendimento OndeCodigoConvenioIgual(int pcd_convenio)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeCodigoPrestadorIgual(int pcd_prestador)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeCodigoPacienteIgual(int pcd_paciente)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeDataAtendimentoIgual(System.DateTime pdata_atendimento)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento EntreOPeriodoDeAtendimento(System.DateTime pdata_atendimento_inicial, System.DateTime pdata_atendimento_final)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeTipoAtendimentoIgual(TipoAtendimento ptipo_atendimento)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento EntreOPeriodoDeAgendamento(System.DateTime PDataAgendamentoInicial, System.DateTime pDataAgendamentoFinal)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeOrigemDoAtendimentoIgual(int pIdOrigem)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeNomeDoPacienteIgual(string pNomePaciente)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndePacienteAtivo()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OrdenaPorDataDeAtendimento()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OrdenaPorNomeDePaciente()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeDataAltaIsNull()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeNaoInternadoEmergencia()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento QuePossuiArquivos()
        {
            throw new System.NotImplementedException();
        }

        public IRepositorioDeAtendimento OndeExisteRegistroDeDor()
        {
            throw new System.NotImplementedException();
        }
    }
}
