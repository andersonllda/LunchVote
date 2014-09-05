using System.Windows;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros
{
    /// <summary>
    /// Interaction logic for winCadEvolucaoPadrao.xaml
    /// </summary>
    public partial class winCadEvolucao: WindowBase
    {
        public winCadEvolucao(vmRegistrosDeEvolucao pData)
        {
            InitializeComponent();
            pData.NovoRegistro();            
            this.DataContext = pData;            
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public bool CancelClose
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        private void txtObservacao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtObservacao.Text))                
                label1.Content = "Máximo 12000 caracteres";
            else 
                label1.Content = string.Format(txtObservacao.Text.Length < 11999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 12000 - txtObservacao.Text.Length);
        }
    }
}
