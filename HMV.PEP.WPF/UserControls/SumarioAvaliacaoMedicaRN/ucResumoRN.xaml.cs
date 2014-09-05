using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.WPF.Report.SumarioAvaliacaoMedicaRN;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaRN
{
    /// <summary>
    /// Interaction logic for ucResumoRN.xaml
    /// </summary>
    public partial class ucResumoRN : UserControlBase
    {
        public ucResumoRN()
        {
            InitializeComponent();
        }

        public rptSumarioAvaliacaoMedicaRN rpt;
        private vmRelatorioSumarioAvaliacaoMedicaRN rel;
        protected override void SetEvents()
        {
            base.SetEvents();

            rel = new vmRelatorioSumarioAvaliacaoMedicaRN((this.DataContext as vmSumarioAvaliacaoMedicaRN));
            rpt = new rptSumarioAvaliacaoMedicaRN(rel);
            
            var uc = new ucReportBase(rpt, (this.DataContext as vmSumarioAvaliacaoMedicaRN).MostraRelatorioFinalizado, (this.DataContext as vmSumarioAvaliacaoMedicaRN).MostraMarcaDaguaRelatorio
                , (this.DataContext as vmSumarioAvaliacaoMedicaRN).MostraRelatorioFinalizado);
            
            this.gridPrincipal.Children.Clear();
            this.gridPrincipal.Children.Add(uc);                      
        }

        public void Clear()
        {
            if (rel != null && rpt != null)
            {                
                rel = null;
                rpt = null;
            }
        }
    }
}
