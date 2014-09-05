using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using HMV.PEP.DTO;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for winPosAltaUltimaPrescricao.xaml
    /// </summary>
    public partial class winPosAltaUltimaPrescricao : WindowBase
    {
        public winPosAltaUltimaPrescricao(IList<MedicamentoPosAltaDTO> pMedicamentoPosAlta, wrpSumarioAlta pSumarioAlta)
        {
            this.DataContext = new vmPosAlta(pMedicamentoPosAlta, pSumarioAlta, App.Usuario);
            InitializeComponent();
        }        

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (MedicamentoPosAltaDTO item in (IList<MedicamentoPosAltaDTO>)gdUltimaPrescricao.ItemsSource)
                item.Marcado = ((CheckEdit)sender).IsChecked.Value;

            gdUltimaPrescricao.RefreshData();
        }

        private void TableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
                btnConfirmar_Click(this, null);
        
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPosAlta).AdicionaMedicamentosDTOPlanosPosAlta();
            this.Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
