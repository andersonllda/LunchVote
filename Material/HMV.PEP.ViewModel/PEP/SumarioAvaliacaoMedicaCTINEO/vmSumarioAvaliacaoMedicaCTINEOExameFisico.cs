using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using StructureMap;
using HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO
{
    public class vmSumarioAvaliacaoMedicaCTINEOExameFisico : ViewModelBase
    {                 
        #region Enum
        public enum TabsExameFisico
        {
            [Description("Parte 1")]
            Parte1,
            [Description("Parte 2")]
            Parte2,
            [Description("Parte 3")]
            Parte3
        }
        #endregion

        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaCTINEOExameFisico(wrpSumarioAvaliacaoMedicaCTINEO pSumarioCTINEO, vmSumarioAvaliacaoMedicaCTINEO pVm)
        {
            this._sumarioavaliacaomedicactineo = pSumarioCTINEO;
            this._vm = pVm;
            if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOExameFisico.IsNull())
                pSumarioCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico = new wrpSumarioAvaliacaoMedicaCTINEOExameFisico(pSumarioCTINEO);

            this._sumarioAvaliacaoMedicaCTINEOExameFisico = pSumarioCTINEO.SumarioAvaliacaoMedicaCTINEOExameFisico;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsExameFisico>> _tabs;
        private TabsExameFisico _tipotabselecionada;
        private wrpSumarioAvaliacaoMedicaCTINEO _sumarioavaliacaomedicactineo;
        private wrpSumarioAvaliacaoMedicaCTINEOExameFisico _sumarioAvaliacaoMedicaCTINEOExameFisico;
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> _collectioncabecapescoco;
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> _collectionrespiratorio;
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> _collectioncardiovascular;
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> _collectionoutros;
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> _collectionosteoarticular;

        //VMs       
        private vmSumarioAvaliacaoMedicaCTINEO _vm;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoMedicaCTINEOExameFisico SumarioAvaliacaoMedicaCTINEOExameFisico
        {
            get
            {
                return this._sumarioAvaliacaoMedicaCTINEOExameFisico;
            }
        }

        public bool TonusNormal
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.Value == TonusRN.Normal);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = TonusRN.Normal;
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipotonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipertonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusNormal);
            }
        }
        public bool TonusHipertonico
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.Value == TonusRN.Hipertonico);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = TonusRN.Hipertonico;
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipotonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusNormal);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipertonico);
            }
        }
        public bool TonusHipotonico
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus.Value == TonusRN.Hipotonico);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = TonusRN.Hipotonico;
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Tonus = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipertonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusNormal);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipotonico);
            }
        }

        public bool Ativo
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.Value == AtividadeRN.Ativo);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = AtividadeRN.Ativo;
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Reativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Hipoativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Ativo);
            }
        }
        public bool Hipoativo
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.Value == AtividadeRN.Hipoativo);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = AtividadeRN.Hipoativo;
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Reativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Hipoativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Ativo);
            }
        }
        public bool Reativo
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade.Value == AtividadeRN.Reativo);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = AtividadeRN.Reativo;
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Atividade = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Reativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Hipoativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Ativo);
            }
        }

        public bool PIntegra //NORMAL
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele.Value == Pele.Normal);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele = Pele.Normal;
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.PeleOutros = string.Empty;
                }
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.PIntegra);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.PLesoes);
            }
        }
        public bool PLesoes //ALTERACOES
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele.Value == Pele.Alteracoes);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele = Pele.Alteracoes;
                else
                {
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.Pele = null;
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.PeleOutros = string.Empty;
                }

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.PIntegra);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.PLesoes);
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> CollectionCabecaPescoco
        {
            get
            {
                if (this._collectioncabecapescoco.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualCabecaPescoco();
                    var lista = rep.List();
                    this._collectioncabecapescoco = new ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item>();
                    lista.Each(x =>
                    {
                        this._collectioncabecapescoco.Add(new vmSumarioAvaliacaoMedicaCTINEO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            IsNormal = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   new Nullable<SimNao>() : new Nullable<SimNao>()
                        });
                    });
                }
                return this._collectioncabecapescoco;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> CollectionRespiratorio
        {
            get
            {
                if (this._collectionrespiratorio.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualRespiratorio();
                    var lista = rep.List();
                    this._collectionrespiratorio = new ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item>();
                    lista.Each(x =>
                    {
                        this._collectionrespiratorio.Add(new vmSumarioAvaliacaoMedicaCTINEO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            IsNormal = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   new Nullable<SimNao>() : new Nullable<SimNao>()
                        });
                    });
                }
                return this._collectionrespiratorio;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> CollectionCardioVascular
        {
            get
            {
                if (this._collectioncardiovascular.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualCardio();
                    var lista = rep.List();
                    this._collectioncardiovascular = new ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item>();
                    lista.Each(x =>
                    {
                        this._collectioncardiovascular.Add(new vmSumarioAvaliacaoMedicaCTINEO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            IsNormal = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   new Nullable<SimNao>() : new Nullable<SimNao>()
                        });
                    });
                }
                return this._collectioncardiovascular;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> CollectionOutros
        {
            get
            {
                if (this._collectionoutros.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualOutros();
                    var lista = rep.List();
                    this._collectionoutros = new ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item>();
                    lista.Each(x =>
                    {
                        this._collectionoutros.Add(new vmSumarioAvaliacaoMedicaCTINEO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            IsNormal = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   new Nullable<SimNao>() : new Nullable<SimNao>()
                        });
                    });
                }
                return this._collectionoutros;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item> CollectionOsteoArticular
        {
            get
            {
                if (this._collectionosteoarticular.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualOsteoArticular();
                    var lista = rep.List();
                    this._collectionosteoarticular = new ObservableCollection<vmSumarioAvaliacaoMedicaCTINEO.Item>();
                    lista.Each(x =>
                    {
                        this._collectionosteoarticular.Add(new vmSumarioAvaliacaoMedicaCTINEO.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty,
                            IsNormal = this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.HasItems() ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   new Nullable<SimNao>() : new Nullable<SimNao>()
                        });
                    });
                }
                return this._collectionosteoarticular;
            }
        }

        public bool CArteriaVeia
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical = SimNao.Sim;
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.UmbilicalAlteracoes = string.Empty;
                }
                else
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CArteriaVeia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CAlteracoes);
            }
        }
        public bool CAlteracoes
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical.HasValue)
                    return (this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical = SimNao.Nao;
                else
                {
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.IsUmbilical = null;
                    this._sumarioAvaliacaoMedicaCTINEOExameFisico.UmbilicalAlteracoes = string.Empty;
                }

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CArteriaVeia);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CAlteracoes);
            }
        }

        public IList<RuntimeTab<TabsExameFisico>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
        }
        public TabsExameFisico? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TipoTabSelecionada);
            }
        }
        #endregion

        #region ----- Métodos Privados -----
        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsExameFisico>>();
           
            //Parte1
            this._tabs.Add(new RuntimeTab<TabsExameFisico>
            {
                TipoTab = TabsExameFisico.Parte1,
                Descricao = TabsExameFisico.Parte1.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucP1.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,                    
                    Source = this
                }
            });

            //Parte2
            this._tabs.Add(new RuntimeTab<TabsExameFisico>
            {
                TipoTab = TabsExameFisico.Parte2,
                Descricao = TabsExameFisico.Parte2.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucP2.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,                    
                    Source = this
                }
            });

            //Parte3
            this._tabs.Add(new RuntimeTab<TabsExameFisico>
            {
                TipoTab = TabsExameFisico.Parte3,
                Descricao = TabsExameFisico.Parte3.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaCTINEO\ucP3.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,                    
                    Source = this
                }
            });          
        }       
        #endregion

        #region ----- Métodos Públicos -----
        public void ChamaSalvar()
        {
            this._vm.Commands.ExecuteCommand(enumCommand.CommandSalvar, null);
        }

        public override void RefreshViewModel()
        {
            this.SumarioAvaliacaoMedicaCTINEOExameFisico.RefreshViewModel();
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipotonico);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusHipertonico);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.TonusNormal);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Reativo);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Hipoativo);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.Ativo);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CollectionCabecaPescoco);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CollectionRespiratorio);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CollectionCardioVascular);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CollectionOutros);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaCTINEOExameFisico>(x => x.CollectionOsteoArticular);
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
