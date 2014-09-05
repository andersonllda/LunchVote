using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.PEP.WPF.Windows;
using System.Linq;
using System;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucItensDeRegistro.xaml
    /// </summary>
    public partial class ucItensDeRegistro : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucItensDeRegistro()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmBoletimEmergencia).vmItensDeRegistro;
        }

        public void SetFocusTexto()
        {
            txtTexto.Focus();         
        }

        private void tvTipoAvaliacao_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if ((this.DataContext as vmItensRegistro).AbreExamesSolicitados)
            {
                winExamesPossiveis win = new winExamesPossiveis();
                win.ShowDialog(base.OwnerBase);
                if (win.DialogResult.HasValue)
                    if (win.DialogResult.Value)
                    {
                        (this.DataContext as vmItensRegistro).RegistraExamesSolicitados(win.RetornaProcedimento);
                    }
                e.Handled = true;
            }

            if ((this.DataContext as vmItensRegistro).AbreCadastroAlta)
            {
                if ((this.DataContext as vmItensRegistro).Valida())
                {
                    winCadastroAlta win = new winCadastroAlta((this.DataContext as vmItensRegistro).vmBoletimEmergencia);
                    win.ShowDialog(base.OwnerBase);
                    (this.DataContext as vmItensRegistro).RefreshBoletim();
                }

                Dispatcher.BeginInvoke(new Action(delegate
                {
                    (sender as TableView).FocusedRow = gdTipoAvaliacao.GetRow(0);
                }));
                e.Handled = true;
            }
        }

        private void UserControlBase_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as vmItensRegistro).AdicionaItem();
        }
    }
}
