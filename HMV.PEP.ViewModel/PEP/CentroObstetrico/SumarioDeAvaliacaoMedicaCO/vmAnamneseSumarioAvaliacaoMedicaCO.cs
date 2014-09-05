using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.AdmAssistencial;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Validations;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.ProcessosEnfermagem.ViewModel;
using StructureMap;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmAnamneseSumarioAvaliacaoMedicaCO : ViewModelBase
    {
        #region Enum
        public enum TabsAnamneseSumarioAvaliacaoMedicaCO
        {
            [Description("Motivo Internação / História")]
            MotivoInternacao,            
            [Description("Gestação Atual")]
            GestacaoAtual,
            [Description("Alergias")]
            Alergias,
            [Description("Gestações Anteriores")]
            GestacoesAnteriores,
            [Description("História Pregressa")]
            HistoriaPregressa,
            [Description("Perfil Psico-Social")]
            PerfilPsicoSocial,
            [Description("Medicamentos Habituais")]
            MedicamentosemUso
        }
        #endregion

        #region ----- Construtor -----
        public vmAnamneseSumarioAvaliacaoMedicaCO(wrpSumarioAvaliacaoMedicaCO pSumarioAvaliacaoMedicaCO, wrpPaciente pPaciente, wrpUsuarios pUsuarios, bool pIsCorpoClinico, vmSumarioAvaliacaoMedicaCO pVm)
        {
            this._sumarioavaliacaocoselecionado = pSumarioAvaliacaoMedicaCO;

            this._paciente = pPaciente;
            this._usuario = pUsuarios;
            this._iscorpoclinico = pIsCorpoClinico;
            //this._evento = pEvento;
            if (this._sumarioavaliacaocoselecionado.Procedencia.IsNotNull())
                this._procedencia = ProcedenciaCollection.Where(x=> x.ID == this._sumarioavaliacaocoselecionado.Procedencia.ID).SingleOrDefault();

            this._sumarioCO = pVm;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>> _tabs;
        private TabsAnamneseSumarioAvaliacaoMedicaCO _tipotabselecionada;
        private vmAlergiasEvento _vmalergiasevento;
        private vmMedicamentosEmUsoEvento _vmmedicamentosemusoevento;
        private wrpSumarioAvaliacaoMedicaCO _sumarioavaliacaocoselecionado;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        private bool _iscorpoclinico;
        //private wrpEventoSumarioAvaliacaoMedicaCO _evento;

        private ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> _collectiongestacaoanterior;
        private ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> _collectionhistoriapregressa;
        private ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> _collectionperfilpsicosocial;
        private ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> _collectiongest;
        private wrpProcedenciaCollection _procedenciacollection;
        private wrpProcedencia _procedencia;
        //private int _idadesemanas;
        //private int _idadedias;

        private vmSumarioAvaliacaoMedicaCO _sumarioCO;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpProcedenciaCollection ProcedenciaCollection
        {
            get
            {
                if (this._procedenciacollection.IsNull())
                {
                    IRepositorioDeProcedencia rep = ObjectFactory.GetInstance<IRepositorioDeProcedencia>();
                    this._procedenciacollection = new wrpProcedenciaCollection(rep.List().Where(x=> x.CO == SimNao.Sim).ToList());
                }
                return this._procedenciacollection;
            }
        }

        public wrpProcedencia Procedencia
        {
            get { return this._procedencia; }
            set
            {
                this._procedencia = value;
                if (this._sumarioavaliacaocoselecionado.Procedencia != value)
                    this._sumarioavaliacaocoselecionado.Procedencia = this._procedencia;
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.Procedencia);
            }
        }

        public IList<RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
        }

        public TabsAnamneseSumarioAvaliacaoMedicaCO? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.TipoTabSelecionada);
            }
        }

        public wrpSumarioAvaliacaoMedicaCO SumarioAvaliacaoMedicaCO
        {
            get
            {
                return this._sumarioavaliacaocoselecionado;
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
                    repa.OndeChaveIgual(this._sumarioavaliacaocoselecionado.Id);
                    repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedicaCO);
                    var reta = repa.List();
                    if (reta.IsNotNull())
                        _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

                    this._vmalergiasevento = new vmAlergiasEvento(false, this._paciente, this._usuario, true, TipoEvento.SumarioAvaliacaoMedicaCO, _AlergiaCollection
                        , this._sumarioavaliacaocoselecionado.Id, this._sumarioavaliacaocoselecionado.Atendimento);

                    if (this._sumarioCO.Novo)
                        this._vmalergiasevento.MarcarTodasAlergias();
                }
                return this._vmalergiasevento;
            }
        }

        public vmMedicamentosEmUsoEvento vmMedicamentosEmUsoEvento
        {
            get
            {
                if (this._vmmedicamentosemusoevento.IsNull())
                {
                    wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;

                    IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                    repp.OndeChaveIgual(this._sumarioavaliacaocoselecionado.Id);
                    repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedicaCO);
                    var ret = repp.List();
                    if (ret.IsNotNull())
                        _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);                   

                    this._vmmedicamentosemusoevento = new vmMedicamentosEmUsoEvento(false, this._paciente, this._usuario, TipoEvento.SumarioAvaliacaoMedicaCO, _MedicamentosCollection
                        , this._sumarioavaliacaocoselecionado.Id, pMostraUltAdministracao: true,
                        pMostraDataInicio: false, pMostraVia: true, pMostraFrequencia: true, 
                        pMostraComboVia: false, pMostraComboFrequencia: false, pAtendimento: this._sumarioavaliacaocoselecionado.Atendimento);
                    
                    if (this._sumarioCO.Novo)
                        this._vmmedicamentosemusoevento.MedicamentosCollection.Each(x => { x.Selecionado = true; });
                }
                return this._vmmedicamentosemusoevento;
            }
        }

        [ValidaMaximoEMinimo(4, 42)]
        public int? IdadeSemana
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.IdadeSemana;
            }
            set
            {
                if (value != null)
                    this.IdadeDesconhecida = false;
                this._sumarioavaliacaocoselecionado.IdadeSemana = value;
            }
        }

        [ValidaMaximoEMinimo(0, 6)]
        public int? IdadeDias
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.IdadeDias;
            }
            set
            {
                if (value != null)
                    this.IdadeDesconhecida = false;

                this._sumarioavaliacaocoselecionado.IdadeDias = value;
            }
        }      

        public bool IdadeDesconhecida
        {
            get
            {
                if (this._sumarioavaliacaocoselecionado.IdadeDesconhecida == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaocoselecionado.IdadeDesconhecida = SimNao.Sim;
                    this.IdadeSemana = null;
                    this.IdadeDias = null;   
                }
                else
                {
                    this._sumarioavaliacaocoselecionado.IdadeDesconhecida = SimNao.Nao;
                }
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IdadeDesconhecida);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IdadeSemana);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IdadeDias);                
            }
        }

        public bool IsGestacaoAnterior
        {
            get
            {
                if (this._sumarioavaliacaocoselecionado.IsGestacaoAnterior == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {                
                if (value)
                {
                    if (this._collectiongestacaoanterior.HasItems())
                    {
                        if (this._collectiongestacaoanterior.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                        else
                        {
                            this._sumarioavaliacaocoselecionado.IsGestacaoAnterior = SimNao.Sim;
                            this._sumarioavaliacaocoselecionado.GestacaoAnteriorObservacao = string.Empty;
                        }
                    }
                    else
                    {
                        this._sumarioavaliacaocoselecionado.IsGestacaoAnterior = SimNao.Sim;
                        this._sumarioavaliacaocoselecionado.GestacaoAnteriorObservacao = string.Empty;
                    }
                }
                else
                    this._sumarioavaliacaocoselecionado.IsGestacaoAnterior = SimNao.Nao;

                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.CollectionGestacaoAnterior);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IsGestacaoAnterior);
            }
        }

        public bool IsPrimeiraGestacao
        {
            get
            {
                if (this._sumarioavaliacaocoselecionado.IsPrimeiraGestacao == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (this._collectiongestacaoanterior.HasItems())
                    {
                        if (this._collectiongestacaoanterior.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                        else
                        {
                            this._sumarioavaliacaocoselecionado.IsPrimeiraGestacao = SimNao.Sim;
                            this._sumarioavaliacaocoselecionado.GestacaoAnteriorObservacao = string.Empty;
                            this._sumarioavaliacaocoselecionado.IsGestacaoAnterior = SimNao.Nao;
                        }
                    }
                    else
                    {
                        this._sumarioavaliacaocoselecionado.IsPrimeiraGestacao = SimNao.Sim;
                        this._sumarioavaliacaocoselecionado.GestacaoAnteriorObservacao = string.Empty;
                        this._sumarioavaliacaocoselecionado.IsGestacaoAnterior = SimNao.Nao;
                    }
                }
                else
                    this._sumarioavaliacaocoselecionado.IsPrimeiraGestacao = SimNao.Nao;

                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IsGestacaoAnterior);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.CollectionGestacaoAnterior);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IsPrimeiraGestacao);
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> CollectionGestacaoAnterior
        {
            get
            {
                if (this._collectiongestacaoanterior.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualGestacaoAnterior();
                    var lista = rep.List();
                    this._collectiongestacaoanterior = new ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item>();
                    lista.Each(x =>
                    {
                        this._collectiongestacaoanterior.Add(new vmSumarioAvaliacaoMedicaCO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                        });
                    });
                }
                return this._collectiongestacaoanterior;
            }
        }

        public bool NegaPatologiaPrevia
        {
            get
            {
                if (this._sumarioavaliacaocoselecionado.NegaPatologiaPrevia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (this._collectionhistoriapregressa.HasItems())
                    {
                        if (this._collectionhistoriapregressa.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                        else
                        {
                            this._sumarioavaliacaocoselecionado.NegaPatologiaPrevia = SimNao.Sim;
                            this._sumarioavaliacaocoselecionado.PatologiaPreviaObservacao = string.Empty;    
                        }
                    }
                    else
                    {
                        this._sumarioavaliacaocoselecionado.NegaPatologiaPrevia = SimNao.Sim;
                        this._sumarioavaliacaocoselecionado.PatologiaPreviaObservacao = string.Empty;    
                    }                                    
                }
                else
                    this._sumarioavaliacaocoselecionado.NegaPatologiaPrevia = SimNao.Nao;

                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.CollectionHistoriaPregressa);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.NegaPatologiaPrevia);
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> CollectionHistoriaPregressa
        {
            get
            {
                if (this._collectionhistoriapregressa.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualDoencasPrevias();
                    var lista = rep.List();
                    this._collectionhistoriapregressa = new ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item>();
                    lista.Each(x =>
                    {
                        this._collectionhistoriapregressa.Add(new vmSumarioAvaliacaoMedicaCO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ? 
                                   true : false : false
                        });
                    });
                }
                return this._collectionhistoriapregressa;
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> CollectionPerfilPsicoSocial
        {
            get
            {
                if (this._collectionperfilpsicosocial.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualHabitos();
                    var lista = rep.List();
                    this._collectionperfilpsicosocial = new ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item>();
                    lista.Each(x =>
                    {
                        this._collectionperfilpsicosocial.Add(new vmSumarioAvaliacaoMedicaCO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true: false : false
                        });
                    });
                }
                return this._collectionperfilpsicosocial;
            }
        }

        public bool IsPatologia
        {
            get
            {
                if (this._sumarioavaliacaocoselecionado.IsPatologia == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                {
                    if (this._collectiongest.HasItems())
                    {
                        if (this._collectiongest.Count(x => x.Selecionado == true) > 0)
                        {
                            DXMessageBox.Show("Deve desmarcar os itens selecionados.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                        }
                        else
                        {
                            this._sumarioavaliacaocoselecionado.IsPatologia = SimNao.Sim;
                        }
                    }
                    else
                    {
                        this._sumarioavaliacaocoselecionado.IsPatologia = SimNao.Sim;
                    }                                  
                }
                else
                    this._sumarioavaliacaocoselecionado.IsPatologia = SimNao.Nao;

                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.CollectionGest);
                this.OnPropertyChanged<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.IsPatologia);
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> CollectionGest
        {
            get
            {
                if (this._collectiongest.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualPatologia();                                        
                    IList<ItensCO> lista = rep.List();
                    this._collectiongest = new ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item>();

                    lista.Each(x =>
                    {
                        this._collectiongest.Add(new vmSumarioAvaliacaoMedicaCO.Item
                        {                            
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,

                            Selecionado = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                   this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false

                        });
                    });
                }
                return this._collectiongest;               
            }
        }
        #endregion

        #region ----- Métodos Privados -----
        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>>();

            //MotivoInternacao
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.MotivoInternacao,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.MotivoInternacao.GetEnumDescription(),
                Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucMotivoInternacao.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //HistorialAtual
            //this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            //{
            //    TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.HistorialAtual,
            //    Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.HistorialAtual.GetEnumDescription(),
            //    Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucHistorialAtual.xaml", UriKind.Relative),
            //    Binding = new Binding
            //    {
            //        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            //        Source = this
            //    }
            //});

            //GestacaoAtual
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.GestacaoAtual.GetEnumDescription(),
                Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucGestacaoAtual.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //Alergias
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.Alergias,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.Alergias.GetEnumDescription(),
                Componente = new Uri(@"/HMV.ProcessosEnfermagem.WPF;component/Views/Alergias/ucAlergiasEvento.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Path = new PropertyPath(ExpressionEx.PropertyName<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.vmAlergiasEvento)),
                    Source = this
                }
            });

            //GestacoesAnteriores
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.GestacoesAnteriores,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.GestacoesAnteriores.GetEnumDescription(),
                Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucGestacoesAnteriores.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //HistoriaPregressa
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.HistoriaPregressa,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.HistoriaPregressa.GetEnumDescription(),
                Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucHistoriaPregressa.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //PerfilPsicoSocial
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.PerfilPsicoSocial,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.PerfilPsicoSocial.GetEnumDescription(),
                Componente = new Uri(@"UserControls\CentroObstetrico\SumarioDeAvaliacaoMedicaCO\ucPerfilPsicoSocial.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                }
            });

            //MedicamentosemUso
            this._tabs.Add(new RuntimeTab<TabsAnamneseSumarioAvaliacaoMedicaCO>
            {
                TipoTab = TabsAnamneseSumarioAvaliacaoMedicaCO.MedicamentosemUso,
                Descricao = TabsAnamneseSumarioAvaliacaoMedicaCO.MedicamentosemUso.GetEnumDescription(),
                Componente = new Uri(@"/HMV.ProcessosEnfermagem.WPF;component/Views/Medicamentos/ucMedicamentosEmUsoEvento.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Path = new PropertyPath(ExpressionEx.PropertyName<vmAnamneseSumarioAvaliacaoMedicaCO>(x => x.vmMedicamentosEmUsoEvento)),
                    Source = this
                }
            });
        }
        #endregion

        #region ----- Métodos Públicos -----
        public void ChamaSalvar()
        {
            _sumarioCO.Commands.ExecuteCommand(enumCommand.CommandSalvar, null);
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
