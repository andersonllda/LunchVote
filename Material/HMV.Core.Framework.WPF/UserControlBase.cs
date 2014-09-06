using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.Core.Framework.WPF
{
    public class UserControlBase : UserControl, IBase
    {
        #region ----- Propriedades Públicas -----
        public ViewModelBase vmObject
        {
            get
            {
                if (!this.DataContext.IsNull() && this.DataContext.GetType().BaseType == typeof(ViewModelBase))
                    return (ViewModelBase)this.DataContext;
                return null;
            }
        }

        public ImageSource IconeImagem
        {
            get { return this._iconeimagem; }
            set { this._iconeimagem = value; }
        }

        public Window OwnerBase
        {
            get { return Window.GetWindow(this); }
        }
        #endregion

        #region ----- Propriedades Protegidas -----
        protected TelaType TelaAbrirCadastroBaseType
        {
            set
            {
                this._telaabrircadastrobase = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaAbrirType
        {
            set
            {
                this._telaabrir = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaAbrirExType
        {
            set
            {
                this._telaabrirex = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaAbrirEx01Type
        {
            set
            {
                this._telaabrirex01 = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaAlterarType
        {
            set
            {
                this._telaalterar = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaIncluirType
        {
            set
            {
                this._telaincluir = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaImprimirType
        {
            set
            {
                this._telaimprimir = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaVisualizarType
        {
            set
            {
                this._telavisualizar = value;
                this.SetEvents();
            }
        }
        protected TelaType TelaPesquisarType
        {
            set
            {
                this._telapesquisar = value;
                this.SetEvents();
            }
        }        
        #endregion

        #region ----- Propriedades Privadas -----
        private ImageSource _iconeimagem;
        private TelaType _telaabrir { get; set; }
        private TelaType _telaabrirex { get; set; }
        private TelaType _telaabrirex01 { get; set; }
        private TelaType _telaabrircadastrobase { get; set; }
        private TelaType _telaalterar { get; set; }
        private TelaType _telaincluir { get; set; }
        private TelaType _telaimprimir { get; set; }
        private TelaType _telavisualizar { get; set; }
        private TelaType _telapesquisar { get; set; }
        #endregion

        #region ----- Construtor -----
        public UserControlBase() { this.DataContext = null; }

        protected UserControlBase(ViewModelBase pVMObject)
        {
            this.SetData(pVMObject);
        }
        #endregion

        #region ----- Métodos Privados -----
        protected virtual void SetEvents()
        {
            if (this.vmObject.IsNull())
                return;

            //Atribui os EventHandlers aos seus respectivos métodos.            
            if (!this.vmObject.IsEventHandlerRegistered("EventCommandAbrir") && !this._telaabrir.IsNull())
                this.vmObject.EventCommandAbrir += this.ChamaTelaAbrir;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandAbrirEx") && !this._telaabrirex.IsNull())
                this.vmObject.EventCommandAbrirEx += this.ChamaTelaAbrirEx;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandAbrirEx01") && !this._telaabrirex01.IsNull())
                this.vmObject.EventCommandAbrirEx += this.ChamaTelaAbrirEx01;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandIncluir") && !this._telaincluir.IsNull())
                this.vmObject.EventCommandIncluir += this.ChamaTelaIncluir;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandAlterar") && (!this._telaalterar.IsNull() || !this._telaincluir.IsNull()))
                this.vmObject.EventCommandAlterar += this.ChamaTelaAlterar;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandImprimir") && !this._telaimprimir.IsNull())
                this.vmObject.EventCommandImprimir += this.ChamaTelaImprimir;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandVisualizar") && !this._telavisualizar.IsNull())
                this.vmObject.EventCommandVisualizar += this.ChamaTelaVisualizar;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandPesquisar") && !this._telapesquisar.IsNull())
                this.vmObject.EventCommandPesquisar += this.ChamaTelaPesquisar;
        }
        #endregion

        #region ----- Métodos Públicos -----
        public virtual void SetData(ViewModelBase pVMObject)
        {
            this.DataContext = pVMObject;
            this.SetEvents();
        }     
        #endregion        

        #region ----- Métodos Override -----
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.vmObject.IsNull())
                if (e.Property == FrameworkElement.DataContextProperty)              
                    this.SetEvents();                

            base.OnPropertyChanged(e);
        }
        #endregion

        #region ----- Métodos Protegidos -----
        protected virtual void ChamaTelaVisualizar(object sender, EventArgs e)
        {
            this.AbreWindow(this._telavisualizar);
        }

        protected virtual void ChamaTelaImprimir(object sender, EventArgs e)
        {
            this.AbreWindow(this._telaimprimir);
        }

        protected virtual void ChamaTelaAbrir(object sender, EventArgs e)
        {
            this.AbreWindow(this._telaabrir);
        }

        protected virtual void ChamaTelaAbrirEx(object sender, EventArgs e)
        {
            this.AbreWindow(this._telaabrirex);
        }

        protected virtual void ChamaTelaAbrirEx01(object sender, EventArgs e)
        {
            this.AbreWindow(this._telaabrirex01);
        }

        protected virtual void ChamaTelaIncluir(object sender, EventArgs e)
        {
            this.AbreWindow(this._telaincluir);
        }

        protected virtual void ChamaTelaAlterar(object sender, EventArgs e)
        {
            if (this._telaalterar.IsNotNull())
                this.AbreWindow(this._telaalterar);
            else
                this.ChamaTelaIncluir(sender, e);
        }

        protected void ExecutaCommandAlterarSelecionar(object param)
        {
            if (this.vmObject.MostraSelecionar)
                this.vmObject.Commands.ExecuteCommand(enumCommand.CommandSelecionar, param);
            else
                this.vmObject.Commands.ExecuteCommand(enumCommand.CommandAlterar, param);
        }

        protected void ExecutaCommandSalvar(object param)
        {
            this.vmObject.Commands.ExecuteCommand(enumCommand.CommandSalvar, param);
        }

        protected virtual void ChamaTelaPesquisar(object sender, EventArgs e)
        {
            this.AbreWindow(this._telapesquisar);
        }
        #endregion                                
    }
}
