using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Windows.Input;
using HMV.PEP.ViewModel.Commands;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.Core.Framework.Expression;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Enum;
namespace HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2
{
    public class vmMotivoInternacaoPim2 : ViewModelBase
    {
        #region ----- Construtor -----
        public vmMotivoInternacaoPim2(Atendimento pAtendimento, Usuarios pUsuarios, bool pIsVisiblePim2)
        {
            this._isvisiblepim2 = pIsVisiblePim2;
            this.SavePIN2Command = new SaveMotivoInternacaoPin2Command(this);
            this.RemovePIN2Command = new RemoveMotivoInternacaoPin2Command(this);
            this.Atendimento = new wrpAtendimento(pAtendimento);
            this.Usuario = new wrpUsuarios(pUsuarios);
            this._vmpin2 = new vmPIN2(this);
            this._vmmotivointernacao = new vmMotivoInternacao(this);
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpUsuarios _usuarios { get; set; }
        private wrpAtendimento _atendimento { get; set; }
        private vmPIN2 _vmpin2 { get; set; }
        private vmMotivoInternacao _vmmotivointernacao { get; set; }
        private bool _isvisiblepim2;
        #endregion

        #region ----- Propriedades Públicas -----
        public vmPIN2 vmPin2
        {
            get { return this._vmpin2; }
        }

        public vmMotivoInternacao vmMotivoDeInternacao
        {
            get { return this._vmmotivointernacao; }
        }

        public wrpAtendimento Atendimento
        {
            get { return this._atendimento; }
            set
            {
                this._atendimento = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacaoPim2>(x => x.Atendimento));
            }
        }

        public wrpUsuarios Usuario
        {
            get { return this._usuarios; }
            set
            {
                this._usuarios = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacaoPim2>(x => x.Usuario));
            }
        }

        public bool IsVisiblePim2
        {
            get { return _isvisiblepim2; }
            set
            {
                this._isvisiblepim2 = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacaoPim2>(x => x.IsVisiblePim2));
            }
        }

        public wrpPIN2Collection ListaPIN2
        {
            get { return this._atendimento.PIN2; }
        }

        public wrpMotivoInternacaoCollection MotivoInternacaoCollection
        {
            get { return new wrpMotivoInternacaoCollection(this._atendimento.DomainObject.MotivosInternacao); }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public void RefreshMotivoInternacao()
        {
            OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacaoPim2>(x => x.Atendimento));
            OnPropertyChanged(ExpressionEx.PropertyName<vmMotivoInternacaoPim2>(x => x.MotivoInternacaoCollection));
        }

        public void ImprimiuORelatorio()
        {
            foreach (wrpPIN2 pin in Atendimento.PIN2)
            {
                pin.Impresso = SimNao.Sim;
            }

            foreach (wrpMotivoInternacao motivo in Atendimento.MotivoInternacao)
            {
                motivo.Impresso = SimNao.Sim;
            }
            Atendimento.Save();
        }
        #endregion

        #region ----- Commands -----
        public ICommand SavePIN2Command { get; set; }
        public ICommand RemovePIN2Command { get; set; }
        #endregion

        protected override void CommandAbrir(object param)
        {
            base.CommandAbrir(param);
        }
    }
}
