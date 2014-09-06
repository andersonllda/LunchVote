using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.AdmAssistencial;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialCTINEO;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.AdmissaoAssistencialCTI;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using StructureMap;
using HMV.Core.Wrappers.CollectionWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO
{
    public class vmSumarioAvaliacaoMedicaCTINEO : ViewModelBase
    {
        #region Enum
        public enum TabsSumarioAvaliacaoMedicaCTINEO
        {
            [Description("Motivo Internação/História")]
            MotivoInternacaoHistoria,
            [Description("Dados do Nascimento")]
            DadosNascimento,
            [Description("Exame Físico")]
            ExameFisico,
            [Description("Diagnósticos/Hipótese Diagnósticas")]
            DiagnosticosHipotesesDiagnosticas,
            [Description("Plano Diagnóstico e Terapêutico")]
            PlanoDiagnosticoTerapeutico,
            [Description("Finalizar/Imprimir")]
            FinalizarImprimir
        }
        #endregion

        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaCTINEO(Atendimento pAtendimento, Usuarios pUsuario, bool pIsCorpoClinico)
        {
            this._usuario = new wrpUsuarios(pUsuario);
            this._atendimento = new wrpAtendimento(pAtendimento);
            this._paciente = this._atendimento.Paciente;
            this._iscorpoclinico = pIsCorpoClinico;

            ////VALIDA SE O USUARIO É MEDICO OU ENFERMEIRA
            //if (PodeAbrirSumario())
            //{
            IRepositorioDeSumarioAvaliacaoMedicaCTINEO rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
            var ret = rep.OndeCodigoAtendimentoIgual(pAtendimento).Single();
            if (ret.IsNotNull())
                this._sumarioavaliacaomedicactineo = new wrpSumarioAvaliacaoMedicaCTINEO(ret);

            if (this._sumarioavaliacaomedicactineo.IsNull())
            {
                this._sumarioavaliacaomedicactineo = new wrpSumarioAvaliacaoMedicaCTINEO(this._atendimento, this._usuario, this._paciente);
                //this._sumarioavaliacaomedicactineo.Save();

                this._copiardadosultimaadmissao();
                this.Novo = true;

                if (_sumarioavaliacaomedicactineo.Atendimento.SumarioAvaliacaoMedicaRNMVRecemNascido.IsNotNull())
                {
                    this._sumarioavaliacaomedicactineo.DataNascimento = _sumarioavaliacaomedicactineo.Atendimento.SumarioAvaliacaoMedicaRNMVRecemNascido.DataNascimento;
                }

                //Verifica rotina de sumario já existente para o paciente.
                rep = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
                int id = rep.ImportarSumarioUTINEO(pAtendimento.ID, pAtendimento.Paciente.ID);
                if (id > 0)
                {
                    IRepositorioDeSumarioAvaliacaoMedicaCTINEO rep2 = ObjectFactory.GetInstance<IRepositorioDeSumarioAvaliacaoMedicaCTINEO>();
                    var antigo = rep2.OndeIdIgual(id).Single();
                    if (antigo.IsNotNull())
                    {
                        this._sumarioavaliacaomedicactineo.ApgarDessimo = antigo.ApgarDessimo;
                        this._sumarioavaliacaomedicactineo.ApgarPrimeiro = antigo.ApgarPrimeiro;
                        this._sumarioavaliacaomedicactineo.ApgarQuinto = antigo.ApgarPrimeiro;
                        this._sumarioavaliacaomedicactineo.Conduta = antigo.Conduta;
                        this._sumarioavaliacaomedicactineo.DataNascimento = antigo.DataNascimento;
                        this._sumarioavaliacaomedicactineo.Exames = antigo.Exames;
                        this._sumarioavaliacaomedicactineo.HistoriaAtual = antigo.HistoriaAtual;
                        this._sumarioavaliacaomedicactineo.IdadeDesconhecida = antigo.IdadeDesconhecida;
                        this._sumarioavaliacaomedicactineo.IdadeDias = antigo.IdadeDias;
                        this._sumarioavaliacaomedicactineo.IdadeSemana = antigo.IdadeSemana;
                        this._sumarioavaliacaomedicactineo.IsForcipe = antigo.IsForcipe;
                        this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia = antigo.MotivoCesarianaUrgencia;
                        this._sumarioavaliacaomedicactineo.MotivoInternacao = antigo.MotivoInternacao;
                        this._sumarioavaliacaomedicactineo.Observacoes = antigo.Observacoes;
                        this._sumarioavaliacaomedicactineo.Peso = antigo.Peso;
                        this._sumarioavaliacaomedicactineo.Procedencia = new wrpAdmissaoAssistencialProcedenciaCTINEO(antigo.Procedencia);
                        this._sumarioavaliacaomedicactineo.ProcedenciaOutros = antigo.ProcedenciaOutros;
                        this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEODiagnostico = new wrpSumarioAvaliacaoMedicaCTINEODiagnosticoCollection(antigo.SumarioAvaliacaoMedicaCTINEODiagnostico);
                        this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico = new wrpSumarioAvaliacaoMedicaCTINEOExameFisico(antigo.SumarioAvaliacaoMedicaCTINEOExameFisico);
                        this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOHipotese = new wrpSumarioAvaliacaoMedicaCTINEOHipoteseCollection(antigo.SumarioAvaliacaoMedicaCTINEOHipotese);
                        //this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens = new wrpSumarioAvaliacaoMedicaCTINEOItemCollection(antigo.SumarioAvaliacaoMedicaCTINEOItens);
                        this._sumarioavaliacaomedicactineo.TipoParto = antigo.TipoParto;

                        this._sumarioavaliacaomedicactineo.ApgarPrimeiro = antigo.ApgarPrimeiro;

                        foreach (SumarioAvaliacaoMedicaCTINEOItem item in antigo.SumarioAvaliacaoMedicaCTINEOItens)
                        {
                            var novoItem = new SumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo.DomainObject, item.ItemCO);
                            novoItem.Observacao = item.Observacao;
                            novoItem.IsNormal = item.IsNormal;
                            this._sumarioavaliacaomedicactineo.DomainObject.SumarioAvaliacaoMedicaCTINEOItens.Add(novoItem);
                        }
                    }
                }
            }
            else
            {
                this._procedencia = ProcedenciaCollection.Where(x=> x.ID == this._sumarioavaliacaomedicactineo.Procedencia.ID).SingleOrDefault();
            }
            //}
        }

        private void _copiardadosultimaadmissao()
        {
            wrpAdmissaoAssistencialCTINEO ultadm = null;

            IRepositorioDeAdmissaoAssistencialCTINEO rep = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCTINEO>();
            var ret = rep.OndeIdAtendimentoIgual(this._atendimento.ID);
            if (ret.List().Count(x => x.DataEncerramento.IsNotNull() && x.DataExclusao.IsNull()) > 0)
            {
                ultadm = new wrpAdmissaoAssistencialCTINEO(ret.List().Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.DataEncerramento).FirstOrDefault());
                rep.Refresh(ultadm.DomainObject);
            }

            if (ultadm.IsNotNull())
            {
                this._sumarioavaliacaomedicactineo.Procedencia = ultadm.Procedencia;
                this._procedencia = this._sumarioavaliacaomedicactineo.Procedencia;
                this._sumarioavaliacaomedicactineo.ProcedenciaOutros = ultadm.ProcedenciaOutros;
                this._sumarioavaliacaomedicactineo.TipoParto = ultadm.TipoParto;
                this._sumarioavaliacaomedicactineo.IsForcipe = ultadm.IsForcipe;
                this._sumarioavaliacaomedicactineo.IdadeSemana = ultadm.IdadeSemana;
                this._sumarioavaliacaomedicactineo.IdadeDias = ultadm.IdadeDias;
                if (ultadm.IdadeDesconhecido.IsNotNull())
                    this._sumarioavaliacaomedicactineo.IdadeDesconhecida = ultadm.IdadeDesconhecido.Value;
                this._sumarioavaliacaomedicactineo.ApgarPrimeiro = ultadm.ApgarPrimeiro;
                this._sumarioavaliacaomedicactineo.ApgarQuinto = ultadm.ApgarQuinto;
                this._sumarioavaliacaomedicactineo.ApgarDessimo = ultadm.ApgarDessimo;

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNull())
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico = new wrpSumarioAvaliacaoMedicaCTINEOExameFisico(this._sumarioavaliacaomedicactineo);

                this._sumarioavaliacaomedicactineo.Peso = ultadm.Peso;
                this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FC = ultadm.AdmissaoAssistencialExameFisicoCTINEO.FC;
                this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FR = ultadm.AdmissaoAssistencialExameFisicoCTINEO.FR;
                this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX = ultadm.AdmissaoAssistencialExameFisicoCTINEO.TAX;
                this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.DOR = ultadm.AdmissaoAssistencialExameFisicoCTINEO.Dor;
                this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT = ultadm.AdmissaoAssistencialExameFisicoCTINEO.SAT;
                this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM = ultadm.AdmissaoAssistencialExameFisicoCTINEO.PAM;
            }
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>> _tabs;
        private TabsSumarioAvaliacaoMedicaCTINEO _tipotabselecionada;
        private wrpAtendimento _atendimento;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        private bool _iscorpoclinico;
        private wrpAdmissaoAssistencialProcedenciaCTINEOCollection _procedenciacollection;
        private wrpAdmissaoAssistencialProcedenciaCTINEO _procedencia;
        private DateTime? _dianascimento;
        private DateTime? _horanascimento;
        private wrpSumarioAvaliacaoMedicaCTINEO _sumarioavaliacaomedicactineo;
        private bool _tabatualdiagnosticohipotese = false;

        //VMs        
        private vmSumarioAvaliacaoMedicaCTINEOExameFisico _vmexamefisicosumarioavaliacaomedicactineo;
        private vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO _vmdiagnosticohipotesesumarioavaliacaomedicactineo;
        #endregion

        #region ----- Propriedades Públicas -----
        public bool SalvaDiagnosticoHipotese { get { return _tabatualdiagnosticohipotese; } set { _tabatualdiagnosticohipotese = value; } }

        public bool Novo { get; set; }

        //public bool PodeAbrir { get { return PodeAbrirSumario(); } }

        public bool boolImprimir
        {
            get { return this.SumarioAvaliacaoMedicaCTINEO.IsNotNull(); }
        }

        public wrpSumarioAvaliacaoMedicaCTINEO SumarioAvaliacaoMedicaCTINEO
        {
            get
            {
                return this._sumarioavaliacaomedicactineo;
            }
            set
            {
                this._sumarioavaliacaomedicactineo = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.SumarioAvaliacaoMedicaCTINEO);
            }
        }

        public DateTime? DiaNascimento
        {
            get
            {
                if (this._dianascimento.IsNull() && this._sumarioavaliacaomedicactineo.DataNascimento.HasValue)
                    this._dianascimento = this._sumarioavaliacaomedicactineo.DataNascimento.Value;
                return this._dianascimento;
            }
            set
            {
                this._dianascimento = value;
                if (_horanascimento.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicactineo.DataNascimento = DateTime.Parse(value.Value.ToShortDateString() + " " + _horanascimento.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicactineo.DataNascimento = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.DiaNascimento);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Idade);
            }
        }
        public DateTime? HoraNascimento
        {
            get
            {
                if (this._horanascimento.IsNull() && this._sumarioavaliacaomedicactineo.DataNascimento.HasValue)
                    this._horanascimento = this._sumarioavaliacaomedicactineo.DataNascimento.Value;
                return this._horanascimento;
            }
            set
            {
                this._horanascimento = value;
                if (this._dianascimento.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicactineo.DataNascimento = DateTime.Parse(this._dianascimento.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicactineo.DataNascimento = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.HoraNascimento);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Idade);
            }
        }

        public string Idade
        {
            get
            {
                if (this._sumarioavaliacaomedicactineo.DataNascimento.HasValue)
                {
                    this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraIdade);
                    return new Age(this._sumarioavaliacaomedicactineo.DataNascimento.Value).GetTime();
                }
                return new Age(DateTime.Now).GetTime();
            }
        }

        public wrpAdmissaoAssistencialProcedenciaCTINEOCollection ProcedenciaCollection
        {
            get
            {
                if (this._procedenciacollection.IsNull())
                {
                    IRepositorioDeProcedenciaCTINEO rep = ObjectFactory.GetInstance<IRepositorioDeProcedenciaCTINEO>();
                    this._procedenciacollection = new wrpAdmissaoAssistencialProcedenciaCTINEOCollection(rep.List());
                }
                return this._procedenciacollection;
            }
        }
        public wrpAdmissaoAssistencialProcedenciaCTINEO Procedencia
        {
            get { return this._procedencia; }
            set
            {
                this._procedencia = value;
                if (this._sumarioavaliacaomedicactineo.Procedencia != value)
                    this._sumarioavaliacaomedicactineo.Procedencia = this._procedencia;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Procedencia);
            }
        }

        public bool MostraIdade
        {
            get
            {
                if (this._sumarioavaliacaomedicactineo.DataNascimento.HasValue && this._sumarioavaliacaomedicactineo.DataNascimento.Value >= DateTime.Now.AddDays(-5))
                    return true;
                return false;
            }
        }

        public bool Vaginal
        {
            get
            {
                if (this._sumarioavaliacaomedicactineo.TipoParto.HasValue)
                    return (this._sumarioavaliacaomedicactineo.TipoParto.Value == TipoPartoRN.Vaginal);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicactineo.TipoParto = TipoPartoRN.Vaginal;
                    this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicactineo.TipoParto = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaUrgencia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaEletiva);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Vaginal);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MotivoCesarianaUrgencia);
            }
        }
        public bool CesarianaEletiva
        {
            get
            {
                if (this._sumarioavaliacaomedicactineo.TipoParto.HasValue)
                    return (this._sumarioavaliacaomedicactineo.TipoParto.Value == TipoPartoRN.CesarianaEletiva);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicactineo.TipoParto = TipoPartoRN.CesarianaEletiva;
                    this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicactineo.TipoParto = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Vaginal);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaUrgencia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaEletiva);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MotivoCesarianaUrgencia);
            }
        }
        public bool CesarianaUrgencia
        {
            get
            {
                if (this._sumarioavaliacaomedicactineo.TipoParto.HasValue)
                    return (this._sumarioavaliacaomedicactineo.TipoParto.Value == TipoPartoRN.CesarianaUrgencia);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicactineo.TipoParto = TipoPartoRN.CesarianaUrgencia;
                else
                {
                    this._sumarioavaliacaomedicactineo.TipoParto = null;
                    this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia = string.Empty;
                }

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Vaginal);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaEletiva);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaUrgencia);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MotivoCesarianaUrgencia);
            }
        }
        public string MotivoCesarianaUrgencia
        {
            get
            {
                return this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia;
            }
            set
            {
                this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MotivoCesarianaUrgencia);
            }
        }

        public vmSumarioAvaliacaoMedicaCTINEOExameFisico vmSumarioAvaliacaoMedicaCTINEOExameFisico
        {
            get
            {
                if (this._vmexamefisicosumarioavaliacaomedicactineo.IsNull())
                    this._vmexamefisicosumarioavaliacaomedicactineo = new vmSumarioAvaliacaoMedicaCTINEOExameFisico(this._sumarioavaliacaomedicactineo, this);

                return this._vmexamefisicosumarioavaliacaomedicactineo;
            }
        }
        public vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO
        {
            get
            {
                if (this._vmdiagnosticohipotesesumarioavaliacaomedicactineo.IsNull())
                    this._vmdiagnosticohipotesesumarioavaliacaomedicactineo = new vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO(this._sumarioavaliacaomedicactineo);

                return this._vmdiagnosticohipotesesumarioavaliacaomedicactineo;
            }
        }

        public IList<RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>> Tabs
        {
            get
            {
                if (this._tabs.IsNull())
                    this._montatabs();
                return this._tabs;
            }
        }
        public TabsSumarioAvaliacaoMedicaCTINEO? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraFinalizar);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.TipoTabSelecionada);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Idade);
            }
        }

        public bool MostraMarcaDaguaRelatorio
        {
            get
            {
                return this.SumarioAvaliacaoMedicaCTINEO.DataEncerramento.IsNull();
            }
        }
        public bool MostraRelatorioFinalizado
        {
            get
            {
                return this.SumarioAvaliacaoMedicaCTINEO.DataEncerramento.IsNotNull();
            }
        }
        public bool MostraFinalizar
        {
            get { return this._tipotabselecionada == TabsSumarioAvaliacaoMedicaCTINEO.FinalizarImprimir && this._sumarioavaliacaomedicactineo.DataEncerramento.IsNull(); }
        }
        public bool MostraImprimir
        {
            get { return this._tipotabselecionada == TabsSumarioAvaliacaoMedicaCTINEO.FinalizarImprimir && this._sumarioavaliacaomedicactineo.DataEncerramento.IsNotNull(); }
        }
        #endregion

        #region ----- Métodos Privados -----     

        private bool PodeAbrirSumario()
        {
            if (this._usuario.IsNotNull() && this._usuario.Prestador.IsNotNull() && !this._usuario.Prestador.IsNurse)
            {
                if (this._sumarioavaliacaomedicactineo.Usuario.cd_usuario != this._usuario.cd_usuario)
                {
                    if (DXMessageBox.Show("Este sumário foi iniciado pelo Profissional: "
                        + this._sumarioavaliacaomedicactineo.Usuario.nm_usuario
                        + ". Deseja editar este sumário ? ", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this._sumarioavaliacaomedicactineo.Usuario = this._usuario;
                        //_mostrabotao = true;
                        return true;
                    }
                    else
                    {
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraFinalizar);
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraImprimir);
                        return false;
                    }
                }
                //else
                //    _mostrabotao = true;
                return true;
            }
            else
                return false;
        }

        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>>();

            if (this._sumarioavaliacaomedicactineo.DataEncerramento.IsNull() && PodeAbrirSumario())
            {
                //MotivoInternacao
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria,
                    Descricao = TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucMotivoHistoria.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Source = this
                    }
                });

                //DadosDoNascimento
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento,
                    Descricao = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucDadosNascimento.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Source = this
                    }
                });

                //ExameFisico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico,
                    Descricao = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucExameFisico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCTINEO>(x => x.vmSumarioAvaliacaoMedicaCTINEOExameFisico)),
                        Source = this
                    }
                });

                //DiagnosticosHipotesesDiagnosticas
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCTINEO.DiagnosticosHipotesesDiagnosticas,
                    Descricao = TabsSumarioAvaliacaoMedicaCTINEO.DiagnosticosHipotesesDiagnosticas.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucDiagnosticoHipotese.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaCTINEO>(x => x.vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO)),
                        Source = this
                    }
                });

                //PlanoDiagnosticoTerapeutico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaCTINEO.PlanoDiagnosticoTerapeutico,
                    Descricao = TabsSumarioAvaliacaoMedicaCTINEO.PlanoDiagnosticoTerapeutico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucPlanoDiagnosticoTerapeutico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Source = this
                    }
                });
            }

            //FinalizarImprimir
            this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaCTINEO>
            {
                TipoTab = TabsSumarioAvaliacaoMedicaCTINEO.FinalizarImprimir,
                Descricao = TabsSumarioAvaliacaoMedicaCTINEO.FinalizarImprimir.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucResumo.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });
        }

        private void _salva()
        {
            if (this.SalvaDiagnosticoHipotese)
                if (this._vmdiagnosticohipotesesumarioavaliacaomedicactineo.IsNotNull())
                    ObjectFactory.GetInstance<HMV.PEP.Interfaces.ICidService>().verificaSeOCIDJaEstaNaListaDeProblemas(this.vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO.CID.DomainObject, this._vmdiagnosticohipotesesumarioavaliacaomedicactineo.Atendimento, this._usuario.DomainObject);


            if (this._vmexamefisicosumarioavaliacaomedicactineo.IsNotNull())
            {
                if (this._vmexamefisicosumarioavaliacaomedicactineo.CollectionCabecaPescoco.HasItems())
                    foreach (var item in this._vmexamefisicosumarioavaliacaomedicactineo.CollectionCabecaPescoco)
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, new wrpItensCO(item.ItemCO))
                            {
                                Observacao = item.Observacao,
                                IsNormal = item.IsNormal
                            });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacao;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                if (this._vmexamefisicosumarioavaliacaomedicactineo.CollectionOutros.HasItems())
                    foreach (var item in this._vmexamefisicosumarioavaliacaomedicactineo.CollectionOutros)
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, new wrpItensCO(item.ItemCO))
                            {
                                Observacao = item.Observacao,
                                IsNormal = item.IsNormal
                            });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacao;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                if (this._vmexamefisicosumarioavaliacaomedicactineo.CollectionCardioVascular.HasItems())
                    foreach (var item in this._vmexamefisicosumarioavaliacaomedicactineo.CollectionCardioVascular)
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {

                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacao;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                if (this._vmexamefisicosumarioavaliacaomedicactineo.CollectionRespiratorio.HasItems())
                    foreach (var item in this._vmexamefisicosumarioavaliacaomedicactineo.CollectionRespiratorio)
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {

                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacao;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                if (this._vmexamefisicosumarioavaliacaomedicactineo.CollectionOsteoArticular.HasItems())
                    foreach (var item in this._vmexamefisicosumarioavaliacaomedicactineo.CollectionOsteoArticular)
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {

                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, new wrpItensCO(item.ItemCO))
                                {
                                    Observacao = item.Observacao,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacao;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }
            }

            this._sumarioavaliacaomedicactineo.Save();
        }
        #endregion

        #region ----- Métodos Públicos -----
        public bool Imprimir()
        {
            if (DXMessageBox.Show("Deseja finalizar o Sumário?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;

            //this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Clear();
            //this._sumarioavaliacaomedicactineo.DomainObject.SumarioAvaliacaoMedicaCTINEOItens.Clear();
            //this._sumarioavaliacaomedicactineo.Save();

            List<string> erros = new List<string>();

            bool setou = false;

            if (this._sumarioavaliacaomedicactineo.Procedencia.IsNull())
            {
                erros.Add("É necessário informar a Procedência na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria;
                    setou = true;
                }
            }
            else
                if (this._sumarioavaliacaomedicactineo.Procedencia.IsOutros == SimNao.Sim)
                    if (this._sumarioavaliacaomedicactineo.ProcedenciaOutros.IsEmptyOrWhiteSpace())
                    {
                        erros.Add("É necessário informar o campo 'Procedência Outros' na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria;
                            setou = true;
                        }
                    }

            if (this._sumarioavaliacaomedicactineo.MotivoInternacao.IsEmptyOrWhiteSpace())
            {
                erros.Add("É necessário informar o Motivo de Internação na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria;
                    setou = true;
                }
            }

            if (this._sumarioavaliacaomedicactineo.HistoriaAtual.IsEmptyOrWhiteSpace())
            {
                erros.Add("É necessário informar a História Atual na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.MotivoInternacaoHistoria;
                    setou = true;
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!this._sumarioavaliacaomedicactineo.DataNascimento.HasValue)
            {
                erros.Add("É necessário informar a Data de Nascimento na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                    setou = true;
                }
            }

            if (!this._sumarioavaliacaomedicactineo.Peso.HasValue)
            {
                erros.Add("É necessário informar o Peso na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                    setou = true;
                }
            }
            /* if (!this._sumarioavaliacaomedicactineo. .HasValue)
             {
                 erros.Add("É necessário informar a Data de Nascimento na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                 if (!setou)
                 {
                     this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                     setou = true;
                 }
             }*/

            if (this._sumarioavaliacaomedicactineo.TipoParto.IsNull())
            {
                erros.Add("É necessário informar o Tipo de Parto na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                    setou = true;
                }
            }
            else
                if (this._sumarioavaliacaomedicactineo.TipoParto == TipoPartoRN.CesarianaUrgencia)
                    if (this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia.IsEmptyOrWhiteSpace())
                    {
                        erros.Add("É necessário informar o Motivo na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                            setou = true;
                        }
                    }

            if (!this._sumarioavaliacaomedicactineo.IdadeDias.HasValue && !this._sumarioavaliacaomedicactineo.IdadeSemana.HasValue)
            {
                if (this._sumarioavaliacaomedicactineo.IdadeDesconhecida == SimNao.Nao)
                {
                    erros.Add("É necessário informar Idade Gestacional ou Desconhecido na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                        setou = true;
                    }
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicactineo.IdadeDias < 0 || this._sumarioavaliacaomedicactineo.IdadeDias > 6)
                {
                    erros.Add("É necessário informar corretamente o campo Idade Gestacional Dia na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                        setou = true;
                    }
                }
                if (this._sumarioavaliacaomedicactineo.IdadeSemana < 4 || this._sumarioavaliacaomedicactineo.IdadeDias > 42)
                {
                    erros.Add("É necessário informar corretamente o campo Idade Gestacional Semanas na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                        setou = true;
                    }
                }
            }

            if (!this._sumarioavaliacaomedicactineo.ApgarPrimeiro.HasValue)
            {
                erros.Add("É necessário informar APGAR 1º min na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicactineo.ApgarPrimeiro < 0 || this._sumarioavaliacaomedicactineo.ApgarPrimeiro > 10)
                {
                    erros.Add("É necessário informar corretamente o campo APGAR 1º min na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                        setou = true;
                    }
                }
            }

            if (!this._sumarioavaliacaomedicactineo.ApgarQuinto.HasValue)
            {
                erros.Add("É necessário informar APGAR 5º min na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                    setou = true;
                }
            }
            else
            {
                if (this._sumarioavaliacaomedicactineo.ApgarQuinto < 0 || this._sumarioavaliacaomedicactineo.ApgarQuinto > 10)
                {
                    erros.Add("É necessário informar corretamente o campo APGAR 5º min na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DadosNascimento;
                        setou = true;
                    }
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNotNull())
            {
                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FC.HasValue)
                {
                    erros.Add("É necessário informar a FC na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }
                else
                {
                    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FC < 30 || this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FC > 300)
                    {
                        erros.Add("É necessário informar corretamente o campo FC na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                            setou = true;
                        }
                    }
                }

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FR.HasValue)
                {
                    erros.Add("É necessário informar a FR na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }
                else
                {
                    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FR < 5 || this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FR > 100)
                    {
                        erros.Add("É necessário informar corretamente o campo FR na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                            setou = true;
                        }
                    }
                }

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX.HasValue)
                {
                    erros.Add("É necessário informar a TAX na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }
                else
                {
                    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX < 34 || this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.TAX > 42)
                    {
                        erros.Add("É necessário informar corretamente o campo TAX na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                            setou = true;
                        }
                    }
                }

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT.HasValue)
                {
                    erros.Add("É necessário informar a SAT na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }
                else
                {
                    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT < 0 || this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.SAT > 100)
                    {
                        erros.Add("É necessário informar corretamente o campo SAT na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                            setou = true;
                        }
                    }
                }

                //if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM.HasValue)
                //{
                //    erros.Add("É necessário informar o PAM na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                //    if (!setou)
                //    {
                //        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                //        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                //        setou = true;
                //    }
                //}
                //else
                //{
                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM.HasValue)
                    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM < 0 || this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PAM > 120)
                    {
                        erros.Add("É necessário informar corretamente o campo PAM na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                            setou = true;
                        }
                    }
                //}

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.DOR.HasValue)
                {
                    erros.Add("É necessário informar a DOR na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FaciesObservacao.IsEmptyOrWhiteSpace() && !this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsFacies.HasValue)
                {
                    erros.Add("É necessário informar a Facies na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorRosada.HasValue && !this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorPalida.HasValue && !this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianose.HasValue && !this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianoseGeneralizada.HasValue)
                {
                    erros.Add("É necessário informar a Cor na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.HasValue)
                {
                    erros.Add("É necessário informar a Atividade na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }

                if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Pele.HasValue)
                {
                    erros.Add("É necessário informar a Pele na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                        setou = true;
                    }
                }
                else
                {
                    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Pele == Pele.Alteracoes)
                        if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PeleOutros.IsEmptyOrWhiteSpace())
                        {
                            erros.Add("É necessário informar as alterações da Pele na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                            if (!setou)
                            {
                                this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                                this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                                setou = true;
                            }
                        }
                }

                //if (!this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical.HasValue
                //    && this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.UmbilicalAlteracoes.IsEmptyOrWhiteSpace())
                //{
                //    erros.Add("É necessário informar o Coto Umbilical na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                //    if (!setou)
                //    {
                //        this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                //        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                //        setou = true;
                //    }
                //}
                //else
                //    if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical == SimNao.Nao)
                //        if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.UmbilicalAlteracoes.IsEmptyOrWhiteSpace())
                //        {
                //            erros.Add("É necessário informar as Alterações do Coto Umbilical na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                //            if (!setou)
                //            {
                //                this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                //                this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                //                setou = true;
                //            }
                //        }

                //var teste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.CabecaPescoco == SimNao.Sim && (y.IsNormal.IsNull() || y.IsNormal == SimNao.Nao) && y.Observacao.IsEmptyOrWhiteSpace()).ToList();
                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => (y.IsNormal.IsNull() || y.IsNormal == SimNao.Nao) && y.Observacao.IsEmptyOrWhiteSpace())
                    .Count(x => x.ItemCO.CabecaPescoco == SimNao.Sim) > 0)
                {
                    this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte1;
                    erros.Add("É necessário preencher todos os itens Cabeça e Pescoço na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                }

                if (erros.Count > 0)
                {
                    string ret = string.Empty;
                    erros.Each(x => { ret += x + Environment.NewLine; });
                    DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => (y.IsNormal.IsNull() || y.IsNormal == SimNao.Nao) && y.Observacao.IsEmptyOrWhiteSpace())
                    .Count(x => x.ItemCO.Respiratorio == SimNao.Sim) > 0)
                {
                    this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte2;
                    erros.Add("É necessário preencher todos os itens Respiratório na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                }

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => (y.IsNormal.IsNull() || y.IsNormal == SimNao.Nao) && y.Observacao.IsEmptyOrWhiteSpace())
                    .Count(x => x.ItemCO.Cardio == SimNao.Sim) > 0)
                {
                    this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte2;
                    erros.Add("É necessário preencher todos os itens Cardio Vascular na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                }

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => (y.IsNormal.IsNull() || y.IsNormal == SimNao.Nao) && y.Observacao.IsEmptyOrWhiteSpace())
                    .Count(x => x.ItemCO.Outros == SimNao.Sim) > 0)
                {
                    this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte2;
                    erros.Add("É necessário preencher todos os itens Outros na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                }

                if (erros.Count > 0)
                {
                    string ret = string.Empty;
                    erros.Each(x => { ret += x + Environment.NewLine; });
                    DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => (y.IsNormal.IsNull() || y.IsNormal == SimNao.Nao) && y.Observacao.IsEmptyOrWhiteSpace())
                    .Count(x => x.ItemCO.OsteoArticular == SimNao.Sim) > 0)
                {
                    this.vmSumarioAvaliacaoMedicaCTINEOExameFisico.TipoTabSelecionada = vmSumarioAvaliacaoMedicaCTINEOExameFisico.TabsExameFisico.Parte3;
                    erros.Add("É necessário preencher todos os itens Osteoarticular na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico.GetEnumDescription() + ".");
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.ExameFisico;
                }

                if (erros.Count > 0)
                {
                    string ret = string.Empty;
                    erros.Each(x => { ret += x + Environment.NewLine; });
                    DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (this._sumarioavaliacaomedicactineo.Atendimento.Cid.IsNull())
            {
                erros.Add("É necessário informar o CID do Atendimento na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.DiagnosticosHipotesesDiagnosticas.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.DiagnosticosHipotesesDiagnosticas;
                    setou = true;
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (this._sumarioavaliacaomedicactineo.Exames.IsEmptyOrWhiteSpace() && this._sumarioavaliacaomedicactineo.Conduta.IsEmptyOrWhiteSpace())
            {
                erros.Add("É necessário informar Exames ou Conduta na aba de " + TabsSumarioAvaliacaoMedicaCTINEO.PlanoDiagnosticoTerapeutico.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaCTINEO.PlanoDiagnosticoTerapeutico;
                    setou = true;
                }
            }

            if (erros.Count > 0)
            {
                string ret = string.Empty;
                erros.Each(x => { ret += x + Environment.NewLine; });
                DXMessageBox.Show(ret.TrimEnd(Environment.NewLine.ToCharArray()), "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                this._sumarioavaliacaomedicactineo.DataEncerramento = DateTime.Now;
                this._tabs = null;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraFinalizar);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraImprimir);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MostraMarcaDaguaRelatorio);
            }

            this._salva();
            DXMessageBox.Show("Finalizado com sucesso!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        #endregion

        #region ----- Commands -----
        protected override void CommandSalvar(object param)
        {
            this._salva();
        }

        //Rotina que COPIA dados do Sumario RN para o Sumario UTINEO.
        protected override void CommandPesquisar(object param)
        {
            wrpSumarioAvaliacaoMedicaRN ultsumariorn = null;

            IRepositorioDeSumarioDeAvaliacaoMedicaRN rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaRN>();
            var ret = rep.OndePacienteIgual(this._atendimento.DomainObject.Paciente);

            if (ret.List().Count(x => x.DataEncerramento.IsNotNull()) > 0)
            {
                ultsumariorn = new wrpSumarioAvaliacaoMedicaRN(ret.List().OrderByDescending(x => x.DataEncerramento).FirstOrDefault());
                rep.Refresh(ultsumariorn.DomainObject);
            }

            if (ultsumariorn.IsNotNull())
            {
                if (ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.IsNotNull())
                {
                    //this._sumarioavaliacaomedicactineo.DataNascimento = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.DataNascimento;
                    this._horanascimento = null;
                    this.DiaNascimento = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.DataNascimento;
                    this._sumarioavaliacaomedicactineo.Peso = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.Peso;
                    this._sumarioavaliacaomedicactineo.ApgarPrimeiro = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.ApgarPrimeiro;
                    this._sumarioavaliacaomedicactineo.ApgarQuinto = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.ApgarQuinto;
                    this._sumarioavaliacaomedicactineo.ApgarDessimo = ultsumariorn.SumarioAvaliacaoMedicaRNSalaParto.ApgarDessimo;
                }

                this._sumarioavaliacaomedicactineo.TipoParto = ultsumariorn.TipoParto;
                this._sumarioavaliacaomedicactineo.MotivoCesarianaUrgencia = ultsumariorn.MotivoCesarianaUrgencia;
                this._sumarioavaliacaomedicactineo.IsForcipe = ultsumariorn.IsForcipe;
                if (ultsumariorn.IsIdadeDesconhecido.IsNotNull())
                {
                    if (this._sumarioavaliacaomedicactineo.IdadeDesconhecida == SimNao.Sim)
                    {
                        this._sumarioavaliacaomedicactineo.IdadeDesconhecida = ultsumariorn.IsIdadeDesconhecido.Value;
                        this._sumarioavaliacaomedicactineo.IdadeSemana = null;
                        this._sumarioavaliacaomedicactineo.IdadeDias = null;
                    }
                    else
                    {
                        this._sumarioavaliacaomedicactineo.IdadeSemana = ultsumariorn.IdadeSemanas;
                        this._sumarioavaliacaomedicactineo.IdadeDias = ultsumariorn.IdadeDias;
                        this._sumarioavaliacaomedicactineo.IdadeDesconhecida = SimNao.Nao;
                    }
                }
                else
                {
                    this._sumarioavaliacaomedicactineo.IdadeSemana = ultsumariorn.IdadeSemanas;
                    this._sumarioavaliacaomedicactineo.IdadeDias = ultsumariorn.IdadeDias;
                    this._sumarioavaliacaomedicactineo.IdadeDesconhecida = SimNao.Nao;
                }

                if (ultsumariorn.Atendimento.SumarioAvaliacaoMedicaRNMVRecemNascido.IsNotNull())
                    this._sumarioavaliacaomedicactineo.DataNascimento = ultsumariorn.Atendimento.SumarioAvaliacaoMedicaRNMVRecemNascido.DataNascimento;              

                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNull())
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico = new wrpSumarioAvaliacaoMedicaCTINEOExameFisico(this._sumarioavaliacaomedicactineo);

                if (ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsNotNull())
                {
                    if (ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsFacies == SimNao.Sim)
                        this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsFacies = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsFacies;
                    else
                        this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.FaciesObservacao = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.FaciesObservacao;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorRosada = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsCorRosada;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorPalida = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsCorPalida;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianose = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianose;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsCorCianoseGeneralizada = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.IsCorCianoseGeneralizada;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.Tonus;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.Atividade;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Observacao = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.Observacao;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.Pele = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.Pele;
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.PeleOutros = ultsumariorn.SumarioAvaliacaoMedicaRNExameFisico.PeleOutros;
                }

                //this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Clear();

                if (ultsumariorn.SumarioAvaliacaoMedicaRNItens.HasItems())
                {
                    foreach (var item in ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.CabecaPescoco == SimNao.Sim))
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(
                                new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, item.ItemCO)
                                {
                                    Observacao = item.Observacoes,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacoes;
                            jaexiste.IsNormal = item.IsNormal; 
                        }
                    }

                    foreach (var item in ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.Respiratorio == SimNao.Sim))
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(
                                new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, item.ItemCO)
                                {
                                    Observacao = item.Observacoes,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacoes;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                    foreach (var item in ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.Cardio == SimNao.Sim))
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(
                                new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, item.ItemCO)
                                {
                                    Observacao = item.Observacoes,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacoes;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                    foreach (var item in ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.Outros == SimNao.Sim))
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(
                                new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, item.ItemCO)
                                {
                                    Observacao = item.Observacoes,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacoes;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }

                    foreach (var item in ultsumariorn.SumarioAvaliacaoMedicaRNItens.Where(x => x.ItemCO.OsteoArticular == SimNao.Sim))
                    {
                        var jaexiste = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(x => x.ItemCO.ID == item.ItemCO.ID).FirstOrDefault();
                        if (jaexiste.IsNull())
                        {
                            this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Add(
                                new wrpSumarioAvaliacaoMedicaCTINEOItem(this._sumarioavaliacaomedicactineo, item.ItemCO)
                                {
                                    Observacao = item.Observacoes,
                                    IsNormal = item.IsNormal
                                });
                        }
                        else
                        {
                            jaexiste.Observacao = item.Observacoes;
                            jaexiste.IsNormal = item.IsNormal;
                        }
                    }
                }

                if (_vmexamefisicosumarioavaliacaomedicactineo.IsNotNull())
                {
                    this._vmexamefisicosumarioavaliacaomedicactineo._collectioncabecapescoco = null;
                    this._vmexamefisicosumarioavaliacaomedicactineo._collectioncardiovascular = null;
                    this._vmexamefisicosumarioavaliacaomedicactineo._collectionosteoarticular = null;
                    this._vmexamefisicosumarioavaliacaomedicactineo._collectionoutros = null;
                    this._vmexamefisicosumarioavaliacaomedicactineo._collectionrespiratorio = null;
                    this._vmexamefisicosumarioavaliacaomedicactineo.RefreshViewModel();
                    //this._vmdiagnosticohipotesesumarioavaliacaomedicactineo.RefreshViewModel();
                }

                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MotivoCesarianaUrgencia);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Vaginal);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaUrgencia);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.CesarianaEletiva);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.MotivoCesarianaUrgencia);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.DiaNascimento);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.HoraNascimento);
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEO>(x => x.Idade);
            }
        }
        #endregion

        #region ----- Classes -----
        public class Item : NotifyPropertyChanged
        {
            public ItensCO ItemCO { get; set; }
            public SimNao? IsNormal { get; set; }
            private string _observacao { get; set; }

            public string Observacao
            {
                get
                {
                    return this._observacao;
                }
                set
                {
                    this._observacao = value;
                    if (value.IsNotEmptyOrWhiteSpace())
                        IsNormal = SimNao.Nao;

                    base.OnPropertyChanged<Item>(x => x.Observacao);
                    base.OnPropertyChanged<Item>(x => x.Normal);
                }
            }

            public bool Normal
            {
                get
                {
                    return (IsNormal == SimNao.Sim);
                }
                set
                {
                    if (value)
                    {
                        IsNormal = SimNao.Sim;
                        this.Observacao = string.Empty;
                    }
                    else
                        IsNormal = SimNao.Nao;

                    base.OnPropertyChanged<Item>(x => x.Observacao);
                    base.OnPropertyChanged<Item>(x => x.IsNormal);
                }
            }
        }
        #endregion
    }
}
