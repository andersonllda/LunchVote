using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using System.Collections.ObjectModel;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using StructureMap;
using System.Linq;
using HMV.Core.Domain.Enum.AdmissaoAssistencialCTINEO;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN
{
    public class vmSumarioAvaliacaoMedicaRNExameClinico : ViewModelBase
    {
        
        #region Enum
        public enum TabsExameClinico
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
        public vmSumarioAvaliacaoMedicaRNExameClinico(wrpSumarioAvaliacaoMedicaRN pSumarioRN, vmSumarioAvaliacaoMedicaRN pVm)
        {
            this._sumarioavaliacaomedicarn = pSumarioRN;
            this._vm = pVm;
            if (this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNExameFisico.IsNull())
                pSumarioRN.SumarioAvaliacaoMedicaRNExameFisico = new wrpSumarioAvaliacaoMedicaRNExameFisico(pSumarioRN);
                
            this._sumarioAvaliacaoMedicaRNExameFisico = pSumarioRN.SumarioAvaliacaoMedicaRNExameFisico;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private IList<RuntimeTab<TabsExameClinico>> _tabs;
        private TabsExameClinico _tipotabselecionada;
        private wrpSumarioAvaliacaoMedicaRN _sumarioavaliacaomedicarn;
        private wrpSumarioAvaliacaoMedicaRNExameFisico _sumarioAvaliacaoMedicaRNExameFisico;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectioncabecapescoco;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectionrespiratorio;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectioncardiovascular;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectionoutros;
        private ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> _collectionosteoarticular;
        //VMs       
        private vmSumarioAvaliacaoMedicaRN _vm;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoMedicaRNExameFisico SumarioAvaliacaoMedicaRNExameFisico
        {
            get
            {
                return this._sumarioAvaliacaoMedicaRNExameFisico;
            }
        }

        //public bool CorCianoseExtremidades
        //{
        //    get
        //    {
        //        if (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.HasValue)
        //            return (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.Value == CorRN.CianoseExtremidades);
        //        return false;
        //    }
        //    set
        //    {
        //        if (value)
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = CorRN.CianoseExtremidades;
        //        else
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = null;
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorRosada);
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorPalida);
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorCianoseGeneralizada);
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorCianoseExtremidades);                
        //    }
        //}
        //public bool CorCianoseGeneralizada
        //{
        //    get
        //    {
        //        if (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.HasValue)
        //            return (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.Value == CorRN.CianoseGeneralizada);
        //        return false;
        //    }
        //    set
        //    {
        //        if (value)
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = CorRN.CianoseGeneralizada;
        //        else
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = null;
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorRosada);
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorPalida);                
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorCianoseExtremidades);    
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorCianoseGeneralizada);
        //    }
        //}
        //public bool CorPalida
        //{
        //    get
        //    {
        //        if (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.HasValue)
        //            return (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.Value == CorRN.Palida);
        //        return false;
        //    }
        //    set
        //    {
        //        if (value)
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = CorRN.Palida;
        //        else
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = null;
        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorPalida);
        //    }
        //}
        //public bool CorRosada
        //{
        //    get
        //    {
        //        if (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.HasValue)
        //            return (this._sumarioAvaliacaoMedicaRNExameFisico.Cor.Value == CorRN.Rosada);
        //        return false;
        //    }
        //    set
        //    {
        //        if (value)
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = CorRN.Rosada;
        //        else
        //            this._sumarioAvaliacaoMedicaRNExameFisico.Cor = null;

        //        this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.CorRosada);
        //    }
        //}

        public bool TonusNormal
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Tonus.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Tonus.Value == TonusRN.Normal);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Tonus = TonusRN.Normal;
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Tonus = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusHipotonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusHipertonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusNormal);
            }
        }
        public bool TonusHipertonico
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Tonus.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Tonus.Value == TonusRN.Hipertonico);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Tonus = TonusRN.Hipertonico;
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Tonus = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusHipotonico);                
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusNormal);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusHipertonico);
            }
        }
        public bool TonusHipotonico
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Tonus.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Tonus.Value == TonusRN.Hipotonico);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Tonus = TonusRN.Hipotonico;
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Tonus = null;                
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusHipertonico);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusNormal);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.TonusHipotonico);
            }
        }

        public bool Ativo
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Atividade.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Atividade.Value == AtividadeRN.Ativo);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Atividade = AtividadeRN.Ativo;
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Atividade = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Reativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Hipoativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Ativo);
            }
        }
        public bool Hipoativo
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Atividade.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Atividade.Value == AtividadeRN.Hipoativo);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Atividade = AtividadeRN.Hipoativo;
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Atividade = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Reativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Hipoativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Ativo);
            }
        }
        public bool Reativo
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Atividade.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Atividade.Value == AtividadeRN.Reativo);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Atividade = AtividadeRN.Reativo;
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Atividade = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Reativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Hipoativo);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.Ativo);
            }
        }


        public bool PIntegra //Normal
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Pele.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Pele.Value == Pele.Normal);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioAvaliacaoMedicaRNExameFisico.Pele = Pele.Normal;
                    this._sumarioAvaliacaoMedicaRNExameFisico.PeleOutros = string.Empty;
                }
                else
                    this._sumarioAvaliacaoMedicaRNExameFisico.Pele = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.PIntegra);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.PLesoes);
            }
        }
        public bool PLesoes //Alteracóes
        {
            get
            {
                if (this._sumarioAvaliacaoMedicaRNExameFisico.Pele.HasValue)
                    return (this._sumarioAvaliacaoMedicaRNExameFisico.Pele.Value == Pele.Alteracoes);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioAvaliacaoMedicaRNExameFisico.Pele = Pele.Alteracoes;
                else
                {
                    this._sumarioAvaliacaoMedicaRNExameFisico.Pele = null;
                    this._sumarioAvaliacaoMedicaRNExameFisico.PeleOutros = string.Empty;
                }

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.PIntegra);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNExameClinico>(x => x.PLesoes);
            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionCabecaPescoco
        {
            get
            {
                if (this._collectioncabecapescoco.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualCabecaPescoco();
                    var lista = rep.List();
                    this._collectioncabecapescoco = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectioncabecapescoco.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                            IsNormal = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   null : null
                        });
                    });
                }
                return this._collectioncabecapescoco;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionRespiratorio
        {
            get
            {
                if (this._collectionrespiratorio.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualRespiratorio();
                    var lista = rep.List();
                    this._collectionrespiratorio = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectionrespiratorio.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                            IsNormal = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   null : null
                        });
                    });
                }
                return this._collectionrespiratorio;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionCardioVascular
        {
            get
            {
                if (this._collectioncardiovascular.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualCardio();
                    var lista = rep.List();
                    this._collectioncardiovascular = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectioncardiovascular.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                            IsNormal = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   null : null
                        });
                    });
                }
                return this._collectioncardiovascular;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionOutros
        {
            get
            {
                if (this._collectionoutros.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualOutros();
                    var lista = rep.List();
                    this._collectionoutros = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectionoutros.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                            IsNormal = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   null : null
                        });
                    });
                }
                return this._collectionoutros;
            }
        }
        public ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item> CollectionOsteoArticular
        {
            get
            {
                if (this._collectionosteoarticular.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    rep.OndeItemIgualOsteoArticular();
                    var lista = rep.List();
                    this._collectionosteoarticular = new ObservableCollection<vmSumarioAvaliacaoMedicaRN.Item>();
                    lista.Each(x =>
                    {
                        this._collectionosteoarticular.Add(new vmSumarioAvaliacaoMedicaRN.Item
                        {
                            ItemCO = x,
                            Observacao = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Observacoes :
                                   string.Empty : string.Empty,
                            Selecionado = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   true : false : false,
                            IsNormal = this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.HasItems() ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicarn.SumarioAvaliacaoMedicaRNItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsNormal :
                                   null : null
                        });
                    });
                }
                return this._collectionosteoarticular;
            }
        }
        public IList<RuntimeTab<TabsExameClinico>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
        }
        public TabsExameClinico? TipoTabSelecionada
        {
            get
            {
                return this._tipotabselecionada;
            }
            set
            {
                if (value.HasValue)
                    this._tipotabselecionada = value.Value;

                this.OnPropertyChanged<vmSumarioObstetrico>(x => x.TipoTabSelecionada);
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsExameClinico>>();
           
            //Parte1
            this._tabs.Add(new RuntimeTab<TabsExameClinico>
            {
                TipoTab = TabsExameClinico.Parte1,
                Descricao = TabsExameClinico.Parte1.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucP1.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    //Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaRN>(x => x.vmAnamneseSumarioAvaliacaoMedicaCO)),
                    Source = this
                }
            });

            //Parte2
            this._tabs.Add(new RuntimeTab<TabsExameClinico>
            {
                TipoTab = TabsExameClinico.Parte2,
                Descricao = TabsExameClinico.Parte2.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucP2.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    //Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaRN>(x => x.vmExamesRealizadosSumarioAvaliacaoMedicaCO)),
                    Source = this
                }
            });

            //Parte3
            this._tabs.Add(new RuntimeTab<TabsExameClinico>
            {
                TipoTab = TabsExameClinico.Parte3,
                Descricao = TabsExameClinico.Parte3.GetEnumDescription(),
                Componente = new Uri(@"UserControls\SumarioAvaliacaoMedicaRN\ucExameClinicoObservacoes.xaml", UriKind.Relative),
                Binding = new Binding
                {
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    //Path = new PropertyPath(ExpressionEx.PropertyName<vmSumarioAvaliacaoMedicaRN>(x => x.vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO)),
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
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
