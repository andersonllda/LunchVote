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

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaCTINEO
{
    public partial class ucP3 : UserControlBase
    {
        public ucP3()
        {
            InitializeComponent();
        }

        private void TableView_CellValueChanging(object sender, DevExpress.Xpf.Grid.CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
        }
    }
}
