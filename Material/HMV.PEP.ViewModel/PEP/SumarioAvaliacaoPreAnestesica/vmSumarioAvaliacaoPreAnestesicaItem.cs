using System;
using System.Collections.ObjectModel;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Validations;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.PEP.DTO;
using System.Collections.Generic;
using HMV.ProcessosEnfermagem.ViewModel;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica
{
    public class vmSumarioAvaliacaoPreAnestesicaItem : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoPreAnestesicaItem(wrpSumarioAvaliacaoPreAnestesica pSumarioAvaliacaoPreAnestesica
            , wrpAnestesiaGrupo pAnestesiaGrupo, bool pIsCorpoClinico, int pTabIndex, bool pnovo, wrpPaciente pPaciente, bool pImportaNovamente) //, wrpEventoAnestesia pEventoAnestesia
        {
            this.SelecionaTab = pTabIndex;
            this._novo = pnovo;
            this._sumarioavaliacaopreanestesica = pSumarioAvaliacaoPreAnestesica;
            this._anestesiagrupo = pAnestesiaGrupo;
            this._iscorpoclinico = pIsCorpoClinico;
            //this._eventoanestesia = pEventoAnestesia;
            this._carregaitensdinamicos();

            if (this._sumarioavaliacaopreanestesica.NPO.HasValue)
            {
                this._npodata = this._sumarioavaliacaopreanestesica.NPO.Value;
                this._npohora = this._sumarioavaliacaopreanestesica.NPO.Value;
            }

            _importanovamente = pImportaNovamente;
            //rz _atendimento = pAtendimento;
            _paciente = pPaciente;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmExameFisicoEvento _vmexamefisicoevento;
        private wrpSumarioAvaliacaoPreAnestesica _sumarioavaliacaopreanestesica;
        private wrpAnestesiaGrupo _anestesiagrupo;
        private ObservableCollection<ItemDinamico> _itenscollection;
        private ItemDinamico _itemdinamico;
        private vmPreMedicacao _vmpremedicacao;
        private vmAlergiasEvento _vmalergiasevento;
        private vmMedicamentosEmUsoEvento _vmmedicamentosemusoevento;
        private vmProcedimentosRealizados _vmprocedimentosrealizados;
        private DateTime? _npohora;
        private DateTime? _npodata;
        //private bool _mostraprocedimentosrealizados;
        private bool _iscorpoclinico;
        //private wrpEventoAnestesia _eventoanestesia;
        //private string _outros;
        //private string _outrosexame;
        //rz private wrpAtendimento _atendimento;
        private wrpPaciente _paciente;
        private bool _novo;
        private bool _importanovamente;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoPreAnestesica SumarioAvaliacaoPreAnestesica
        {
            get { return _sumarioavaliacaopreanestesica; }
        }

        public wrpAnestesiaGrupo AnestesiaGrupo
        {
            get
            {
                return this._anestesiagrupo;
            }
        }

        public SimNao NaoSeAplica
        {
            get
            {
                if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull() && this._sumarioavaliacaopreanestesica.AvisoCirurgia.Paciente.TipoDoPaciente == TipoPaciente.Pediatrico)
                    return this._anestesiagrupo.NaoSeAplica;
                return SimNao.Nao;
            }
        }

        public bool MostraCheck
        {
            get
            {
                if (this._anestesiagrupo.NaoAvaliado == SimNao.Nao && this._anestesiagrupo.NaoRealizado == SimNao.Nao &&
                    this._anestesiagrupo.NaoSeAplica == SimNao.Nao && this._anestesiagrupo.SemParticularidade == SimNao.Nao &&
                    this._anestesiagrupo.ResultadoIndisponivel == SimNao.Nao && this._anestesiagrupo.Nega == SimNao.Nao && MostraCombo == false)
                    return true;
                return false;
            }
        }

        public bool MostraCombo
        {
            get
            {
                if (this._anestesiagrupo.AnestesiaItens.Count(x => x.ValoresPossiveis != null) > 0)
                    return true;
                return false;
            }
        }

        public double TamanhoGrid
        {
            get
            {
                if (this._anestesiagrupo.MostraProcedimentosRealizados == SimNao.Sim)
                    return 100;
                return double.NaN;
            }
        }

        public ObservableCollection<ItemDinamico> ItensCollection
        {
            get
            {
                return this._itenscollection;
            }
        }

        public ItemDinamico ItemD
        {
            get
            {
                return this._itemdinamico;
            }
            set
            {
                this._itemdinamico = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItemD));
            }
        }

        public vmPreMedicacao vmPreMedicacao
        {
            get
            {
                if (this._vmpremedicacao.IsNull() && this._anestesiagrupo.MostraMedicamentos == SimNao.Sim)
                    this._vmpremedicacao = new vmPreMedicacao(this._sumarioavaliacaopreanestesica);
                return this._vmpremedicacao;
            }
        }

        public vmAlergiasEvento vmAlergiasEvento
        {
            get
            {
                if (this._vmalergiasevento.IsNull() && this._anestesiagrupo.MostraAlergias == SimNao.Sim)
                {
                    wrpAlergiaEventoCollection _AlergiaCollection = null;
                    IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                    repa.OndeChaveIgual(this._sumarioavaliacaopreanestesica.ID);
                    repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica);
                    var reta = repa.List();
                    if (reta.IsNotNull())
                        _AlergiaCollection = new wrpAlergiaEventoCollection(reta);
                    if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull())
                        this._vmalergiasevento = new vmAlergiasEvento(false, this._sumarioavaliacaopreanestesica.AvisoCirurgia.Paciente, this._sumarioavaliacaopreanestesica.UsuarioInclusao
                        , true, TipoEvento.SumarioAvaliacaoPreAnestesica, _AlergiaCollection
                        , this._sumarioavaliacaopreanestesica.ID, null
                        , (this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.IsNull() || this._sumarioavaliacaopreanestesica.Temporario == SimNao.Sim));
                    else
                        this._vmalergiasevento = new vmAlergiasEvento(
                                false,
                                this._sumarioavaliacaopreanestesica.Paciente,
                                this._sumarioavaliacaopreanestesica.UsuarioInclusao,
                                true,
                                TipoEvento.SumarioAvaliacaoPreAnestesica,
                                _AlergiaCollection,
                                this._sumarioavaliacaopreanestesica.ID,
                                null,
                                (this._sumarioavaliacaopreanestesica.Temporario == SimNao.Sim)
                             );

                    if (this._novo)
                        this._vmalergiasevento.MarcarTodasAlergias();
                }
                return this._vmalergiasevento;
            }
        }

        public vmMedicamentosEmUsoEvento vmMedicamentosEmUsoEvento
        {
            get
            {
                if (this._vmmedicamentosemusoevento.IsNull() && this._anestesiagrupo.MostraMedicamentosEmUso == SimNao.Sim)
                {
                    wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;
                    IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                    repp.OndeChaveIgual(this._sumarioavaliacaopreanestesica.ID);
                    repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoPreAnestesica);
                    var ret = repp.List();
                    if (ret.IsNotNull())
                        _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);

                    if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull())
                    {
                        this._vmmedicamentosemusoevento = new vmMedicamentosEmUsoEvento(false, this._sumarioavaliacaopreanestesica.AvisoCirurgia.Paciente
                        , this._sumarioavaliacaopreanestesica.UsuarioInclusao, TipoEvento.SumarioAvaliacaoPreAnestesica, _MedicamentosCollection
                        , this._sumarioavaliacaopreanestesica.ID, true, false, true, true, false, false
                        , pSalvaTemporario: (this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.IsNull() || this._sumarioavaliacaopreanestesica.Temporario == SimNao.Sim)
                        , pNaoTrocaFiltro: true);

                        if ((this._novo && this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.IsNotNull()) || (!_MedicamentosCollection.HasItems() && _importanovamente))
                            this._vmmedicamentosemusoevento.MarcarTodosDoAtendimentoAdmissao(this._sumarioavaliacaopreanestesica.AvisoCirurgia.Atendimento.DomainObject);
                    }
                    else
                        this._vmmedicamentosemusoevento = new vmMedicamentosEmUsoEvento(false, this._sumarioavaliacaopreanestesica.Paciente
                        , this._sumarioavaliacaopreanestesica.UsuarioInclusao, TipoEvento.SumarioAvaliacaoPreAnestesica, _MedicamentosCollection
                        , this._sumarioavaliacaopreanestesica.ID, true, false, true, true, false, false
                        , pSalvaTemporario: (this._sumarioavaliacaopreanestesica.Temporario == SimNao.Sim)
                        , pNaoTrocaFiltro: true);             
                }
                return this._vmmedicamentosemusoevento;
            }
        }

        public vmProcedimentosRealizados vmProcedimentosRealizados
        {
            get
            {
                if (this._vmprocedimentosrealizados.IsNull() && this._anestesiagrupo.MostraProcedimentosRealizados == SimNao.Sim)
                    if (this._sumarioavaliacaopreanestesica.AvisoCirurgia.IsNotNull())
                        this._vmprocedimentosrealizados = new vmProcedimentosRealizados(this._sumarioavaliacaopreanestesica.AvisoCirurgia.Paciente.DomainObject);
                    else if (this._sumarioavaliacaopreanestesica.Paciente.IsNotNull())
                        this._vmprocedimentosrealizados = new vmProcedimentosRealizados(this._sumarioavaliacaopreanestesica.Paciente.DomainObject);
                return this._vmprocedimentosrealizados;
            }
        }

        public vmExameFisicoEvento vmExameFisicoEvento
        {
            get
            {
                if (this._vmexamefisicoevento.IsNull() && this._anestesiagrupo.MostraExameFisico == SimNao.Sim)
                    this._vmexamefisicoevento = new vmExameFisicoEvento(this._sumarioavaliacaopreanestesica); //, new wrpEvento(this._eventoanestesia.DomainObject)
                return _vmexamefisicoevento;
            }
        }

        [ValidaCampoObrigatorio]
        public DateTime? NPOData
        {
            get
            {
                return this._npodata;
            }
            set
            {
                this._npodata = value;
                if (this._npohora.HasValue && value.HasValue)
                    this._sumarioavaliacaopreanestesica.NPO = DateTime.Parse(value.Value.ToShortDateString() + " " + this._npohora.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaopreanestesica.NPO = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.NPOData));
            }
        }

        [ValidaCampoObrigatorio]
        public DateTime? NPOHora
        {
            get
            {
                return this._npohora;
            }
            set
            {
                this._npohora = value;
                if (this._npodata.HasValue && value.HasValue)
                    this._sumarioavaliacaopreanestesica.NPO = DateTime.Parse(this._npodata.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaopreanestesica.NPO = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.NPOHora));
            }
        }

        public bool MarcaDesmarcaTodosSemParticularidade
        {
            get
            {
                if (this.ItensCollection != null)
                    return this.ItensCollection.Count(x => x.SemParticularidade == SimNao.Sim) == this.ItensCollection.Count;
                return false;
            }
            set
            {
                if (value)
                    MarcaDesmarcaTodosNaoAvaliado = false;

                foreach (var item in this.ItensCollection)
                {
                    item.SemParticularidade = value ? SimNao.Sim : SimNao.Nao;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosSemParticularidade));
            }
        }

        public bool MarcaDesmarcaTodosNaoAvaliado
        {
            get
            {
                if (this.ItensCollection != null)
                    return this.ItensCollection.Count(x => x.NaoAvaliado == SimNao.Sim) == this.ItensCollection.Count;
                return false;
            }
            set
            {
                if (value)
                    MarcaDesmarcaTodosSemParticularidade = false;

                foreach (var item in this.ItensCollection)
                {
                    item.NaoAvaliado = value ? SimNao.Sim : SimNao.Nao;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNaoAvaliado));
            }
        }

        public bool MarcaDesmarcaTodosNaoSeAplica
        {
            get
            {
                if (this.ItensCollection != null)
                    return this.ItensCollection.Count(x => x.NaoSeAplica == SimNao.Sim) == this.ItensCollection.Count;
                return false;
            }
            set
            {
                if (value)
                    MarcaDesmarcaTodosNega = false;

                foreach (var item in this.ItensCollection)
                {
                    item.NaoSeAplica = value ? SimNao.Sim : SimNao.Nao;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNaoSeAplica));
            }
        }

        public bool MarcaDesmarcaTodosNega
        {
            get
            {
                if (this.ItensCollection != null)
                    return this.ItensCollection.Count(x => x.Nega == SimNao.Sim) == this.ItensCollection.Count;
                return false;
            }
            set
            {
                if (value)
                    MarcaDesmarcaTodosNaoSeAplica = false;

                foreach (var item in this.ItensCollection)
                {
                    item.Nega = value ? SimNao.Sim : SimNao.Nao;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNega));
            }
        }

        public bool MarcaDesmarcaTodosNaoRealizado
        {
            get
            {
                if (this.ItensCollection != null)
                    return this.ItensCollection.Count(x => x.NaoRealizado == SimNao.Sim) == this.ItensCollection.Count;
                return false;
            }
            set
            {
                if (value)
                    MarcaDesmarcaTodosResultadoIndisponivel = false;
                foreach (var item in this.ItensCollection)
                {
                    item.NaoRealizado = value ? SimNao.Sim : SimNao.Nao;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNaoRealizado));
            }
        }

        public bool MarcaDesmarcaTodosResultadoIndisponivel
        {
            get
            {
                if (this.ItensCollection != null)
                    return this.ItensCollection.Count(x => x.ResultadoIndisponivel == SimNao.Sim) == this.ItensCollection.Count;
                return false;
            }
            set
            {
                if (value)
                    MarcaDesmarcaTodosNaoRealizado = false;
                foreach (var item in this.ItensCollection)
                {
                    item.ResultadoIndisponivel = value ? SimNao.Sim : SimNao.Nao;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosResultadoIndisponivel));
            }
        }

        public int SelecionaTab { get; set; }

        public bool MostraRelatorio
        {
            get
            {
                return this._anestesiagrupo.MostraAlergias == SimNao.Nao &&
                    this._anestesiagrupo.MostraExameFisico == SimNao.Nao &&
                    this._anestesiagrupo.MostraGrid == SimNao.Nao &&
                    this._anestesiagrupo.MostraMedicamentos == SimNao.Nao &&
                    this._anestesiagrupo.MostraMedicamentosEmUso == SimNao.Nao;
            }
        }

        public IList<ProcedimentosRealizadosDTO> Print_ProcedimentosRealizados
        {
            get
            {
                return this._vmprocedimentosrealizados.ProcedimentosRealizados;
            }
        }

        #endregion

        #region ----- Métodos Privados -----
        private void _carregaitensdinamicos()
        {
            this._itenscollection = new ObservableCollection<ItemDinamico>();
            foreach (var item in this._anestesiagrupo.AnestesiaItens)
            {
                //Informacoes genericas
                var novo = new ItemDinamico(this._sumarioavaliacaopreanestesica, this);
                novo.AnestesiaItem = item;
                novo.Descricao = item.Descricao;
                novo.OrdemTela = item.OrdemTela;
                if (!item.ValoresPossiveis.IsNull())
                {
                    ObservableCollection<string> vals = new ObservableCollection<string>();
                    item.ValoresPossiveis.Split(';').Each(x => vals.Add(x));
                    novo.Valores = vals;
                }
                //
                //Informaçoes carregadas do sumario
                if (this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Count(x => x.AnestesiaItem.ID == item.ID) > 0)
                {
                    var itens = this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Where(x => x.AnestesiaItem.ID == item.ID).ToList();
                    var itemsuma = this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreAnestesicaItens.Single(x => x.AnestesiaItem.ID == item.ID);
                    novo.Selecionado = true;
                    novo.NaoAvaliado = itemsuma.NaoAvaliado;
                    novo.NaoRealizado = itemsuma.NaoRealizado;
                    novo.NaoSeAplica = itemsuma.NaoSeAplica;
                    novo.Nega = itemsuma.Nega;
                    novo.Observacao = itemsuma.Observacao;
                    novo.Resultado = itemsuma.Resultado;
                    novo.ResultadoIndisponivel = itemsuma.ResultadoIndisponivel;
                    novo.SemParticularidade = itemsuma.SemParticularidade;
                    novo.Valor = itemsuma.Valor;
                }
                //
                this._itenscollection.Add(novo);
            }

            this._itenscollection.Sort(x => x.OrdemTela);
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
        }

        private void MarcarDesmarcar(ItemDinamico pItem)
        {
            if (this.MostraCheck)
            {
                foreach (var item in this._itenscollection)
                {
                    if (item != pItem)
                    {
                        item.Selecionado = false;
                        item.Observacao = string.Empty;
                    }
                }
            }
        }

        private void RefreshMarcaDesmarcaTodos()
        {
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNaoAvaliado));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosSemParticularidade));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNega));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNaoSeAplica));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosNaoRealizado));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MarcaDesmarcaTodosResultadoIndisponivel));
        }
        #endregion

        #region ----- Métodos Públicos -----
        public override void RefreshViewModel()
        {
            _carregaitensdinamicos();
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.NPOHora));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.NPOData));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.vmPreMedicacao));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItemD));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.SumarioAvaliacaoPreAnestesica));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.ItensCollection));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.NaoSeAplica));
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmSumarioAvaliacaoPreAnestesicaItem>(x => x.MostraCheck));

            this.RefreshMarcaDesmarcaTodos();
        }
        #endregion

        #region ----- Commands -----

        #endregion

        public class ItemDinamico : wrpSumarioAvaliacaoPreAnestesicaItem
        {
            public ItemDinamico(wrpSumarioAvaliacaoPreAnestesica pVmAnestesica, vmSumarioAvaliacaoPreAnestesicaItem pVm)
                : base(pVmAnestesica)
            {
                this._vm = pVm;
            }

            private vmSumarioAvaliacaoPreAnestesicaItem _vm;
            private bool _selecionado;
            private ObservableCollection<string> _valores;

            public bool Selecionado
            {
                get
                {
                    return this._selecionado;
                }
                set
                {
                    this._selecionado = value;

                    if (value)
                        this._vm.MarcarDesmarcar(this);
                    else
                        base.Observacao = string.Empty;

                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.Selecionado));
                }
            }

            public ObservableCollection<string> Valores
            {
                get { return this._valores; }
                set { this._valores = value; }
            }

            public string Observacoes
            {
                get
                {
                    return base.Observacao;
                }
                set
                {
                    base.Observacao = value;
                    if (this._vm.MostraCheck)
                        if (!value.IsEmptyOrWhiteSpace())
                            this._selecionado = true;

                    this._vm.MarcarDesmarcar(this);
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.Selecionado));
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.Observacao));
                }
            }

            public override SimNao? SemParticularidade
            {
                get
                {
                    return base.SemParticularidade;
                }
                set
                {
                    base.SemParticularidade = value;
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.SemParticularidade));
                }
            }

            public override SimNao? NaoAvaliado
            {
                get { return base.NaoAvaliado; }
                set
                {
                    base.NaoAvaliado = value;
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.NaoAvaliado));
                }
            }

            public override SimNao? NaoSeAplica
            {
                get { return base.NaoSeAplica; }
                set
                {
                    base.NaoSeAplica = value;
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.NaoAvaliado));
                }
            }

            public override SimNao? Nega
            {
                get { return base.Nega; }
                set
                {
                    base.Nega = value;
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.Nega));
                }
            }

            public override SimNao? NaoRealizado
            {
                get { return base.NaoRealizado; }
                set
                {
                    base.NaoRealizado = value;
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.NaoRealizado));
                }
            }

            public override SimNao? ResultadoIndisponivel
            {
                get { return base.ResultadoIndisponivel; }
                set
                {
                    base.ResultadoIndisponivel = value;
                    this._vm.RefreshMarcaDesmarcaTodos();
                    this.OnPropertyChanged(ExpressionEx.PropertyName<ItemDinamico>(x => x.ResultadoIndisponivel));
                }
            }
            public string Descricao { get; set; }
            public int OrdemTela { get; set; }
        }
    }
}

