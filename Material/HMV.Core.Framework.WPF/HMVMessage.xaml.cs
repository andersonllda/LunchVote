using System;
using System.Windows;

namespace HMV.Core.Framework.WPF
{
    /// <summary>
    /// Interaction logic for HMVMessage.xaml
    /// </summary>
    public partial class HMVMessage : Window
    {
        public HMVMessage(String pMsg)
        {
            InitializeComponent();
            txtMsg.Content = pMsg;
            ShowDialog();
        }

        private void btnFechar_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
