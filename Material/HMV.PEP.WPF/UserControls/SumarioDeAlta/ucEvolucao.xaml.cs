using System.Windows;
using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucEvolucao.xaml
    /// </summary>
    public partial class ucEvolucao : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucEvolucao()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = ((pData as vmSumarioAlta).SumarioAlta);
        } 

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winCadEvolucaoPadrao win = new winCadEvolucaoPadrao((this.DataContext as wrpSumarioAlta).Evolucao);
            win.ShowDialog(base.OwnerBase);            
        }

        private void btnRecoPadrao_Click(object sender, RoutedEventArgs e)
        {
            winSelEvolucaoPadrao win = new winSelEvolucaoPadrao(this.DataContext as wrpSumarioAlta);
            win.ShowDialog(base.OwnerBase);
        }

        private void TextEdit_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            Caracter();
        }

        private void Caracter()
        {
            int NroCaracteres = 8000;

            if (string.IsNullOrEmpty(txtEvolucao.Text))
                label10.Content = "Máximo " + NroCaracteres.ToString() + " caracteres";
            else label10.Content = string.Format(txtEvolucao.Text.Length < (NroCaracteres - 1) ? "Máximo {0} caracteres" : "Máximo {0} caracter", NroCaracteres - txtEvolucao.Text.Length);
        }

        private void txtEvolucao_GotFocus(object sender, RoutedEventArgs e)
        {
            Caracter();
        }
    }
}
