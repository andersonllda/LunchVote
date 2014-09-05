using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica
{
    public class vmPreMedicacao : ViewModelBase
    {
        #region ----- Construtor -----
        public vmPreMedicacao(wrpSumarioAvaliacaoPreAnestesica pwrpSumarioAvaliacaoPreAnestesica)
        {
            this._sumarioavaliacaopreanestesica = pwrpSumarioAvaliacaoPreAnestesica;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoPreAnestesica _sumarioavaliacaopreanestesica { get; set; }
        private wrpSumarioAvaliacaoPreAnestesicaMedicamentos _premedicamentos { get; set; }
        private bool IsNovaPreMedicacao;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoPreAnestesicaMedicamentosCollection PreMedicacaoCollecion
        {
            get { return this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreanestesicaMedicamentos; }
        }

        public wrpSumarioAvaliacaoPreAnestesicaMedicamentos PreMedicamento
        {
            get { return _premedicamentos; }
            set
            {
                this._premedicamentos = value;
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.PreMedicamento));
            }
        }

        public SimNao? NaoHaPreMedicacao
        {
            get { return this._sumarioavaliacaopreanestesica.PreMedicacao; }
            set
            {
                if (value == SimNao.Sim)
                {
                    if (!this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreanestesicaMedicamentos.HasItems())
                        this._sumarioavaliacaopreanestesica.PreMedicacao = value;
                    else
                    {
                        this._sumarioavaliacaopreanestesica.PreMedicacao = SimNao.Nao;
                        DXMessageBox.Show("Não é possível marcar esta opção enquanto houver 'Medicação pré-anestésica' cadastrados.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                    this._sumarioavaliacaopreanestesica.PreMedicacao = value;

                base.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.NaoHaPreMedicacao));
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
            this.IsNovaPreMedicacao = true;
            this._premedicamentos = new wrpSumarioAvaliacaoPreAnestesicaMedicamentos(this._sumarioavaliacaopreanestesica);
            base.CommandIncluir(param);
            base.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.PreMedicacaoCollecion));
        }

        protected override bool CommandCanExecuteAlterar(object param)
        {
            return this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreanestesicaMedicamentos.HasItems();
        }

        protected override void CommandAlterar(object param)
        {
            this.IsNovaPreMedicacao = false;
            this._premedicamentos.BeginEdit();
            base.CommandAlterar(param);
            base.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.PreMedicacaoCollecion));
        }

        protected override bool CommandCanExecuteSalvar(object param)
        {            
            return !this._premedicamentos.IsNull() && this._premedicamentos.IsValid;
        }

        protected override void CommandSalvar(object param)
        {
            if (IsNovaPreMedicacao)
                this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreanestesicaMedicamentos.Add(this._premedicamentos);
            this._sumarioavaliacaopreanestesica.Save();
            base.CommandSalvar(param);
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.PreMedicacaoCollecion));            
        }

        protected override bool CommandCanExecuteExcluir(object param)
        {
            return this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreanestesicaMedicamentos.HasItems();
        }

        protected override void CommandExcluir(object param)
        {
            if (DXMessageBox.Show("Confirma exclusão da Medicação pré-anestésica? ", "Alerta", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                this._sumarioavaliacaopreanestesica.SumarioAvaliacaoPreanestesicaMedicamentos.Remove(this._premedicamentos);
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.PreMedicacaoCollecion));
                base.OnPropertyChanged(ExpressionEx.PropertyName<vmPreMedicacao>(x => x.PreMedicamento));
            }
        }

        protected override void CommandFechar(object param)
        {
            this._premedicamentos.CancelEdit();
            base.CommandFechar(param);
        }
        #endregion
    }
}
