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
using HMV.PEP.WPF.Windows.PAME;
using System.Configuration;
using HMV.Core.Framework.Helper;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winSelPepPame.xaml
    /// </summary>
    public partial class winSelPepPame : Window
    {
        public bool AbrirPEP = false;

        public winSelPepPame()
        {
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Normal;
        }

        private void btnPame_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            winPAME win = new winPAME();
            win.Show();
        }

        private void btnPEP_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            winPEP win = new winPEP();
            win.Show();
        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown(-1);
        }
    }
}
