using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HMV.Core.Framework.WPF;
using DevExpress.Xpf.Grid;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaRN
{
    /// <summary>
    /// Interaction logic for ucGestacoesAnteriores.xaml
    /// </summary>
    public partial class ucGestacoesAnteriores : UserControlBase
    {
        public ucGestacoesAnteriores()
        {
            InitializeComponent();
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.grdGestAnterior.IsEnabled = false;
            this.txtOutros.IsEnabled = false;
            this.chkSemInter.IsEnabled = false;
            this.chkSemInter.IsChecked = false;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdGestAnterior.IsEnabled = true;
            this.txtOutros.IsEnabled = true;
            this.chkSemInter.IsEnabled = true;
        }

        private void chkNega_Checked(object sender, RoutedEventArgs e)
        {
            this.grdGestAnterior.IsEnabled = false;
            this.txtOutros.IsEnabled = false;
            this.chkPrimeira.IsEnabled = false;
            this.chkPrimeira.IsChecked = false;
        }

        private void chkNega_Unchecked(object sender, RoutedEventArgs e)
        {
            this.grdGestAnterior.IsEnabled = true;
            this.txtOutros.IsEnabled = true;
            this.chkPrimeira.IsEnabled = true;
        }
    }
}
