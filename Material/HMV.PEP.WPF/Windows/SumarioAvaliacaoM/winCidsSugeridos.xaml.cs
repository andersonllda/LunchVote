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

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for winCidsSugeridos.xaml
    /// </summary>
    public partial class winCidsSugeridos : WindowBase
    {       
        public winCidsSugeridos(Core.Domain.Model.Paciente pPaciente)
        {
            InitializeComponent();
            this.DataContext = pPaciente;
            ucListaDeProblemas.SetData(this.DataContext);
            ucListaDeProblemas.ReadOnly();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
