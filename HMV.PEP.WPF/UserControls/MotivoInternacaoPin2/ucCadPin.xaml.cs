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
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.Core.Domain.Model;
using DevExpress.Xpf.Grid;
using HMV.PEP.WPF.Report.Pim2;
using HMV.PEP.WPF.Report;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.MotivoInternacaoPin2
{
    /// <summary>
    /// Interaction logic for ucPin.xaml
    /// </summary>
    public partial class ucCadPin : UserControlBase
    {
        public ucCadPin()
        {
            InitializeComponent(); 
        }     
      
        private void btnAjuda_Click(object sender, RoutedEventArgs e)
        {
            rptPim2Especificacao rel = new rptPim2Especificacao();
            winRelatorio win = new winRelatorio(rel, true, "Ajuda Pim 2.", false);
            win.ShowDialog(base.OwnerBase);
        }
    }
}
