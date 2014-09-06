using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.CentroObstetrico;
using HMV.Core.Domain.Enum.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Types;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmExamesRealizadosSumarioAvaliacaoMedicaCO : ViewModelBase
    {
        #region ----- Construtor -----
        public vmExamesRealizadosSumarioAvaliacaoMedicaCO(wrpSumarioAvaliacaoMedicaCO pSumarioAvaliacaoMedicaCO)
        {
            this._sumarioavaliacaocoselecionado = pSumarioAvaliacaoMedicaCO;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaCO _sumarioavaliacaocoselecionado;
        private ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> _collectionExames;        
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoMedicaCO SumarioAvaliacaoMedicaCO
        {
            get
            {
                return this._sumarioavaliacaocoselecionado;
            }
        }
        public List<string> CarregaTipagem
        {
            get
            {
                return Enum<Tipagem>.GetCustomDisplay().ToList();
            }
        }

        public List<string> CarregaPositivo
        {
            get
            {
                return Enum<FatorRH>.GetCustomDisplay().ToList();
            }
        }

        public FatorRH? RHFator
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.RHPaciente;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.RHPaciente = value;
                if (value.HasValue && value.Value == FatorRH.Positivo)
                    this._sumarioavaliacaocoselecionado.Coombs = null;
                base.OnPropertyChanged("CoPositivo");
                base.OnPropertyChanged("CoNegativo");
                base.OnPropertyChanged<vmExamesRealizadosSumarioAvaliacaoMedicaCO>(x => x.HabilitaCoombs);
                base.OnPropertyChanged<vmExamesRealizadosSumarioAvaliacaoMedicaCO>(x => x.RHFator);
            }
        }

        public bool CoPositivo
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.Coombs == FatorRH.Positivo;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.Coombs = null;
                if (value)
                    this._sumarioavaliacaocoselecionado.Coombs = FatorRH.Positivo;
                base.OnPropertyChanged("CoPositivo");
                base.OnPropertyChanged("CoNegativo");
            }
        }

        public bool CoNegativo
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.Coombs == FatorRH.Negativo;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.Coombs = null;
                if (value)
                    this._sumarioavaliacaocoselecionado.Coombs = FatorRH.Negativo;
                base.OnPropertyChanged("CoNegativo");
                base.OnPropertyChanged("CoPositivo");

            }
        }

        public bool HivNaoRealizado
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.HIV == HIV.NaoRealizado;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.HIV = null;
                if (value)
                    this._sumarioavaliacaocoselecionado.HIV = HIV.NaoRealizado;
                base.OnPropertyChanged("HivNaoRealizado");
                base.OnPropertyChanged("HivPositivo");
                base.OnPropertyChanged("HivNegativo");
            }
        }

        public bool HivPositivo
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.HIV == HIV.Posivo;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.HIV = null;
                if (value)
                    this._sumarioavaliacaocoselecionado.HIV = HIV.Posivo;

                base.OnPropertyChanged("HivPositivo");
                base.OnPropertyChanged("HivNegativo");
                base.OnPropertyChanged("HivNaoRealizado");


            }
        }

        public bool HivNegativo
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.HIV == HIV.Negativo;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.HIV = null;
                if (value)
                    this._sumarioavaliacaocoselecionado.HIV = HIV.Negativo;


                base.OnPropertyChanged("HivNegativo");
                base.OnPropertyChanged("HivPositivo");
                base.OnPropertyChanged("HivNaoRealizado");

            }
        }

        public ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item> CollectionResultados
        {
            get
            {
                if (this._collectionExames.IsNull())
                {
                    IRepositorioDeItensCO rep = ObjectFactory.GetInstance<IRepositorioDeItensCO>();
                    rep.FiltraAtivos();
                    var lista = rep.List().Where(x => x.Sorologia == SimNao.Sim); // x.Exames == SimNao.Sim || 
                    this._collectionExames = new ObservableCollection<vmSumarioAvaliacaoMedicaCO.Item>();
                    lista.Each(x =>
                    {
                        this._collectionExames.Add(new vmSumarioAvaliacaoMedicaCO.Item
                        {
                            ItemCO = x,
                            Resultado = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                            this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                            this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Resultado
                            : new Nullable<ResultadoItemCO>()
                            : new Nullable<ResultadoItemCO>(),

                            ResultadoPositivo = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                                this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                                this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Resultado == ResultadoItemCO.Positivo ? true 
                                                : false : false : false,

                            ResultadoNegativo = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                                this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                                this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Resultado == ResultadoItemCO.Negativo ? true 
                                                : false : false : false,

                            ResultadoIndisponivel = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                                    this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                                    this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().Resultado == ResultadoItemCO.NaoDisponivel ? true 
                                                    : false : false : false,

                            IsTriPrimeiro = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                            this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                            this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsTrimestre1 == SimNao.Sim ? true 
                                            : false : false : false,

                            IsTriSegundo = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                           this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                           this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsTrimestre2 == SimNao.Sim ? true 
                                           : false : false : false,

                            IsTriTerceiro = this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.HasItems() ?
                                            this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).Count() > 0 ?
                                            this._sumarioavaliacaocoselecionado.SumarioAvaliacaoMedicaCOItens.Where(y => y.ItemCO.ID == x.ID).FirstOrDefault().IsTrimestre3 == SimNao.Sim ? true 
                                            : false : false :false,
                        });
                    });
                }


                return this._collectionExames;
            }
        }

        public string ExamesRealizadosObservacao
        {
            get
            {
                return this._sumarioavaliacaocoselecionado.ExamesRealizadosObservacao;
            }
            set
            {
                this._sumarioavaliacaocoselecionado.ExamesRealizadosObservacao = value;

            }
        }

        public bool HabilitaCoombs
        {
            get
            {
                return (this._sumarioavaliacaocoselecionado.RHPaciente.HasValue && this._sumarioavaliacaocoselecionado.RHPaciente.Value == FatorRH.Negativo);
            }
        }

        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----

        #endregion
    }

}
