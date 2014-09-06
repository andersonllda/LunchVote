using System.Windows;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.SumarioDeAlta;

namespace HMV.PEP.WPF.Evolucao
{
    /// <summary>
    /// Interaction logic for winCadEvolucaoPadrao.xaml
    /// </summary>
    public partial class winCadEvolucaoPadrao : WindowBase
    {
        public winCadEvolucaoPadrao(vmPEPEvolucaoPadrao pVm)
        {            
            InitializeComponent();
            this.DataContext = pVm;           
        }
      
        private void txtDescricao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtObservacao.Text))
                label1.Content = "Máximo 8000 caracteres";
            else label1.Content = string.Format(txtObservacao.Text.Length < 7999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 8000 - txtObservacao.Text.Length); 
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPEPEvolucaoPadrao).Titulo = string.Empty;
            (this.DataContext as vmPEPEvolucaoPadrao).Descricao = string.Empty;           
            this.Close();
        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            base.CancelaCloseVM = true;         
        }

        private void WindowBase_Closed(object sender, System.EventArgs e)
        {
            base.CancelaCloseVM = false;
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmPEPEvolucaoPadrao).Descricao.Length > 0
                && (this.DataContext as vmPEPEvolucaoPadrao).Titulo.Length > 0)
            {
                base.ExecutaCommandSalvar(null);
                this.Close();
            }
        }

    }
}
