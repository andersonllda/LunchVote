using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    /// <summary>
    /// Interaction logic for ucResumo.xaml
    /// </summary>
    public partial class ucResumo : UserControlBase
    {
        public ucResumo()
        {
            InitializeComponent();
        }

        public rptSumarioAvaliacaoMedicaCO rpt { get; private set; }
        protected override void SetEvents()
        {
            base.SetEvents();

            rpt = new rptSumarioAvaliacaoMedicaCO(new vmRelatorioSumarioAvaliacaoMedicaCO((this.DataContext as vmSumarioAvaliacaoMedicaCO).SumarioAvaliacaoMedicaCO));

            var uc = new ucReportBase(rpt, (this.DataContext as vmSumarioAvaliacaoMedicaCO).MostraRelatorioFinalizado, (this.DataContext as vmSumarioAvaliacaoMedicaCO).MostraMarcaDaguaRelatorio, (this.DataContext as vmSumarioAvaliacaoMedicaCO).MostraRelatorioFinalizado);
            while (gridPrincipal.Children.Count > 0)
                this.gridPrincipal.Children.RemoveAt(0);
            this.gridPrincipal.Children.Add(uc);
        }

        //private void HMVButton_Click(object sender, RoutedEventArgs e)
        //{
        //    vmRelatorioSumarioAvaliacaoMedicaCO vm = new vmRelatorioSumarioAvaliacaoMedicaCO((this.DataContext as vmSumarioAvaliacaoMedicaCO).SumarioAvaliacaoMedicaCO);
        //    rptSumarioAvaliacaoMedicaCO rpt = new rptSumarioAvaliacaoMedicaCO(vm);
        //    winRelatorio rel = new winRelatorio(rpt, false, "Sumario de Avaliação Médica CO", !(this.DataContext as vmSumarioAvaliacaoMedicaCO).boolImprimir);
        //    rel.ShowDialog(base.OwnerBase);
        //}
    }
}
