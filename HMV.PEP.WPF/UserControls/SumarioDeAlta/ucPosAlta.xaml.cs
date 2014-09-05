using System.Windows;
using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers;
using HMV.PEP.WPF.Cadastros.SumarioDeAlta;
using HMV.Core.Domain.Enum;
using DevExpress.Xpf.Grid;
using System.Text.RegularExpressions;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucPosAlta.xaml
    /// </summary>
    public partial class ucPosAlta : UserControlBase, IUserControl
    {
        public ucPosAlta()
        {
            InitializeComponent();            
        }

        public bool CancelClose { get; set; }  

        public void SetData(object pData)
        {
            this.DataContext = new vmPosAlta((pData as vmSumarioAlta).SumarioAlta, App.Usuario);
        }

        private void btnIncluirPossivel_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPosAlta).TipoMedicamento = TipoMedicamentoPosAlta.Possivel;
            winSelMedicamentos win = new winSelMedicamentos((vmPosAlta)this.DataContext);
            win.ShowDialog(base.OwnerBase);
            gdPosAlta.RefreshData();
        }
         
        private void btnIncluirPrescrito_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPosAlta).TipoMedicamento = TipoMedicamentoPosAlta.Prescrito;
           
            winSelMedicamentos wins = new winSelMedicamentos((vmPosAlta)this.DataContext);
            if ((this.DataContext as vmPosAlta).MedicamentosItens.Count > 0)
                wins.ShowDialog(base.OwnerBase);
            else
                DXMessageBox.Show("Não há medicamentos " + TipoMedicamentoPosAlta.Prescrito + " para selecionar","Atenção",MessageBoxButton.OK,MessageBoxImage.Information);
            gdPosAlta.RefreshData();
        }

        private void btnIncluirOutros_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPosAlta).TipoMedicamento = TipoMedicamentoPosAlta.Outros;
            winCadMedicamentos win = new winCadMedicamentos((vmPosAlta)this.DataContext);
            win.ShowDialog(base.OwnerBase);
            gdPosAlta.RefreshData();
        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmPosAlta).PlanoPosAltaSelecionado != null)
            {
                vmPosAlta vm = (vmPosAlta)this.DataContext;

                vm.TipoMedicamento = null;
                vm.PlanoPosAltaSelecionado.BeginEdit();
                winCadMedicamentos win = new winCadMedicamentos(vm);
                if (win.ShowDialog(base.OwnerBase).Equals(false))
                    vm.PlanoPosAltaSelecionado.CancelEdit();
              // else
              //      vm.PlanoPosAltaSelecionado.EndEdit();
                gdPosAlta.RefreshData();
            }
        }

        private void gdPosAlta_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as GridControl).View.FocusedRow = (sender as GridControl).View.FocusedRow;
        }

        private void gdPosAlta_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
              btnAlterar_Click(this, null);
        }
    }
}
