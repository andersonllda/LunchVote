using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.UrgenciaP;
using HMV.Core.Domain.Model;
using System.Windows;
using System.Collections.Generic;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winUrgenciaPediatrica.xaml
    /// </summary>
    public partial class winUrgenciaPediatrica : WindowBase
    {
        public winUrgenciaPediatrica(vmUrgenciaPediatrica pVm)
        {            
            this.DataContext = pVm;
            if (!(this.DataContext as vmUrgenciaPediatrica).VerificaDataNascimento())
            {
                this.Close();
                return;
            }
            else
                (this.DataContext as vmUrgenciaPediatrica).AddNovaUrgencia(App.Usuario);

            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCalcular_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmUrgenciaPediatrica).Calcula();
        }

        private void grid_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
        {
            e.Result = Comparer<int>.Default.Compare(e.ListSourceRowIndex1,
                e.ListSourceRowIndex2);

            e.Handled = true;
        }       
    }
}
