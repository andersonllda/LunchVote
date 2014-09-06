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
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winMsgProcedimentosRealizados.xaml
    /// </summary>
    public partial class winMsgProcedimentosRealizados : WindowBase
    {
        public winMsgProcedimentosRealizados(string pSolicitante, string pMedicamentoProcedimento)
        {
            InitializeComponent();

            txtSolicitante.Text = pSolicitante;
            txtMedicamentoProcedimento.Text = pMedicamentoProcedimento;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            if (chkDeseja.IsChecked == true)
                this.DialogResult = true;
            else
                this.DialogResult = false;
        }
    }
}
