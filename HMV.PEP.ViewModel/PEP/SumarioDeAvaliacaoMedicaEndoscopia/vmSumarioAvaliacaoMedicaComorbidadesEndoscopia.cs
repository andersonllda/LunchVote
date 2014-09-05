using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmSumarioAvaliacaoMedicaComorbidadesEndoscopia : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaComorbidadesEndoscopia(wrpSumarioAvaliacaoMedicaEndoscopia pSumarioAvaliacaoMedicaEndoscopia)
        {
            this._sumarioavaliacaoendoscopiaselecionado = pSumarioAvaliacaoMedicaEndoscopia;

            if (this._sumarioavaliacaoendoscopiaselecionado.ProcedimentosEndoscopia.IsNull())
                this._sumarioavaliacaoendoscopiaselecionado.ProcedimentosEndoscopia = new wrpSumarioAvaliacaoMedicaProcedimentosEndoscopiaCollection(new List<SumarioAvaliacaoMedicaProcedimentosEndoscopia>());

            //Instancio uma NOVA Wrapper para poder controlar os procedimentos realizados que vão ser gravados na MESMA tabela dos procedimentos informados pelo usuário.
            //Por este motivo tem um controle na hora de adicionar na collection e remover, e quando salva a admissao tenho que rolar esta collection e adicionar na admissao.
            this._collectionprocedimentoendoscopia = new wrpSumarioAvaliacaoMedicaProcedimentosEndoscopiaCollection(this._sumarioavaliacaoendoscopiaselecionado.DomainObject.ProcedimentosEndoscopia.Where(x => x.IdCirurgia == null).ToList());

            if (this._collectionprocedimentoendoscopia.HasItems())
            {
                this._procedimentoendoscopiaselecionado = this._collectionprocedimentoendoscopia.FirstOrDefault();
                this._datano = this._procedimentoendoscopiaselecionado.Data.HasValue;
            }
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaEndoscopia _sumarioavaliacaoendoscopiaselecionado;
        private ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia> _collectionitemendoscopia;
        private wrpSumarioAvaliacaoMedicaProcedimentosEndoscopiaCollection _collectionprocedimentoendoscopia;
        private wrpSumarioAvaliacaoMedicaProcedimentosEndoscopia _procedimentoendoscopiaselecionado;
        private vmProcedimentosRealizados _vmprocedimentosrealizados;
        private bool _novo;
        private bool _datano;
        #endregion

        #region ----- Propriedades Públicas -----
        public ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia> CollectionItemEndoscopia
        {
            get
            {
                if (this._collectionitemendoscopia.IsNull())
                {
                    IRepositorioDeItemEndoscopia rep = ObjectFactory.GetInstance<IRepositorioDeItemEndoscopia>();
                    rep.FiltraAtivos();
                    rep.FiltraComorbidade();
                    var lista = rep.List();
                    this._collectionitemendoscopia = new ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia>();
                    lista.Each(x =>
                    {
                        this._collectionitemendoscopia.Add(new vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia
                        {
                            ItemEndoscopia = x,
                            Nega = this._sumarioavaliacaoendoscopiaselecionado.ItemEndoscopia.HasItems() ?
                                   this._sumarioavaliacaoendoscopiaselecionado.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaoendoscopiaselecionado.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).FirstOrDefault().Nega :
                                   SimNao.Nao : SimNao.Nao,
                            Observacao = this._sumarioavaliacaoendoscopiaselecionado.ItemEndoscopia.HasItems() ?
                                   this._sumarioavaliacaoendoscopiaselecionado.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaoendoscopiaselecionado.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty
                        });
                    });
                }
                return this._collectionitemendoscopia;
            }
        }

        public string Outras
        {
            get
            {
                return this._sumarioavaliacaoendoscopiaselecionado.OutrasComorbidades;
            }
            set
            {
                this._sumarioavaliacaoendoscopiaselecionado.OutrasComorbidades = value;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaComorbidadesEndoscopia>(x => x.Outras);
            }
        }

        public vmProcedimentosRealizados vmProcedimentosRealizados
        {
            get
            {
                if (this._vmprocedimentosrealizados.IsNull())
                    this._vmprocedimentosrealizados = new vmProcedimentosRealizados(this._sumarioavaliacaoendoscopiaselecionado.Paciente.DomainObject, false);
                return this._vmprocedimentosrealizados;
            }
        }

        public wrpSumarioAvaliacaoMedicaProcedimentosEndoscopiaCollection CollectionProcedimentosEndoscopia
        {
            get
            {
                return this._collectionprocedimentoendoscopia;
            }
        }

        public wrpSumarioAvaliacaoMedicaProcedimentosEndoscopia ProcedimentoEndoscopiaSelecionado
        {
            get
            {
                return this._procedimentoendoscopiaselecionado;
            }
            set
            {
                if (value.IsNotNull())
                {
                    this._procedimentoendoscopiaselecionado = value;
                    this._datano = this._procedimentoendoscopiaselecionado.Data.HasValue;
                }
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaComorbidadesEndoscopia>(x => x.ProcedimentoEndoscopiaSelecionado);
            }
        }

        public bool Data
        {
            get
            {
                return this._datano;
            }
            set
            {
                this._datano = value;
                if (value)
                    this._procedimentoendoscopiaselecionado.Ano = null;
                else
                    this._procedimentoendoscopiaselecionado.Data = null;
            }
        }

        public bool Ano
        {
            get
            {
                return !this._datano;
            }
            set
            {
                this._datano = !value;
                if (!value)
                    this._procedimentoendoscopiaselecionado.Data = null;
                else
                    this._procedimentoendoscopiaselecionado.Ano = null;
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----
        protected override void CommandIncluir(object param)
        {
            this._novo = true;
            this._procedimentoendoscopiaselecionado = new wrpSumarioAvaliacaoMedicaProcedimentosEndoscopia(this._sumarioavaliacaoendoscopiaselecionado);
            this._procedimentoendoscopiaselecionado.Data = DateTime.Today;
            this._datano = true;
            base.CommandIncluir(param);
        }
        protected override bool CommandCanExecuteAlterar(object param)
        {
            return (this._procedimentoendoscopiaselecionado.IsNotNull() && this._collectionprocedimentoendoscopia.HasItems());
        }
        protected override bool CommandCanExecuteSalvar(object param)
        {
            return (this._procedimentoendoscopiaselecionado.Descricao.IsNotEmptyOrWhiteSpace());
        }
        protected override void CommandSalvar(object param)
        {
            if (this._novo)
            {
                this._collectionprocedimentoendoscopia.Add(this._procedimentoendoscopiaselecionado);
                this._novo = false;
            }
            this.OnPropertyChanged<vmSumarioAvaliacaoMedicaComorbidadesEndoscopia>(x => x.CollectionProcedimentosEndoscopia);

            base.CommandSalvar(param);
        }
        protected override bool CommandCanExecuteExcluir(object param)
        {
            return (this._procedimentoendoscopiaselecionado.IsNotNull() && this._collectionprocedimentoendoscopia.HasItems());
        }
        protected override void CommandExcluir(object param)
        {
            if (DXMessageBox.Show("Deseja realmente excluir este Procedimento?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                this._collectionprocedimentoendoscopia.Remove(this._procedimentoendoscopiaselecionado);
                if (this._sumarioavaliacaoendoscopiaselecionado.ProcedimentosEndoscopia.Count(x => x.ID == this._procedimentoendoscopiaselecionado.ID) > 0)
                    this._sumarioavaliacaoendoscopiaselecionado.ProcedimentosEndoscopia.Remove(this._sumarioavaliacaoendoscopiaselecionado.ProcedimentosEndoscopia.SingleOrDefault(x => x.ID == this._procedimentoendoscopiaselecionado.ID));
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaComorbidadesEndoscopia>(x => x.CollectionProcedimentosEndoscopia);
            }
        }
        protected override void CommandFechar(object param)
        {
            this._procedimentoendoscopiaselecionado.CancelEdit();
            base.CommandFechar(param);
        }
        #endregion
    }
}
