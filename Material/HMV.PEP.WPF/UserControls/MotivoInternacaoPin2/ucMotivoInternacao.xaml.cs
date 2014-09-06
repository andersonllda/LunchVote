using System;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using HMV.PEP.WPF.Windows.MotivoInternacaoPin2;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Report.Pim2;
using HMV.PEP.WPF.Report;
using System.Windows;

namespace HMV.PEP.WPF.UserControls.MotivoInternacaoPin2
{
    /// <summary>
    /// Interaction logic for ucMotivoInternacao.xaml
    /// </summary>
    public partial class ucMotivoInternacao : UserControlBase, IUserControl
    {
        public ucMotivoInternacao()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                this.DataContext = new vmMotivoInternacaoPim2((pData as Atendimento), App.Usuario,false);               
            }
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private void btnIncluir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winCadMotivoInternacaoPin2 winCadPin = new winCadMotivoInternacaoPin2(this.DataContext as vmMotivoInternacaoPim2,0);
            winCadPin.ShowDialog(Window.GetWindow(this));
        }

        public void RefreshMotivoInternacao()
        {
            (this.DataContext as vmMotivoInternacaoPim2).RefreshMotivoInternacao();
        }

        private void btnImprimir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            rptPim2 pim = new rptPim2();
            (this.DataContext as vmMotivoInternacaoPim2).ImprimiuORelatorio();
            pim.sCabecalho.ReportSource.DataSource = (this.DataContext as vmMotivoInternacaoPim2).vmPin2.RelCabecalho;
            pim.sListaValores.ReportSource.DataSource = (this.DataContext as vmMotivoInternacaoPim2).vmPin2.RelListaValores;
            pim.sMotivoInternacao.ReportSource.DataSource = (this.DataContext as vmMotivoInternacaoPim2).vmMotivoDeInternacao.RelMotivoInternacao;
            pim.sMotivoInternacao.Visible = true;
            if (pim.sMotivoInternacao.ReportSource.DataSource == null)
                pim.sMotivoInternacao.Visible = false;
            winRelatorio win = new winRelatorio(pim, true, "Motivo de Internação / Pim 2.", false);
            win.ShowDialog(base.OwnerBase);
        }
    }
}
