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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucMedicoAssistente.xaml
    /// </summary>
    public partial class ucMedicoAssistente : UserControlBase
    {
        public ucMedicoAssistente()
        {
            InitializeComponent();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelProfissional win = new winSelProfissional(false);
            win.ShowDialog(base.OwnerBase);
            if (win.GetPrestador() != null)
                (this.DataContext as vmOrientacoesDoMedicoAssistente).Prestador = new wrpPrestador(win.GetPrestador());
        }

        private void ButtonInfo_Click_1(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmOrientacoesDoMedicoAssistente).Prestador = null;
        }      
    }
}
