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
using HMV.PEP.WPF.Report.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.ProcessoEnfermagem.WPF.Base;
using HMV.PEP.WPF.Report.SumarioAvaliacaoMedicaCTINEO;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaCTINEO;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoMedicaCTINEO
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

        public rptSumarioAvaliacaoMedicaCTINEO rpt;
        protected override void SetEvents()
        {
            base.SetEvents();

            rpt = new rptSumarioAvaliacaoMedicaCTINEO(new vmRelatorioSumarioAvaliacaoMedicaCTINEO((DataContext as vmSumarioAvaliacaoMedicaCTINEO).SumarioAvaliacaoMedicaCTINEO));

            var uc = new ucReportBase(rpt, (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).MostraRelatorioFinalizado, (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).MostraMarcaDaguaRelatorio, (this.DataContext as vmSumarioAvaliacaoMedicaCTINEO).MostraRelatorioFinalizado);
            while (gridPrincipal.Children.Count > 0)
                this.gridPrincipal.Children.RemoveAt(0);
            this.gridPrincipal.Children.Add(uc);
        }  
    }
}
