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
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaEndoscopia
{
    /// <summary>
    /// Interaction logic for ucVisualizar.xaml
    /// </summary>
    public partial class ucVisualizar : UserControlBase
    {
        public rptSumarioAvaliacaoMedicaEndoscopia rpt;
        public ucVisualizar()
        {
            InitializeComponent();
        }

        protected override void SetEvents()
        {
            base.SetEvents();

            //rpt = new rptSumarioAvaliacaoMedicaEndoscopia(this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia);

            rpt = new rptSumarioAvaliacaoMedicaEndoscopia(new vmRelatorioSumarioAvaliacaoMedicaEndoscopia((DataContext as vmSumarioAvaliacaoMedicaEndoscopia)));

            var uc = new ucReportBase(rpt, (this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).MostraRelatorioFinalizado
                                        , (this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).MostraMarcaDaguaRelatorio
                                        , (this.DataContext as vmSumarioAvaliacaoMedicaEndoscopia).MostraRelatorioFinalizado);
            while (gridPrincipal.Children.Count > 0)
                this.gridPrincipal.Children.RemoveAt(0);
            this.gridPrincipal.Children.Add(uc);
        }       
    }
}
