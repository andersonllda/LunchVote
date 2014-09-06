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
using System.Windows.Shapes;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.RegistroDor;
using DevExpress.Xpf.Carousel;
using DevExpress.XtraReports.UI;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.RegistroDor
{
    /// <summary>
    /// Interaction logic for winEvolucao.xaml
    /// </summary>
    public partial class winEvolucao : WindowBase
    {
        public winEvolucao(vmRegistrosDeDor ObjvmRegistrosDeDor)
        {
            InitializeComponent();
            this.DataContext = ObjvmRegistrosDeDor;
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmRegistrosDeDor).MarcaTodosRegistroDor();
        }

        private void ckbSelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmRegistrosDeDor).DesMarcaTodosRegistroDor();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((this.DataContext as vmRegistrosDeDor).ListaRegistroDorEvolucaoGrafico.Count > 0)
                {
                    rptRegistroDor report = new rptRegistroDor();
                    report.DataSource = (this.DataContext);

                    winRelatorio win = new winRelatorio(report, true, "Evolução.", false);
                    win.ShowDialog(this);
                }
                else
                    DXMessageBox.Show("Não há registros selecionado para imprimir o relatório!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch { }
        }

    }
}
