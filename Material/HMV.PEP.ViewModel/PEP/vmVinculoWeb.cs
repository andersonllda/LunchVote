using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using StructureMap;
using HMV.Core.Framework.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Views.PEP;
using HMV.PEP.Interfaces;

namespace HMV.PEP.ViewModel.SumarioDeAtendimento
{
    public class vmVinculoWeb : ViewModelBase
    {
        #region Contrutor
        public vmVinculoWeb(Atendimento pAtendimento, Usuarios pUsuarios)
        {
            this._atendimento = new wrpAtendimento(pAtendimento);
            this._filtrapacientesprofissional = true;
            this.CarregaPacientesInternados();
            this._usuario = new wrpUsuarios(pUsuarios);
            this._prestador = this._usuario.Prestador;

            ISumarioDeAvaliacaoMedicaConsult consult = ObjectFactory.GetInstance<ISumarioDeAvaliacaoMedicaConsult>();
            this._sumariosweb = consult.carregaSumarioAvaliacoesWeb(pUsuarios);                       
        }
        #endregion

        #region Propriedades Publicas
        public IList<SumarioAvaliacaoMDTO> SumariosAvaliacaoMedicaWeb
        {
            get
            {
                return _sumariosweb;
            }
        }

        public SumarioAvaliacaoMDTO SumarioAvaliacaoMedicaWebSelecionado
        {
            get
            {
                return this._sumariowebselecionado;
            }
            set
            {
                this._sumariowebselecionado = value;
                this.OnPropertyChanged("SumarioAvaliacaoMedicaWebSelecionado");
            }
        }

        public int IdPrestador
        {
            get
            {
                return this._prestador.Id;
            }
        }

        public string NomePrestador
        {
            get
            {
                return this._prestador.Nome;
            }
        }

        public IList<vPacienteInternado> PacientesInternados
        {
            get
            {
                if (this._filtrapacientesprofissional)
                    return this._pacientesinternados.Where(x => x.IDMedicoAssistente == this._prestador.Id).ToList();
                return this._pacientesinternados;
            }
        }

        public vPacienteInternado PacienteInternadoSelecionado
        {
            get
            {
                return this._pacienteinternadoselecionado;
            }
            set
            {
                this._pacienteinternadoselecionado = value;
            }
        }

        public bool FiltraPacientesProfissional
        {
            get
            {
                return this._filtrapacientesprofissional;
            }
            set
            {
                this._filtrapacientesprofissional = value;                
                this.OnPropertyChanged("PacientesInternados");
                this.OnPropertyChanged("FiltraPacientesProfissional");
                this.OnPropertyChanged("BotaoVincular");
            }
        }

        public bool BotaoVincular
        {
            get { return (PacientesInternados.Count > 0 ? true : false); }
        }

        public bool ImprimeCO { get; set; }

        public wrpSumarioAvaliacaoMedica SumarioAvaliacaoMedica
        {
            get { return _sumario; }
        }
        #endregion

        #region Metodos Privados
        private void CarregaPacientesInternados()
        {
            IRepositorioDeAtendimentoInternado rep = ObjectFactory.GetInstance<IRepositorioDeAtendimentoInternado>();
            StructureMap.Pipeline.ExplicitArguments args = new StructureMap.Pipeline.ExplicitArguments();
            args.SetArg("rep", rep);            
            IListaPacientesInternadosConsult consult = ObjectFactory.GetInstance<IListaPacientesInternadosConsult>(args);
            this._pacientesinternados = consult.ListaPacientesInternados();

            IAmbulatorioService srv = ObjectFactory.GetInstance<IAmbulatorioService>();
            IList<PacienteAmbulatorialDTO> lst = srv.ListaPacientes(0, string.Empty);
            foreach (var item in lst.DistinctBy(x=> x.Atendimento))
            {
                this._pacientesinternados.Add(
                    new vPacienteInternado
                    {
                        Atendimento = item.Atendimento,
                        Data = item.Data,
                        Paciente = item.Paciente,
                        Prontuario = item.Prontuario,
                        IDMedicoAssistente = item.Prestador,
                        MedicoAssistente = item.MedicoAssistente,
                        Convenio = item.Convenio,
                        CPF = item.CPF,
                        DataNascimento = item.DataNascimento,
                        NomeMae = item.NomeMae,
                        TipoAtendimento = item.TipoAtendimentoDescricao                        
                    });
            }                        
        }
        #endregion

