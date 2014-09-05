using System;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for ucSumarioAvaliacaoPreAnestesicaItens.xaml
    /// </summary>
    public partial class ucSumarioAvaliacaoPreAnestesicaItens : UserControlBase
    {        
        public ucSumarioAvaliacaoPreAnestesicaItens()
        {
            InitializeComponent();
        }

        //private void TableView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    ((TableView)sender).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)sender).BestFitColumns()));
        //}

        private void View_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);            
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            grdAnestesiaGrupo.RefreshData();           
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            grdAnestesiaGrupo.RefreshData();           
        }     
    }
}
