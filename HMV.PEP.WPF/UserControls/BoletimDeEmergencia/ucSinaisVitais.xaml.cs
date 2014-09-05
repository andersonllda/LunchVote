using System.Windows.Input;
using DevExpress.Xpf.Grid;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucSinaisVitais.xaml
    /// </summary>
    public partial class ucSinaisVitais : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucSinaisVitais()
        {
            InitializeComponent();           
        }

        public void SetData(object pData)
        {
            vmSinaisVitais vm = pData as vmSinaisVitais;
            this.DataContext = vm;
            colSAT.Visible = vm.SATVisible;
            colPESO.Visible = vm.PESOVisible;
            colBCF.Visible = vm.BCFVisible;
        }

        private void tbvSinaisVitais_RowUpdated(object sender, RowEventArgs e)
        {
            (sender as TableView).CommitEditing();
            (this.DataContext as vmSinaisVitais).SalvaSinalVital(e.Row);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as vmSinaisVitais).DeletaSinalVital(grdSinaisVitais.View.FocusedRow);
        }      
    }
}
