using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using System.Windows.Media;
using System.Windows;
using HMV.Core.Framework.Expression;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.BoletimEmergencia;


namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia
{
    public class vmBoletimEmergenciaCO : ViewModelBase
    {
        #region Enum
        public enum TabsBoletimEmergenciaCO
        {
            [Description("Classificação")]
            Classificacao = 0,
            [Description("Sinais Vitais")]
            SinaisVitais = 1,
            [Description("Motivo Consulta")]
            MotivoConsulta = 2,
            [Description("Avaliação Clínica")]
            AvaliacaoClinica = 3,
            [Description("Procedimentos/Exames")]
            ProcedimentosExames = 4,
            [Description("CID's/Diagnóstico")]
            CidDiagnostico = 5,
            [Description("Conduta e Reavaliações")]
            CondutaEReavaliacoes = 6,
            [Description("Orientações do Médico Assistente")]
            OrientacoesDoMedicoAssistente = 7,
            [Description("Relatório")]
            Relatorio = 8
        }
        #endregion
        
        #region Contrutor
        public vmBoletimEmergenciaCO(Atendimento pAtendimento, Paciente pPaciente, Usuarios pUsuario)
        {
            _dictionary = new Dictionary<TabsBoletimEmergenciaCO, ViewModelBase>();

            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.Refresh(pAtendimento);

            this._usuario = new wrpUsuarios(pUsuario);
            this._paciente = new wrpPaciente(pPaciente);

            this._atendimento = new wrpAtendimento(pAtendimento);

            if (pAtendimento.BoletinsDeEmergencia != null)
            {
                if (pAtendimento.BoletinsDeEmergencia.Count == 0)
                {
                    pAtendimento.BoletinsDeEmergencia.Add(new HMV.Core.Domain.Model.BoletimDeEmergencia(pAtendimento));
                }
                this._boletim = new wrpBoletimDeEmergencia(pAtendimento.BoletinsDeEmergencia.OrderBy(x => x.Id).Last());
            }
            else
            {
                pAtendimento.BoletinsDeEmergencia.Add(new HMV.Core.Domain.Model.BoletimDeEmergencia(pAtendimento));
                this._boletim = new wrpBoletimDeEmergencia(pAtendimento.BoletinsDeEmergencia.OrderBy(x => x.Id).Last());
            }            

            if (pUsuario.Prestador.Conselho.isMedico() && this._boletim.DataHoraInicioAtendimento == null)
                this._boletim.DataHoraInicioAtendimento = DateTime.Now;

            this._boletim.BoletimCO = Core.Domain.Enum.SimNao.Sim;

        }
        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        private wrpBoletimDeEmergencia _boletim;
        private IList<RuntimeTab<TabsBoletimEmergenciaCO>> _tabs;
        private TabsBoletimEmergenciaCO _tipotabselecionada;
        private vmClassificacao _vmclassificacao;
        private vmMotivoConsulta _vmMotivoConsulta;
        private vmAvaliacaoClinica _vmAvaliacaoClinica;
        private Dictionary<TabsBoletimEmergenciaCO, ViewModelBase> _dictionary;
        private vmCondutaEReavaliacao _vmCondutaEReavaliacao;
        private vmProcedimentosExames _vmProcedimentosExames;
        private vmOrientacoesDoMedicoAssistente _vmOrientacoesDoMedicoAssistente;
        private vmCidDiagnostico _vmCidDiagnostico;
        private vmSinaisVitaisCO _vmSinaisVitais;
        #endregion   

        #region Propriedades Publicas

        public wrpBoletimCentroObstetrico BoletimCO
        {
            get {
                if (_boletim.CentroObstetrico.IsNull())
                    _boletim.CentroObstetrico = new wrpBoletimCentroObstetrico(_boletim, _usuario);
                return _boletim.CentroObstetrico;
            }
        }

        public wrpBoletimDeEmergencia Boletim
        {
            get { return _boletim; }
        }

        public wrpAtendimento Atendimento
        {
            get { return this._atendimento; }
        }    
        public wrpUsuarios Usuarios
        {
            get { return this._usuario; }
        }

        public IList<RuntimeTab<TabsBoletimEmergenciaCO>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
            set {
                _tabs = value;
                OnPropertyChanged<vmBoletimEmergenciaCO>(x=>x.Tabs);
            }
        }


        TabsBoletimEmergenciaCO? _tabQueEstavaSelecionada; 

        public TabsBoletimEmergenciaCO? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                _tabQueEstavaSelecionada = value.Value;
                this.OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.TipoTabSelecionada);
            }
        }

        public SolidColorBrush CorRisco
        {
            get
            {
                if (this._boletim != null && this._boletim.Classificacoes.Count > 0 && _boletim.AltaCO.IsNull() )
                        return this._boletim.Classificacoes.OrderBy(x => x.DataHoraInclusaoDate).Last().CorTeste;
                    return null;
            }
        }

        public Dictionary<TabsBoletimEmergenciaCO, ViewModelBase> DictionaryCO
        {
            get { return _dictionary; }
            set {
                _dictionary = value;               
            }
        }

        //VMs
        public vmClassificacao vmClassificacao
        {
            get
            {
                if (this._vmclassificacao.IsNull())
                    this._vmclassificacao = new vmClassificacao(this._boletim, this._usuario, this);
                return this._vmclassificacao;
            }
        }

        public vmMotivoConsulta vmMotivoConsulta
        {
            get
            {
                if (this._vmMotivoConsulta.IsNull())
                    this._vmMotivoConsulta = new vmMotivoConsulta(this);
                return this._vmMotivoConsulta;
            }
        }

        public vmAvaliacaoClinica vmAvaliacaoClinica
        {
            get
            {
                if (this._vmAvaliacaoClinica.IsNull())
                    this._vmAvaliacaoClinica = new vmAvaliacaoClinica(this);
                return this._vmAvaliacaoClinica;
            }
        }

        public vmCondutaEReavaliacao vmCondutaEReavaliacao
        {
            get {
                if (this._vmCondutaEReavaliacao.IsNull())
                    this._vmCondutaEReavaliacao = new vmCondutaEReavaliacao(this);
                return this._vmCondutaEReavaliacao;            
            }
        }

        public vmProcedimentosExames vmProcedimentosExames
        {
            get
            {
                if (this._vmProcedimentosExames.IsNull())
                    this._vmProcedimentosExames = new vmProcedimentosExames(this);
                return this._vmProcedimentosExames;
            }
        }

        public vmOrientacoesDoMedicoAssistente vmOrientacoesDoMedicoAssistente
        {
            get {
                if (this._vmOrientacoesDoMedicoAssistente.IsNull())
                    this._vmOrientacoesDoMedicoAssistente = new vmOrientacoesDoMedicoAssistente(this);
                return this._vmOrientacoesDoMedicoAssistente;
            }
        }

        public vmCidDiagnostico vmCidDiagnostico
        {
            get
            {
                if (this._vmCidDiagnostico.IsNull())
                    this._vmCidDiagnostico = new vmCidDiagnostico(this);
                return this._vmCidDiagnostico;
            }
        }

        public vmSinaisVitaisCO vmSinaisVitais
        {
            get
            {
                if (this._vmSinaisVitais.IsNull())
                    this._vmSinaisVitais = new vmSinaisVitaisCO(this);
                return this._vmSinaisVitais;
            }
        }

        public bool boolImprimir
        {
            get { return this.BoletimCO.IsNotNull() && this.Boletim.AltaCO.IsNotNull(); }
        }

        public bool boolImprimirEAlta
        {
            get
            {
                if (this.Usuarios.Prestador.IsNurse) return false;

                return boolImprimir;
            }
        
        }

        public bool boolVisualizar
        { get { return !boolImprimir; } }

        #endregion     

        #region Metodos Públicos
        public void RefreshCor()
        {
            this.OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.CorRisco);
        }
        public void Salva()
        {
            if (TipoTabSelecionada == TabsBoletimEmergenciaCO.AvaliacaoClinica)
            {
                if (!vmAvaliacaoClinica.IsValid)
                    return;
            }

            IRepositorioDeBoletimDeEmergencia rep = ObjectFactory.GetInstance<IRepositorioDeBoletimDeEmergencia>();
            rep.Save(_boletim.DomainObject);
        }
        #endregion

        #region Métodos Privados
        private void LimpaVms()
        {
            DictionaryCO.Clear();
            _vmAvaliacaoClinica = null;
            _vmCidDiagnostico = null;
            _vmclassificacao = null;
            _vmCondutaEReavaliacao = null;
            _vmMotivoConsulta = null;
            _vmOrientacoesDoMedicoAssistente = null;
            _vmProcedimentosExames = null;
            _vmSinaisVitais = null;
        }

        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsBoletimEmergenciaCO>>();
            OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.CorRisco);
            OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.boolImprimir);

            if (Boletim.AltaCO.IsNull())
            {

                //Classificacao
                this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                {
                    TipoTab = TabsBoletimEmergenciaCO.Classificacao,
                    Descricao = TabsBoletimEmergenciaCO.Classificacao.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucClassificacao.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmClassificacao)),
                        Source = this
                    }
                });

                //Sinais Vitais
                this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                {
                    TipoTab = TabsBoletimEmergenciaCO.SinaisVitais,
                    Descricao = TabsBoletimEmergenciaCO.SinaisVitais.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucSinaisVitais.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,                       
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmSinaisVitais)),
                        Source = this
                    }
                });

                //Motivo Consulta
                this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                {
                    TipoTab = TabsBoletimEmergenciaCO.MotivoConsulta,
                    Descricao = TabsBoletimEmergenciaCO.MotivoConsulta.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucMotivoConsulta.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmMotivoConsulta)),
                        Source = this
                    }
                });

                if (!this._usuario.Prestador.IsNurse)
                {
                    //Avaliacao Clinica
                    this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                    {
                        TipoTab = TabsBoletimEmergenciaCO.AvaliacaoClinica,
                        Descricao = TabsBoletimEmergenciaCO.AvaliacaoClinica.GetEnumDescription(),
                        Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucAvaliacaoClinica.xaml", UriKind.Relative),
                        Binding = new Binding
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmAvaliacaoClinica)),
                            Source = this
                        }
                    });

                    //ProcedimentosExames
                    this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                    {
                        TipoTab = TabsBoletimEmergenciaCO.ProcedimentosExames,
                        Descricao = TabsBoletimEmergenciaCO.ProcedimentosExames.GetEnumDescription(),
                        Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucProcedimentosExames.xaml", UriKind.Relative),
                        Binding = new Binding
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmProcedimentosExames)),
                            Source = this
                        }
                    });

                    //Cid/Diagnostico
                    this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                    {
                        TipoTab = TabsBoletimEmergenciaCO.CidDiagnostico,
                        Descricao = TabsBoletimEmergenciaCO.CidDiagnostico.GetEnumDescription(),
                        Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucCIDs.xaml", UriKind.Relative),
                        Binding = new Binding
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmCidDiagnostico)),
                            Source = this
                        }
                    });


                    //Conduta e Reavaliacoes
                    this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                    {
                        TipoTab = TabsBoletimEmergenciaCO.CondutaEReavaliacoes,
                        Descricao = TabsBoletimEmergenciaCO.CondutaEReavaliacoes.GetEnumDescription(),
                        Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucCondutaReavaliacoes.xaml", UriKind.Relative),
                        Binding = new Binding
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmCondutaEReavaliacao)),
                            Source = this
                        }
                    });

                    //Orientacoes Do Medico Assistente
                    this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                    {
                        TipoTab = TabsBoletimEmergenciaCO.OrientacoesDoMedicoAssistente,
                        Descricao = TabsBoletimEmergenciaCO.OrientacoesDoMedicoAssistente.GetEnumDescription(),
                        Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucMedicoAssistente.xaml", UriKind.Relative),
                        Binding = new Binding
                        {
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                            Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmOrientacoesDoMedicoAssistente)),
                            Source = this
                        }
                    });

                }
            }
            else
            {
                //Relatório
                this._tabs.Add(new RuntimeTab<TabsBoletimEmergenciaCO>
                {
                    TipoTab = TabsBoletimEmergenciaCO.Relatorio,
                    Descricao = TabsBoletimEmergenciaCO.Relatorio.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\CentroObstetrico\BoletimDeEmergencia\ucRelatorio.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        //Path = new PropertyPath(ExpressionEx.PropertyName<vmBoletimEmergenciaCO>(x => x.vmCidDiagnostico)),
                        Source = this
                    }
                });
            }

        }

        private bool mensagemValidaAlta(string msg)
        {
            DXMessageBox.Show(msg, "Atenção", MessageBoxButton.OK, MessageBoxImage.Stop);
            return false;
        }

        public bool validaAlta()
        {
            if (this._boletim.Classificacoes.Count() == 0)
                return mensagemValidaAlta("Deve ser informada a classificação.");

            if (this._boletim.SinaisVitais.Count() == 0)
                return mensagemValidaAlta("Deve ser informado os sinais vitais.");

            if (this._boletim.MotivoConsultaCO.Count() == 0)
                return mensagemValidaAlta("Deve ser informada o motivo da consulta.");

            if (this._boletim.CentroObstetrico.AvaliacoesClinicas.Count() == 0)
                return mensagemValidaAlta("Deve ser informada a avaliação clínica.");

            if (this._boletim.CentroObstetrico.Procedimentos.Count() == 0)
                return mensagemValidaAlta("Deve ser informado Procedimentos/Exames.");

            if (this._boletim.CentroObstetrico.Orientacoes.Count() == 0)
                return mensagemValidaAlta("Deve ser informada as Orientações do Médico Assistente.");
            
            if (this._boletim.CentroObstetrico.Condutas.Count() == 0)
                return mensagemValidaAlta("Deve ser informada a Conduta e Reavaliações.");

            if (this._boletim.Atendimento.CIDs.Count() == 0)
                return mensagemValidaAlta("Deve ser informado CID's/Diagnóstico.");
            
            return true;
        }

        #endregion

        protected override void CommandSalvar(object param)
        {
            this.Salva();
        }

        protected override void CommandIncluir(object param)
        {
            LimpaVms();
            this._boletim = new wrpBoletimDeEmergencia(_atendimento.DomainObject);
            _atendimento.BoletinsDeEmergencia.Add(this._boletim);
            OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.boolImprimirEAlta);
            OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.Boletim);
        }

        public override void RefreshViewModel()
        {
            OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.boolImprimir);
            OnPropertyChanged<vmBoletimEmergenciaCO>(x => x.boolImprimirEAlta);
            base.RefreshViewModel();
        }

        protected override bool CommandCanExecuteAbrir(object param)
        {
            return !this.Usuarios.Prestador.IsNurse;
        }

    }

    public class BoletimCOHistoricoDTO
    {       
        public BoletimCOHistoricoDTO(bool pIsNurse, string pTitulo, string pDescricao)
        {
            IsVisibilityPeople = Visibility.Visible;
            IsVisibilityBorda = Visibility.Collapsed;
            IsNurse = pIsNurse;
            Descricao = pDescricao;
            Titulo = pTitulo;
        }

        public BoletimCOHistoricoDTO(string pTitulo, string pDescricao, bool pIsBorda)
        {
            IsVisibilityPeople = Visibility.Collapsed;
            IsVisibilityBorda = pIsBorda ? Visibility.Visible : Visibility.Collapsed;
            Descricao = pDescricao;
            Titulo = pTitulo;
        }

        public BoletimCOHistoricoDTO(string pTitulo, string pDescricao)
        {
            IsVisibilityPeople = Visibility.Collapsed;
            IsVisibilityBorda = Visibility.Collapsed;
            Descricao = pDescricao;
            Titulo = pTitulo;
        }

        public bool IsNurse { get; set; }
        public string Descricao { get; set; }
        public string Titulo { get; set; }
        public Visibility IsVisibilityPeople { get; set; }
        public Visibility IsVisibilityBorda { get; set; }
    }

}
