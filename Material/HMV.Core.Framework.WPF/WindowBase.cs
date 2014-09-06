using System;
using System.Windows;
using System.Windows.Media;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.Core.Framework.WPF
{
    public class WindowBase : Window, IBase
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
            get { return this; }
        }

        //Propriedade que controla o Close de uma VM que estiver vinculada a duas ou mais Views.
        public bool CancelaCloseVM { get; set; }
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
        private bool _jafechou { get; set; }
        #endregion

        #region ----- Eventos Públicos -----
        public event EventHandler<MinimizeEventArgs> MinimizeEvent;
        #endregion

        #region ----- Construtor -----
        public WindowBase() { }

        protected WindowBase(ViewModelBase pVMObject)
        {
            this.DataContext = pVMObject;
            this.SetEvents();
        }
        #endregion

        #region ----- Métodos Privados -----
        private void SetEvents()
        {
            if (this.vmObject.IsNull())
                return;

            //Atribui os EventHandlers aos seus respectivos métodos.            
            this.vmObject.ActionCommandFechar += this._fechatela;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandAbrir") && !this._telaabrir.IsNull())
                this.vmObject.EventCommandAbrir += this.ChamaTelaAbrir;

            if (!this.vmObject.IsEventHandlerRegistered("EventCommandAbrirEx") && !this._telaabrirex.IsNull())
                this.vmObject.EventCommandAbrirEx += ChamaTelaAbrirEx;

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

        private void _fechatela()
        {
            this._jafechou = true;
            try
            {
                this.Close();
            }
            catch { }
            this._jafechou = false;
        }

        private void OnMinimize()
        {
            if (MinimizeEvent != null) MinimizeEvent(this, new MinimizeEventArgs(this.WindowState));
        }
        #endregion

        #region ----- Métodos Públicos -----
        /// <summary>
        /// NÃO UTILIZAR ESTE MÉTODO! Pois sempre é necessário enviar a window OWNER.
        /// Use a assinatura que envia a window pai.
        /// </summary>
        /// <returns>NotSupportedException</returns>
        [Obsolete("Método ShowDialog() não suportado para esta classe.", true)]
        public new bool? ShowDialog()
        {
            throw new NotSupportedException("Método ShowDialog() está obsoleto, utilizar o ShowDialog(Window pOwner)!" + Environment.NewLine + this.GetType().Name);
        }

        /// <summary>
        /// Abre a window na forma modal.
        /// </summary>
        /// <returns>bool?</returns>
        public bool? ShowDialog(Window pOwner)
        {
            this.Owner = pOwner;
            return base.ShowDialog();
        }
        /// <summary>
        /// Abre a window na forma modal.
        /// </summary>
        /// <returns>bool?</returns>
        public bool? ShowDialog(WindowBase pOwner)
        {
            if (!pOwner.IsNull())
                this.MinimizeEvent += new EventHandler<MinimizeEventArgs>(pOwner.MinimizeEventVoid);

            this.Owner = pOwner;
            return base.ShowDialog();
        }
        #endregion

        #region ----- Métodos Override -----
        protected override void OnClosed(EventArgs e)
        {
            if (!this.vmObject.IsNull() && !this._jafechou && !CancelaCloseVM)
            {
                this.vmObject.Commands.ExecuteCommand(enumCommand.CommandFechar, null);
                CancelaCloseVM = false;
            }
            else
                base.OnClosed(e);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.vmObject.IsNull())
                if (e.Property == FrameworkElement.DataContextProperty)
                    this.SetEvents();

            base.OnPropertyChanged(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            this.OnMinimize();
            base.OnStateChanged(e);
        }
        #endregion

        #region ----- Métodos Protegidos -----
        protected void MinimizeEventVoid(object sender, MinimizeEventArgs e)
        {
            //this.WindowState = e.State;            
            if (e.State == WindowState.Minimized)
                this.Hide();
            else
                this.Show();
        }

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
            if (this._telaalterar != null)
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
