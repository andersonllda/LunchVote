using HMV.PEP.WPF.Report;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Report.CentroObstetrico;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.BoletimDeEmergencia;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucSinaisVitais.xaml
    /// </summary>
    public partial class ucRelatorio : UserControlBase
    {
        public rptBoletimEmergenciaCO rpt;
        public ucRelatorio()
        {
            InitializeComponent();           
        }

        public override void SetData(ViewModelBase pVMObject)
        {
           vmRelatorioBoletimEmergenciaCO vm = new vmRelatorioBoletimEmergenciaCO((this.DataContext as vmBoletimEmergenciaCO).Boletim);
           rpt = new rptBoletimEmergenciaCO(vm);
           //winRelatorio rel = new winRelatorio(rpt, true, "Boletim de Emergência", !(this.DataContext as vmBoletimEmergenciaCO).boolImprimir);
           var uc = new ucReportBase(rpt, true, !(this.DataContext as vmBoletimEmergenciaCO).boolImprimir, true);
           this.gridPrincipal.Children.Clear();
           this.gridPrincipal.Children.Add(uc);
        }
    }
}
