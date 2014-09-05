using System;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Windows.CheckListUDI;

namespace HMV.PEP.WPF.UserControls.CheckListUDI
{
    /// <summary>
    /// Interaction logic for ucCheckListUDI.xaml
    /// </summary>
    public partial class ucCheckListUDI : UserControlBase, IUserControl
    {
        public ucCheckListUDI()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmCheckListUDI((pData as Atendimento), App.Usuario);
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Confirma exclusão do 'CheckList UDI' ?", "Atenção", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                ((vmCheckListUDI)this.DataContext).Remover();
            grdCheckList.RefreshData();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmCheckListUDI).CheckListdto.CheckList == null)
                ((vmCheckListUDI)this.DataContext).Novo();

            ((vmCheckListUDI)this.DataContext).IniciaVMS();
            winCadCheckList win = new winCadCheckList(int.Parse((sender as HMVButton).Name.Replace("btn", "")), (vmCheckListUDI)this.DataContext);
            win.ShowDialog(this.OwnerBase);
            grdCheckList.RefreshData();
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmCheckListUDI).CheckListdto.CheckList != null)
            {
                var rpt = new vmRelatorioChecklistUDI((this.DataContext as vmCheckListUDI).CheckListdto.CheckList).Relatorio();
                winRelatorio win = new winRelatorio(rpt, true, "Check List UDI", false);
                win.ShowDialog(this.OwnerBase);
            }
        }
    }
}
