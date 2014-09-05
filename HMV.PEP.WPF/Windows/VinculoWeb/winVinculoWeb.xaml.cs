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

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winVinculoWeb.xaml
    /// </summary>
    public partial class winVinculoWeb : WindowBase
    {
        public winVinculoWeb(Atendimento pAtendimento)
        {
            this.DataContext = new vmVinculoWeb(pAtendimento, App.Usuario);
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVincular_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmVinculoWeb).VerificaSeAbreConfirmacao())
            {
                winVinculoWebConfirma win = new winVinculoWebConfirma((this.DataContext as vmVinculoWeb));
                win.ShowDialog(this);              
            }
            else           
                (this.DataContext as vmVinculoWeb).VinculaSumarios();

            if ((this.DataContext as vmVinculoWeb).ImprimeCO)
            {
                ucRelSumarioAvaliacaoMedica rel = new ucRelSumarioAvaliacaoMedica();
                rel.SetData((this.DataContext as vmVinculoWeb).SumarioAvaliacaoMedica.DomainObject);
                rel.Show();

                (this.DataContext as vmVinculoWeb).DeletaSumarioWeb();
            }

            this.Close();
        }
    
        private void grdSumarios_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)grdSumarios.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)grdSumarios.View).BestFitColumns()));
        }

        private void grdInternados_Loaded(object sender, RoutedEventArgs e)
        {            
            ((TableView)grdInternados.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)grdInternados.View).BestFitColumns()));
        }

        private void optPacientesDoProfissional_Checked(object sender, RoutedEventArgs e)
        {
            ((TableView)grdSumarios.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)grdSumarios.View).BestFitColumns()));
            ((TableView)grdInternados.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)grdInternados.View).BestFitColumns()));
        }
    }
}
