using System.Windows;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winSelPepPame.xaml
    /// </summary>
    public partial class winSelecionaSumarioDeAvaliacao : WindowBase
    {
        public bool SumarioRN;
        public bool SumarioCTINEO;

        public winSelecionaSumarioDeAvaliacao()
        {
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Normal;
        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {          
        }

        private void btnRN_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            SumarioRN = true;
            this.Close();
        }

        private void btnUTINEO_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            SumarioCTINEO = true;
            this.Close();
        }
    }
}
