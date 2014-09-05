using System.Windows;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winCadastroAlta.xaml
    /// </summary>
    public partial class winCadPAME : WindowBase
    {
        private bool NaoFechar = true;
        public winCadPAME(vmBoletimEmergencia pvmBoletim)
        {
            InitializeComponent();

            this.DataContext = pvmBoletim;
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmBoletimEmergencia).IsValidPAME)
            {
                NaoFechar = false;
                this.Close();
            }
        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = NaoFechar;
        }              
    }
}
