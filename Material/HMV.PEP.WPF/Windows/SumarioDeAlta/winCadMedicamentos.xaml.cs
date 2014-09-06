using System.Windows;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.SumarioDeAlta; 
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winCadMedicamentos.xaml
    /// </summary>
    public partial class winCadMedicamentos : WindowBase
    {
        public winCadMedicamentos(vmPosAlta pPosAlta)
        {
            this.DataContext = pPosAlta;
            InitializeComponent();
            if (pPosAlta.IsCadastroOutros)
                txtNomeComercial.Focus();
            else
                txtMedicamento.Focus();
        }       

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnGravarFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();

        }       
    }
}
