using System.Windows;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winCadastroAlta.xaml
    /// </summary>
    public partial class winCadastroAlta : WindowBase
    {

        public winCadastroAlta(vmBoletimEmergencia pvmBoletim)
        {
            InitializeComponent();

            this.DataContext = new vmCadastroAlta(pvmBoletim);
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmCadastroAlta).IsValid)
            {
                winConfirmaClassificacao win = new winConfirmaClassificacao((this.DataContext as vmCadastroAlta).vmBoletimEmergencia);
                win.ShowDialog(this);
                if (win.DialogResult.HasValue)
                    if (win.DialogResult.Value)
                    {
                        if ((this.DataContext as vmCadastroAlta).Salva())
                            this.Close();
                    }
            }
        }
    }
}
