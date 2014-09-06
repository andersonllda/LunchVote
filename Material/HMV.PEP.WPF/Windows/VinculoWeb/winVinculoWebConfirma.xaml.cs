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
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winVinculoWebConfirma.xaml
    /// </summary>
    public partial class winVinculoWebConfirma : WindowBase
    {
        public winVinculoWebConfirma(vmVinculoWeb pvmVinculo)
        {
            this.DataContext = pvmVinculo;
            InitializeComponent();
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            (this.DataContext as vmVinculoWeb).VinculaSumarios();
            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
