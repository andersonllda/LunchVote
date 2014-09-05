using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for ucListaPacientesInternados.xaml
    /// </summary>
    public partial class ucTabMinhaEquipe : UserControlBase, IUserControl
    {
        #region --- Construtor ---

        public ucTabMinhaEquipe(WindowBase winPep)
        {
            InitializeComponent();
            _winPep = winPep;
            loadMinhaEquipe();
        }

        #endregion

        #region --- Propriedades Privadas ---
        WindowBase _winPep;
        #endregion

        #region --- Propriedades Púplicas ---
        public bool CancelClose { get; set; }
        #endregion

        #region --- Métodos Privados ---
        private void loadMinhaEquipe()
        {
            if (App.Usuario.Prestador == null)
                return;

            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            var prest = serv.FiltraPorId(App.Usuario.Prestador.Id);

            IList<Prestador> prestador = prest.getEquipeMedica();
            gdMinhaEquipe.ItemsSource = prestador;

            if (prestador.Count > 0)
                btnExcluirEquipeMedica.IsEnabled = true;
            else
                btnExcluirEquipeMedica.IsEnabled = false;
        }
        #endregion

        #region --- Métodos Públicos ---
        public void SetData(object pData)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region --- Eventos ---
        private void btnAdicionaEquipeMedica_Click(object sender, RoutedEventArgs e)
        {
            IRepositorioDeUsuarios rep = ObjectFactory.GetInstance<IRepositorioDeUsuarios>();
            rep.Refresh(App.Usuario);

            winSelProfissional win = new winSelProfissional();
            win.ShowDialog(_winPep);
            Prestador prestadorSelecionado = win.GetPrestador();

            if (prestadorSelecionado != null && App.Usuario.Prestador != null)
            {
                try
                {
                    IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
                    App.Usuario.Prestador.addEquipeMedica(prestadorSelecionado);
                    serv.Save(App.Usuario.Prestador);
                    loadMinhaEquipe();
                }
                catch (BusinessValidatorException err)
                {
                    DXMessageBox.Show(err.GetErros()[0].Message, "ATENÇÃO", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnExcluirEquipeMedica_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Confirma exclusão do prestador da equipe ? ", "ATENÇÃO", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;

            Prestador prestadorSelecionado = (Prestador)gdMinhaEquipe.GetFocusedRow();
            if (prestadorSelecionado != null)
            {
                IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
                App.Usuario.Prestador.removeEquipeMedica(prestadorSelecionado);
                serv.Save(App.Usuario.Prestador);
                loadMinhaEquipe();
            }
        }

        private void gdMinhaEquipe_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdMinhaEquipe.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdMinhaEquipe.View).BestFitColumns()));
        }

        private void gdMinhaEquipe_KeyDown(object sender, KeyEventArgs e)
        {
            if ((sender as GridControl).VisibleRowCount > 0)
            {
                if (e.Key.ToString().Length == 1)
                    if (char.IsLetter(e.Key.ToString(), 0))
                        this.SetKeys(e.Key.ToString(), (sender as GridControl), "MINHA_EQUIPE");
                if (e.Key.ToString().Length == 2)
                    if (char.IsDigit(e.Key.ToString(), 1))
                        this.SetKeys(e.Key.ToString().Right(1), (sender as GridControl), "MINHA_EQUIPE");
            }
        }

        #region SETFOCUS por Tecla!
        private string _keys = string.Empty;
        public void SetKeys(string pkey, GridControl pGrid, string QualGrid)
        {
            if (this._keys.IsEmpty())
                Do(() => SetFocusedRow(pGrid, QualGrid), 1000);

            this._keys += pkey;
            Do(() => this._keys = string.Empty, 1200);
        }


        private void SetFocusedRow(GridControl pGrid, string QualGrid)
        {
            if (QualGrid.Equals("MINHA_EQUIPE"))
                pGrid.View.FocusedRow = (pGrid.ItemsSource as IList<Prestador>).Where(x => x.Nome.ToUpper().StartsWith(this._keys.ToUpper())).FirstOrDefault();

            this._keys = string.Empty;
        }

        #region DelayAction
        private readonly static TimerCallback timer = new TimerCallback(ExecuteDelayedAction);

        private static void ExecuteDelayedAction(object o)
        {
            App.Current.Dispatcher.Invoke((o as Action));
            return;
        }

        public static void Do(Action action, TimeSpan delay, int interval = Timeout.Infinite)
        {
            new Timer(timer, action, Convert.ToInt32(delay.TotalMilliseconds), interval);
            return;
        }

        public static void Do(Action action, int delay, int interval = Timeout.Infinite)
        {
            Do(action, TimeSpan.FromMilliseconds(delay), interval);

            return;
        }
        #endregion

        #endregion

        #endregion  
        
    }
}
