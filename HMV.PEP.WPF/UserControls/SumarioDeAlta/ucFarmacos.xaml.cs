using System.Windows;
using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using DevExpress.Xpf.Grid;
using System.Windows.Input;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucFarmacos.xaml
    /// </summary>
    public partial class ucFarmacos : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucFarmacos()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmFarmacos((pData as vmSumarioAlta).SumarioAlta, App.Usuario);
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmFarmacos).BeginEdit();
            winSelMedicamentos win = new winSelMedicamentos((this.DataContext as vmFarmacos));
            if (win.ShowDialog(base.OwnerBase).Equals(false))
                (this.DataContext as vmFarmacos).CancelEdit();
            (this.DataContext as vmFarmacos).EndEdit();
        }

        private void txtObservacao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtObservacao.Text))
                label1.Content = string.Format(txtObservacao.Text.Length < 499 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 500 - txtObservacao.Text.Length);
            else
                label1.Content = "Máximo 500 caracteres";

        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmFarmacos).MarcaTodos();
        }

        private void ckbSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmFarmacos).DesMarcaTodos();
        }

        private void viewListaFarmacos_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            btnIncluir.Focus();
            gdFarmaco.Focus();
        }
    }
}
