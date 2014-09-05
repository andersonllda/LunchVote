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
using System.Windows.Threading;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winVisualizaListaProblema.xaml
    /// </summary>
    public partial class winVisualizaListaProblema : WindowBase
    {
        public winVisualizaListaProblema(Paciente pPaciente)
        {
            InitializeComponent();
            rbAtivos.IsChecked = true;
            this.Filtro_Checked(null, null);

            this.DataContext = pPaciente;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Filtro_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gdListaProblemas != null)
                {
                    if (rbAtivos.IsChecked == true)
                    {
                        gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Ativo.ToString(), BinaryOperatorType.Equal));
                    }
                    //else if (rbExcluidos.IsChecked == true)
                    //{
                    //    gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Excluído.ToString(), BinaryOperatorType.Equal));
                    //}
                    else if (rbInativos.IsChecked == true)
                    {
                        gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Inativo.ToString(), BinaryOperatorType.Equal));
                    }
                    else
                    {
                        //gdListaProblemas.FilterCriteria = null;
                        gdListaProblemas.FilterCriteria = (new BinaryOperator("Status", StatusAlergiaProblema.Excluído.ToString(), BinaryOperatorType.NotEqual));
                    }
                }
            }
            catch (Exception ex)
            {
                DXMessageBox.Show(ex.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void gdListaProblemas_Loaded(object sender, RoutedEventArgs e)
        {
            ((TableView)gdListaProblemas.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdListaProblemas.View).BestFitColumns()));
        }


    }
}