        #region Propriedades Privadas
        private wrpUsuarios _usuario { get; set; }
        private wrpAtendimento _atendimento { get; set; }
        private wrpPrestador _prestador { get; set; }
        private IList<vPacienteInternado> _pacientesinternados { get; set; }
        private vPacienteInternado _pacienteinternadoselecionado { get; set; }
        private bool _filtrapacientesprofissional { get; set; }
        private IList<SumarioAvaliacaoMDTO> _sumariosweb { get; set; }
        private SumarioAvaliacaoMDTO _sumariowebselecionado { get; set; }
        private Siga_Profissional _profissional { get; set; } 
        private wrpSumarioAvaliacaoMedica _sumario;
        #endregion

        #region Metodos Publicos
        public bool VerificaSeAbreConfirmacao()
        {
            if (this._pacienteinternadoselecionado.Paciente != this._sumariowebselecionado.NomePaciente)
                return true;
            return false;
        }

        public void VinculaSumarios()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = rep.OndeCodigoAtendimentoIgual(this._pacienteinternadoselecionado.Atendimento).Single();

            bool sumarioCO = false;

            IRepositorioDeParametrosInternet repCO2 = ObjectFactory.GetInstance<IRepositorioDeParametrosInternet>();
            ParametroInternet parametroCO2 = repCO2.OndeOrigemParaSumarioCO().Single();
            if (parametroCO2.IsNotNull())
            {
                try
                {
                    IList<int> codigos = parametroCO2.valor.Split(',').Select(x => int.Parse(x)).ToList();
                    if (codigos.Contains(atend.OrigemAtendimento.ID) && atend.SumarioAvaliacaoMedica.IsNull())
                    {
                        sumarioCO = true;
                    }
                }
                catch (Exception err)
                {
                    throw new Exception(err.ToString() + " Parametro CD_ORIGEM_CO_SUMARIO deve ser inteiro e separado por virgula.");
                }
            }

            IRepositorioDeSumariosAvaliacaoMedicaAbertos repsum = ObjectFactory.GetInstance<IRepositorioDeSumariosAvaliacaoMedicaAbertos>();
            _sumario = new wrpSumarioAvaliacaoMedica(repsum.OndeIDdoSumarioIgual(this._sumariowebselecionado.ID).Single());
            if (!sumarioCO)
            {
                ImprimeCO = false;
                _sumario.Paciente = new wrpPaciente(atend.Paciente);
                _sumario.Atendimento = new wrpAtendimento(atend);
                _sumario.Save();

                DXMessageBox.Show("Vínculo Concluído", "Vínculo Web", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                ImprimeCO = true;
                DXMessageBox.Show("ATENÇÃO – Não será possível vincular o Sumário de Avaliação Médica (realizado no portal médico) para os Pacientes Obstétricos." + Environment.NewLine +
                                  "O sistema irá mostrar em tela todos os dados preenchidos no sistema web, auxiliando ao redigir novamente o sumário de avaliação médica da paciente obstétrica." + Environment.NewLine
                                   , "Vínculo Web", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
        }

        public void DeletaSumarioWeb()
        {
            _sumario.DomainObject.ExcluiSumario();
            _sumario.Save();
        }
        #endregion

        #region Commands
        //public ICommand SaveSumarioAltaCommand { get; set; }
        #endregion
    }
}
