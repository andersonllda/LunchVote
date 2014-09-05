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
using HMV.Core.Domain.Model;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winGED.xaml
    /// </summary>
    public partial class winGED : WindowBase
    {
        public winGED(Atendimento pAtendimento)
        {
            InitializeComponent();
            ucGED.MostraAtendimento = true;
            ucGED.SetData(pAtendimento);
            this.Title = "Atendimento: " + pAtendimento.ID.ToString();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
