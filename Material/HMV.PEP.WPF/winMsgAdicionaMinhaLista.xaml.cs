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
    /// Interaction logic for winMsgAdicionaMinhaLista.xaml
    /// </summary>
    public partial class winMsgAdicionaMinhaLista : WindowBase
    {
        public winMsgAdicionaMinhaLista()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
