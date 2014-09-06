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
using DevExpress.Xpf.Grid;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.WPF;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winEvolucoesEmergencia2.xaml
    /// </summary>
    public partial class winEvolucoesEmergencia2 : Window
    {
        public winEvolucoesEmergencia2()
        {
            InitializeComponent();
            this.DataContext = new vmEvolucaoEmergencia2();
            //_MontaGrid();
        }

        public bool _isVermelho = false;
        public bool isVermelho
        {
            get
            {
                return _isVermelho;
            }            
            set
            {
                _isVermelho = value;
            }
        }

        private void txtListaDeProblema_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {

        }

        private void txtPlano_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {

        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _MontaGrid()
        {
            List<EvolucaoEmergencia2> nova = new List<EvolucaoEmergencia2>();

            nova.Add(new EvolucaoEmergencia2
            {
                Nome = "Isak Silva Silva",
                Leito = "V02"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "V03",
                Nome = "Nathan Lima"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "V04",
                Nome = "Cecilia Maria"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L05",
                Nome = "Otavia de Borba"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L06",
                Nome = "Darcy Antonio"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L07",
                Nome = "Darcy José"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L08",
                Nome = "Mariza Mariza"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "A09",
                Nome = "Adela Pires"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "A10",
                Nome = "Esther Hulda"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "A11",
                Nome = "Beatriz"
            });

            grdEvolucao.ItemsSource = nova;

            //_AlteraCorLinha();
        }

        private void _AlteraCorLinha()
        {
            //int i = 0;

            //    //DataGridRow row = grdEvolucao.View.ros
            //    DataGridRow row = grdEvolucao.GetGroupRowValue(i) as DataGridRow;
            //    if (grdEvolucao.Leito.FirstOrDefault().ToString().ToUpper() == "V")
            //    {
            //        row.Background = Brushes.Red;
            //    }
            //    else if (grdEvolucao.Leito.FirstOrDefault().ToString().ToUpper() == "L")
            //    {
            //        row.Background = Brushes.Orange;
            //    }
            //}
        }
    }


    public class EvolucaoEmergencia2
    {
        public string Nome { get; set; }
        public string Leito { get; set; }
    }
}