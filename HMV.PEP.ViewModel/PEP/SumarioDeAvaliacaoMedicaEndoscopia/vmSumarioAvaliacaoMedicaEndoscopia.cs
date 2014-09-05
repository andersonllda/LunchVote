using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Domain.Repository.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.ProcessosEnfermagem.AdmissaoAssistencialDeEndoscopia;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using StructureMap;
using HMV.ProcessosEnfermagem.ViewModel;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;


namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmSumarioAvaliacaoMedicaEndoscopia : ViewModelBase
    {
        #region Enum
        public enum TabsSumarioAvaliacaoMedicaEndoscopia
        {
            [Description("Procedimento")]
            Procedimento,
            [Description("Alergias")]
            Alergias,
            [Description("Perfil Psico-social")]
            PerfilPsicoSocial,
            [Description("Comorbidades")]
            Comorbidades,
            [Description("História Familiar")]
            HistoriaFamiliar,
            [Description("Medicamentos Habituais")]
            MedicamentosEmUso,
            [Description("Exame Físico")]
            ExameFisico,
            [Description("Visualizar")]
            Visualizar
        }
        #endregion

        #region Contrutor
        public vmSumarioAvaliacaoMedicaEndoscopia(Atendimento pAtendimento, Paciente pPaciente, Usuarios pUsuario, bool pIsCorpoClinico)
        {
            this._usuario = new wrpUsuarios(pUsuario);
            this._paciente = new wrpPaciente(pPaciente);
            this._iscorpoclinico = pIsCorpoClinico;
            this._atendimento = new wrpAtendimento(pAtendimento);

            IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia rep = ObjectFactory.GetInstance<IRepositorioDeSumarioDeAvaliacaoMedicaEndoscopia>();
            var ret = rep.OndeIdAtendimentoIgual(pAtendimento.ID).Single();
            if (ret.IsNotNull())
                this._sumarioavaliacaomedicaendoscopia = new wrpSumarioAvaliacaoMedicaEndoscopia(ret);

            if (this._sumarioavaliacaomedicaendoscopia.IsNull())
            {
                this._sumarioavaliacaomedicaendoscopia = new wrpSumarioAvaliacaoMedicaEndoscopia(this._atendimento, this._usuario, this._paciente);
                this._sumarioavaliacaomedicaendoscopia.DataInclusao = DateTime.Now;
                this._copiardadosultimaadmissao();
                this._novo = true;
            }

            IRepositorioDeEventoSumarioAvaliacaoEndoscopia repe = ObjectFactory.GetInstance<IRepositorioDeEventoSumarioAvaliacaoEndoscopia>();
            this._evento = new wrpEventoSumarioAvaliacaoMedicaEndoscopia(repe.Single());
        }

        private void _copiardadosultimaadmissao()
        {
            wrpAdmissaoAssistencialEndoscopia ultadm = null;
            if (this._atendimento.AdmissaoAssistencialEndoscopia.HasItems())
                if (this._atendimento.AdmissaoAssistencialEndoscopia.Count(x => x.DataExclusao.IsNull()) > 0)
                    ultadm = this._atendimento.AdmissaoAssistencialEndoscopia.Where(x => x.DataExclusao.IsNull()).OrderByDescending(x => x.DataEncerramento).FirstOrDefault();

            if (ultadm.IsNotNull())
            {
                this._sumarioavaliacaomedicaendoscopia.Altura = ultadm.Altura.HasValue ? ultadm.Altura.Value : 0;
                this._sumarioavaliacaomedicaendoscopia.Ausencia = ultadm.Ausencia;
                this._sumarioavaliacaomedicaendoscopia.Cancer = ultadm.Cancer;
                this._sumarioavaliacaomedicaendoscopia.CancerIntestino = ultadm.CancerIntestino;
                this._sumarioavaliacaomedicaendoscopia.CancerIntestinoObservacao = ultadm.CancerIntestinoObservacao;
                this._sumarioavaliacaomedicaendoscopia.CancerObservacao = ultadm.CancerObservacao;
                this._sumarioavaliacaomedicaendoscopia.Motivo = ultadm.Motivo;
                this._sumarioavaliacaomedicaendoscopia.OutrasComorbidades = ultadm.OutrasComorbidades;
                this._sumarioavaliacaomedicaendoscopia.Outro = ultadm.Outro;
                this._sumarioavaliacaomedicaendoscopia.OutroObservacao = ultadm.OutroObservacao;
                this._sumarioavaliacaomedicaendoscopia.Peso = ultadm.Peso.HasValue ? ultadm.Peso.Value : 0;
                this._sumarioavaliacaomedicaendoscopia.SAT = ultadm.SAT.HasValue ? ultadm.SAT.Value : 0;
                this._sumarioavaliacaomedicaendoscopia.PA = ultadm.PA;
                this._sumarioavaliacaomedicaendoscopia.FR = ultadm.FR.HasValue ? ultadm.FR.Value : 0;
                this._sumarioavaliacaomedicaendoscopia.FC = ultadm.FC.HasValue ? ultadm.FC.Value : 0;
                this._sumarioavaliacaomedicaendoscopia.TAX = ultadm.TAX.HasValue ? ultadm.TAX.Value : 0;

                foreach (var item in ultadm.ProcedimentosEndoscopia)
                    this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.Add(
                        new wrpSumarioAvaliacaoMedicaProcedimentosEndoscopia(this._sumarioavaliacaomedicaendoscopia)
                        {
                            Data = item.Data,
                            Descricao = item.Descricao,
                            IdAtendimento = ultadm.Atendimento.ID,
                            IdCirurgia = item.IdCirurgia,
                            Ano = item.Ano,
                            Observacao = item.Observacao 
                        });
                
                //this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado = ultadm.ProcedimetoPlanejado;              
                this._sumarioavaliacaomedicaendoscopia.EndoscopiaDigestivaAlta = ultadm.EndoscopiaDigestivaAlta;
                this._sumarioavaliacaomedicaendoscopia.Colonoscopia = ultadm.Colonoscopia;
                this._sumarioavaliacaomedicaendoscopia.Colangiopancreatografia = ultadm.Colangiopancreatografia ;
                this._sumarioavaliacaomedicaendoscopia.Laringoscopia = ultadm.Laringoscopia;
                this._sumarioavaliacaomedicaendoscopia.Fibrobroncospia = ultadm.Fibrobroncospia;
                this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaAlta = ultadm.EcoendoscopiaAlta;
                this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaBaixa = ultadm.EcoendoscopiaBaixa;
                this._sumarioavaliacaomedicaendoscopia.Retossigmoidoscopia = ultadm.Retossigmoidoscopia;
                this._sumarioavaliacaomedicaendoscopia.Enema = ultadm.Enema;

                foreach (var item in ultadm.AdmissaoItemEndoscopia)
                    this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Add(
                        new wrpSumarioAvaliacaoMedicaItemEndoscopia(this._sumarioavaliacaomedicaendoscopia, item.ItemEndoscopia)
                        {
                            Nega = item.Nega,
                            Observacao = item.Observacao
                        });
            }
        }
        #endregion

        #region Propriedades Publicas

        public wrpSumarioAvaliacaoMedicaEndoscopia SumarioAvaliacaoMedicaEndoscopia
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia;
            }
            set
            {
                this._sumarioavaliacaomedicaendoscopia = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.SumarioAvaliacaoMedicaEndoscopia);
            }
        }

        public wrpAtendimento Atendimento
        {
            get { return this._atendimento; }
        }

        public wrpUsuarios Usuarios
        {
            get { return this._usuario; }
        }

        public IList<RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
        }

        public TabsSumarioAvaliacaoMedicaEndoscopia? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                //if (this._tipotabselecionada == TabsSumarioAvaliacaoMedicaEndoscopia.Visualizar)
                //    this.MostraFinalizar = true;
                //else
                //    this.MostraFinalizar = false;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.MostraFinalizar);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.TipoTabSelecionada);
            }
        }

        public bool MostraFinalizar
        {
            get { return this._tipotabselecionada == TabsSumarioAvaliacaoMedicaEndoscopia.Visualizar && this._sumarioavaliacaomedicaendoscopia.DataEncerramento.IsNull(); }
        }

        public bool MostraImprimir
        {
            get { return this._tipotabselecionada == TabsSumarioAvaliacaoMedicaEndoscopia.Visualizar && this._sumarioavaliacaomedicaendoscopia.DataEncerramento.IsNotNull(); }
        }

        public vmSumarioAvaliacaoMedicaProcedimentoEndoscopia vmSumarioAvaliacaoMedicaProcedimentoEndoscopia
        {
            get
            {
                if (this._vmsumarioavaliacaomedicaprocedimentoendoscopia.IsNull())
                    this._vmsumarioavaliacaomedicaprocedimentoendoscopia = new vmSumarioAvaliacaoMedicaProcedimentoEndoscopia(this._sumarioavaliacaomedicaendoscopia);
                return this._vmsumarioavaliacaomedicaprocedimentoendoscopia;
            }
        }

        public vmAlergiasEvento vmAlergiasEvento
        {
            get
            {
                if (this._vmalergiasevento.IsNull())
                {
                    wrpAlergiaEventoCollection _AlergiaCollection = null;
                    IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                    repa.OndeChaveIgual(this._sumarioavaliacaomedicaendoscopia.ID);
                    repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedicaEndoscopia);
                    var reta = repa.List();
                    if (reta.IsNotNull())
                        _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

                    this._vmalergiasevento = new vmAlergiasEvento(false, this._paciente, this._usuario, true, TipoEvento.SumarioAvaliacaoMedicaEndoscopia, _AlergiaCollection
                        , this._sumarioavaliacaomedicaendoscopia.ID, _atendimento);
                   
                    if (this._novo)
                        this._vmalergiasevento.MarcarTodasAlergias();
                }
                return this._vmalergiasevento;
            }
        }

        public vmSumarioAvaliacaoMedicaComorbidadesEndoscopia vmSumarioAvaliacaoMedicaComorbidadesEndoscopia
        {
            get
            {
                if (this._vmsumarioavaliacaomedicacomorbidadesendoscopia.IsNull())
                    this._vmsumarioavaliacaomedicacomorbidadesendoscopia = new vmSumarioAvaliacaoMedicaComorbidadesEndoscopia(this._sumarioavaliacaomedicaendoscopia);
                return this._vmsumarioavaliacaomedicacomorbidadesendoscopia;
            }
        }

        public vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia
        {
            get
            {
                if (this._vmsumarioavaliacaomedicahistoriafamiliarendoscopia.IsNull())
                    this._vmsumarioavaliacaomedicahistoriafamiliarendoscopia = new vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia(this._sumarioavaliacaomedicaendoscopia);
                return this._vmsumarioavaliacaomedicahistoriafamiliarendoscopia;
            }
        }

        public vmSumarioAvaliacaoMedicaMedicamentosEndoscopia vmSumarioAvaliacaoMedicaMedicamentosEndoscopia
        {
            get
            {
                if (this._vmsumarioavaliacaomedicamedicamentosendoscopia.IsNull())
                {
                    this._vmsumarioavaliacaomedicamedicamentosendoscopia = new vmSumarioAvaliacaoMedicaMedicamentosEndoscopia(this._sumarioavaliacaomedicaendoscopia);
                    if (this._novo)
                        this._vmsumarioavaliacaomedicamedicamentosendoscopia.vmMedicamentosEmUsoEvento.MarcarTodosDoAtendimentoAdmissao(this._atendimento.DomainObject);
                }
                return this._vmsumarioavaliacaomedicamedicamentosendoscopia;
            }
        }

        public vmSumarioAvaliacaoMedicaExameFisicoEndoscopia vmSumarioAvaliacaoMedicaExameFisicoEndoscopia
        {
            get
            {
                if (this._vmsumarioavaliacaomedicaexamefisicoendoscopia.IsNull())
                    this._vmsumarioavaliacaomedicaexamefisicoendoscopia = new vmSumarioAvaliacaoMedicaExameFisicoEndoscopia(this._sumarioavaliacaomedicaendoscopia);
                return this._vmsumarioavaliacaomedicaexamefisicoendoscopia;
            }
        }

        //ABA PERFIL-PSICO-SOCIAL
        public ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia> CollectionItemEndoscopiaPerfilPsicoSocial
        {
            get
            {
                if (this._collectionitemendoscopiaperfilpsicosocial.IsNull())
                {
                    IRepositorioDeItemEndoscopia rep = ObjectFactory.GetInstance<IRepositorioDeItemEndoscopia>();
                    rep.FiltraAtivos();
                    rep.FiltraPerfilPsicoSocial();
                    var lista = rep.List();
                    this._collectionitemendoscopiaperfilpsicosocial = new ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia>();
                    lista.Each(x =>
                    {
                        this._collectionitemendoscopiaperfilpsicosocial.Add(new vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia
                        {
                            ItemEndoscopia = x,
                            Nega = this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.HasItems() ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).FirstOrDefault().Nega :
                                   SimNao.Nao : SimNao.Nao,
                            Observacao = this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.HasItems() ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty
                        });
                    });
                }
                return this._collectionitemendoscopiaperfilpsicosocial;
            }
        }

        public bool MostraMarcaDaguaRelatorio
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.DataEncerramento.IsNull();
            }
        }

        public bool MostraRelatorioFinalizado
        {
            get
            {
                return this._sumarioavaliacaomedicaendoscopia.DataEncerramento.IsNotNull();
            }
        }

        //public wrpEventoSumarioAvaliacaoMedicaEndoscopia Evento
        //{
        //    get
        //    {
        //        return this._evento;
        //    }
        //}
        #endregion

        #region Commands
        protected override void CommandSalvar(object param)
        {
            this._salva();
        }
        #endregion

        #region Metodos Públicos
        public bool Imprimir()
        {
            if (DXMessageBox.Show("Deseja finalizar o Sumário?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return false;
            List<string> erros = new List<string>();

            bool setou = false;

            if ((this._sumarioavaliacaomedicaendoscopia.EndoscopiaDigestivaAlta.IsNull() || this._sumarioavaliacaomedicaendoscopia.EndoscopiaDigestivaAlta == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.Colonoscopia.IsNull() || this._sumarioavaliacaomedicaendoscopia.Colonoscopia == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.Colangiopancreatografia.IsNull() || this._sumarioavaliacaomedicaendoscopia.Colangiopancreatografia == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.Laringoscopia.IsNull() || this._sumarioavaliacaomedicaendoscopia.Laringoscopia == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.Fibrobroncospia.IsNull() || this._sumarioavaliacaomedicaendoscopia.Fibrobroncospia == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaAlta.IsNull() || this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaAlta == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaBaixa.IsNull() || this._sumarioavaliacaomedicaendoscopia.EcoendoscopiaBaixa == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.Retossigmoidoscopia.IsNull() || this._sumarioavaliacaomedicaendoscopia.Retossigmoidoscopia == SimNao.Nao)
                && (this._sumarioavaliacaomedicaendoscopia.Enema.IsNull() || this._sumarioavaliacaomedicaendoscopia.Enema == SimNao.Nao))
            //if (this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado.IsNull() || this._sumarioavaliacaomedicaendoscopia.ProcedimetoPlanejado.Value.IsNull())
            {
                erros.Add("É necessário informar uma das opções na aba de " + TabsSumarioAvaliacaoMedicaEndoscopia.Procedimento.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaEndoscopia.Procedimento;
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

            if (this._vmsumarioavaliacaomedicacomorbidadesendoscopia.IsNotNull())
                foreach (var item in this._vmsumarioavaliacaomedicacomorbidadesendoscopia.CollectionItemEndoscopia)
                {
                    if (item.Observacao.IsEmptyOrWhiteSpace() && item.Nega == SimNao.Nao)
                    {
                        erros.Add("É necessário marcar Nega ou digitar alguma observação do item " + item.ItemEndoscopia.Descricao + " na aba " + TabsSumarioAvaliacaoMedicaEndoscopia.Comorbidades.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaEndoscopia.Comorbidades;
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

            if (this._vmsumarioavaliacaomedicamedicamentosendoscopia.IsNotNull())
                foreach (var item in this._vmsumarioavaliacaomedicamedicamentosendoscopia.CollectionItemEndoscopia)
                {
                    if (item.Observacao.IsEmptyOrWhiteSpace() && item.Nega == SimNao.Nao)
                    {
                        erros.Add("É necessário marcar Nega ou digitar alguma observação do item " + item.ItemEndoscopia.Descricao + " na aba " + TabsSumarioAvaliacaoMedicaEndoscopia.MedicamentosEmUso.GetEnumDescription() + ".");
                        if (!setou)
                        {
                            this._tipotabselecionada = TabsSumarioAvaliacaoMedicaEndoscopia.MedicamentosEmUso;
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

            if ((this._sumarioavaliacaomedicaendoscopia.Cancer == null || this._sumarioavaliacaomedicaendoscopia.Cancer == SimNao.Nao) &&
                (this._sumarioavaliacaomedicaendoscopia.CancerIntestino == null || this._sumarioavaliacaomedicaendoscopia.CancerIntestino == SimNao.Nao) &&
                (this._sumarioavaliacaomedicaendoscopia.Outro == null || this._sumarioavaliacaomedicaendoscopia.Outro == SimNao.Nao) &&
                (this._sumarioavaliacaomedicaendoscopia.Ausencia == null || this._sumarioavaliacaomedicaendoscopia.Ausencia == SimNao.Nao))
            {
                erros.Add("É necessário informar uma das opções na aba de " + TabsSumarioAvaliacaoMedicaEndoscopia.HistoriaFamiliar.GetEnumDescription() + ".");
                if (!setou)
                {
                    this._tipotabselecionada = TabsSumarioAvaliacaoMedicaEndoscopia.HistoriaFamiliar;
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

            if (this._vmsumarioavaliacaomedicaexamefisicoendoscopia.IsNotNull())
                if (!this._vmsumarioavaliacaomedicaexamefisicoendoscopia.IsValid)
                {
                    erros.Add("É necessário informar os dados na aba de " + TabsSumarioAvaliacaoMedicaEndoscopia.ExameFisico.GetEnumDescription() + ".");
                    if (!setou)
                    {
                        this._tipotabselecionada = TabsSumarioAvaliacaoMedicaEndoscopia.ExameFisico;
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
            
            this._sumarioavaliacaomedicaendoscopia.DataEncerramento = DateTime.Now;
            this._tabs = null;
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.MostraFinalizar);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.MostraImprimir);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.MostraMarcaDaguaRelatorio);

            this._salva();

            DXMessageBox.Show("Finalizado com sucesso!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
        #endregion

        #region Métodos Privados
        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>>();

            //Procedimento
            if (this._sumarioavaliacaomedicaendoscopia.DataEncerramento.IsNull())
            {
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.Procedimento,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.Procedimento.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucProcedimento.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.vmSumarioAvaliacaoMedicaProcedimentoEndoscopia)),
                        Source = this
                    }
                });

                //Alergias
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.Alergias,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.Alergias.GetEnumDescription(),
                    Componente = new Uri(@"/HMV.ProcessosEnfermagem.WPF;component/Views/Alergias/ucAlergiasEvento.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.vmAlergiasEvento)),
                        Source = this
                    }
                });

                //Perfil Psico-social
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.PerfilPsicoSocial,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.PerfilPsicoSocial.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucPerfilPsicoSocial.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Source = this
                    }
                });

                //Comorbidades
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.Comorbidades,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.Comorbidades.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucComorbidades.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.vmSumarioAvaliacaoMedicaComorbidadesEndoscopia)),
                        Source = this
                    }
                });

                //História Familiar
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.HistoriaFamiliar,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.HistoriaFamiliar.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucHistoriaFamiliar.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia)),
                        Source = this
                    }
                });

                //Medicamentos em Uso - Medicamentos Habituais
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.MedicamentosEmUso,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.MedicamentosEmUso.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucMedicamentosEndoscopia.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.vmSumarioAvaliacaoMedicaMedicamentosEndoscopia)),
                        Source = this
                    }
                });

                //Exame Físico
                this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
                {
                    TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.ExameFisico,
                    Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.ExameFisico.GetEnumDescription(),
                    Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucExameFisico.xaml", UriKind.Relative),
                    Binding = new Binding
                    {
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaEndoscopia>(x => x.vmSumarioAvaliacaoMedicaExameFisicoEndoscopia)),
                        Source = this
                    }
                });
            }

            //Visualizar
            this._tabs.Add(new RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>
            {
                TipoTab = TabsSumarioAvaliacaoMedicaEndoscopia.Visualizar,
                Descricao = TabsSumarioAvaliacaoMedicaEndoscopia.Visualizar.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaEndoscopia\ucVisualizar.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });
        }
        private void _salva()
        {
            if (this._vmsumarioavaliacaomedicacomorbidadesendoscopia.IsNotNull())
            {
                foreach (var item in this._vmsumarioavaliacaomedicacomorbidadesendoscopia.CollectionItemEndoscopia)
                {
                    if (this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Count(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID) == 0)
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Add(new wrpSumarioAvaliacaoMedicaItemEndoscopia(this._sumarioavaliacaomedicaendoscopia, new wrpItemEndoscopia(item.ItemEndoscopia))
                        {
                            Nega = item.Nega,
                            Observacao = item.Observacao
                        });
                    else
                    {
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID).SingleOrDefault().Nega = item.Nega;
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID).SingleOrDefault().Observacao = item.Observacao;
                    }
                }

                if (this._vmsumarioavaliacaomedicacomorbidadesendoscopia.CollectionProcedimentosEndoscopia.HasItems())
                    foreach (var proc in this._vmsumarioavaliacaomedicacomorbidadesendoscopia.CollectionProcedimentosEndoscopia)
                    {
                        if (this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.Count(x => x.ID == proc.ID && proc.ID > 0) == 0)
                            this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.Add(proc);
                        else
                        {
                            this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.SingleOrDefault(x => x.ID == proc.ID).Data = proc.Data;
                            this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.SingleOrDefault(x => x.ID == proc.ID).Descricao = proc.Descricao;
                            this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.SingleOrDefault(x => x.ID == proc.ID).Ano = proc.Ano;
                            this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.SingleOrDefault(x => x.ID == proc.ID).Observacao = proc.Observacao;
                        }
                    }

                if (this._vmsumarioavaliacaomedicacomorbidadesendoscopia.vmProcedimentosRealizados.ProcedimentosRealizados.HasItems())
                    foreach (var proc in this._vmsumarioavaliacaomedicacomorbidadesendoscopia.vmProcedimentosRealizados.ProcedimentosRealizados)
                    {
                        if (this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.HasItems())
                        {
                            if (this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.Count(x => x.IdCirurgia == proc.IdCirurgia) == 0)
                                this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.Add(new wrpSumarioAvaliacaoMedicaProcedimentosEndoscopia(this._sumarioavaliacaomedicaendoscopia)
                                {
                                    Data = proc.DataAtendimento.HasValue ? proc.DataAtendimento.Value : DateTime.Now,
                                    Descricao = proc.Procedimento,
                                    IdCirurgia = proc.IdCirurgia,
                                    NomePrestador = proc.NomePrestador,
                                    IdAtendimento = proc.IdAtendimento,
                                    DataAtendimento = proc.DataAtendimento
                                });
                        }
                        else
                            this._sumarioavaliacaomedicaendoscopia.ProcedimentosEndoscopia.Add(new wrpSumarioAvaliacaoMedicaProcedimentosEndoscopia(this._sumarioavaliacaomedicaendoscopia)
                            {
                                Data = proc.DataAtendimento.HasValue ? proc.DataAtendimento.Value : DateTime.Now,
                                Descricao = proc.Procedimento,
                                IdCirurgia = proc.IdCirurgia,
                                NomePrestador = proc.NomePrestador,
                                IdAtendimento = proc.IdAtendimento,
                                DataAtendimento = proc.DataAtendimento
                            });
                    }
            }

            if (this._vmsumarioavaliacaomedicamedicamentosendoscopia.IsNotNull())
                foreach (var item in this._vmsumarioavaliacaomedicamedicamentosendoscopia.CollectionItemEndoscopia)
                {
                    if (this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Count(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID) == 0)
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Add(new wrpSumarioAvaliacaoMedicaItemEndoscopia(this._sumarioavaliacaomedicaendoscopia, new wrpItemEndoscopia(item.ItemEndoscopia))
                        {
                            Nega = item.Nega,
                            Observacao = item.Observacao
                        });
                    else
                    {
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID).SingleOrDefault().Nega = item.Nega;
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID).SingleOrDefault().Observacao = item.Observacao;
                    }
                }


            if (this._collectionitemendoscopiaperfilpsicosocial.IsNotNull())
                foreach (var item in this._collectionitemendoscopiaperfilpsicosocial)
                {
                    if (this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Count(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID) == 0)
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Add(new wrpSumarioAvaliacaoMedicaItemEndoscopia(this._sumarioavaliacaomedicaendoscopia, new wrpItemEndoscopia(item.ItemEndoscopia))
                        {
                            Nega = item.Nega,
                            Observacao = item.Observacao
                        });
                    else
                    {
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID).SingleOrDefault().Nega = item.Nega;
                        this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(x => x.ItemEndoscopia.ID == item.ItemEndoscopia.ID).SingleOrDefault().Observacao = item.Observacao;
                    }
                }

            this._sumarioavaliacaomedicaendoscopia.Paciente.Save();
            this._sumarioavaliacaomedicaendoscopia.Save();

            //Salva AlergiasEvento
            if (this._vmalergiasevento.IsNotNull())
                this._vmalergiasevento.SalvarNovo();                             

            //Salva MedicamentosEmUsoEvento        
            if (this._vmsumarioavaliacaomedicamedicamentosendoscopia.IsNotNull())
                this._vmsumarioavaliacaomedicamedicamentosendoscopia.vmMedicamentosEmUsoEvento.SalvarNovo();           

            if (this._evento.IsNotNull())
                this._evento.Save();
        }
        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        private bool _iscorpoclinico;
        private wrpSumarioAvaliacaoMedicaEndoscopia _sumarioavaliacaomedicaendoscopia;
        private IList<RuntimeTab<TabsSumarioAvaliacaoMedicaEndoscopia>> _tabs;
        private vmSumarioAvaliacaoMedicaProcedimentoEndoscopia _vmsumarioavaliacaomedicaprocedimentoendoscopia;
        private vmSumarioAvaliacaoMedicaMedicamentosEndoscopia _vmsumarioavaliacaomedicamedicamentosendoscopia;
        private vmSumarioAvaliacaoMedicaComorbidadesEndoscopia _vmsumarioavaliacaomedicacomorbidadesendoscopia;
        private vmSumarioAvaliacaoMedicaHistoriaFamiliarEndoscopia _vmsumarioavaliacaomedicahistoriafamiliarendoscopia;
        private vmAlergiasEvento _vmalergiasevento;
        private vmSumarioAvaliacaoMedicaExameFisicoEndoscopia _vmsumarioavaliacaomedicaexamefisicoendoscopia;
        private wrpEventoSumarioAvaliacaoMedicaEndoscopia _evento;
        private ObservableCollection<ItensEndoscopia> _collectionitemendoscopiaperfilpsicosocial;
        private TabsSumarioAvaliacaoMedicaEndoscopia _tipotabselecionada;
        private bool _novo;
        #endregion

        #region ----- Classes -----
        public class ItensEndoscopia : NotifyPropertyChanged
        {
            public ItemEndoscopia ItemEndoscopia { get; set; }
            public SimNao Nega
            {
                get
                {
                    return this._nega;
                }
                set
                {
                    this._nega = value;
                    if (value == SimNao.Sim)
                        this._obs = string.Empty;

                    this.OnPropertyChanged<ItensEndoscopia>(x => x.Observacao);
                    this.OnPropertyChanged<ItensEndoscopia>(x => x.Nega);
                }
            }
            public string Observacao
            {
                get
                {
                    return this._obs;
                }
                set
                {
                    this._obs = value;
                    if (value.IsNotEmptyOrWhiteSpace())
                        this._nega = SimNao.Nao;

                    this.OnPropertyChanged<ItensEndoscopia>(x => x.Nega);
                    this.OnPropertyChanged<ItensEndoscopia>(x => x.Observacao);
                }
            }

            private SimNao _nega;
            private string _obs;
        }
        #endregion
    }
}
