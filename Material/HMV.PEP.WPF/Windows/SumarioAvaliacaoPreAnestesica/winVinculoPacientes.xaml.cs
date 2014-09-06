using System;
using System.Configuration;
using System.Windows;
using HMV.Core.Domain.Model;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using DevExpress.Xpf.Grid;
using System.Windows.Threading;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.UserControls.SumarioAvaliacaoM;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for winVinculoWeb.xaml
    /// </summary>
    public partial class winVinculoPacientes : WindowBase
    {
        public winVinculoPacientes(vmSumarioAvaliacaoPreAnestesica pVm)
        {
            this.DataContext = pVm;
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVincular_Click(object sender, RoutedEventArgs e)
        {
           (this.DataContext as vmSumarioAvaliacaoPreAnestesica).VincularSumarioWeb();
           this.Close();
        }
    
        private void grdSumarios_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)grdSumarios.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)grdSumarios.View).BestFitColumns()));
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmSumarioAvaliacaoPreAnestesica).ExcluirSumarioWeb();         
        }      
    }
}
