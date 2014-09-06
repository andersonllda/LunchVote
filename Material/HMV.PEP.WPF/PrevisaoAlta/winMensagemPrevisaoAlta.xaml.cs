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
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.Evolucao;

namespace HMV.PEP.WPF.PrevisaoAlta
{
    /// <summary>
    /// Interaction logic for winMensagemPrevisaoAlta.xaml
    /// </summary>
    public partial class winMensagemPrevisaoAlta : WindowBase
    {
        public winMensagemPrevisaoAlta(ViewModelBase pVM)
            : base(pVM)
        {
            InitializeComponent();
        }

        void wnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            winNovaPrevisaoAlta win = new winNovaPrevisaoAlta(new HMV.PEP.ViewModel.PEP.Evolucao.VMPrevisaoAlta(App.Usuario, (this.DataContext as VMPrevisaoAltaConsulta).Atendimento, false, false));
            win.ShowDialog(null);
            Close();
        }
    }
}
