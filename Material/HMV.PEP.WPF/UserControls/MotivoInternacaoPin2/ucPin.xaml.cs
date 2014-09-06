using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.Core.Domain.Model;
using DevExpress.Xpf.Grid;
using HMV.PEP.WPF.Report.Pim2;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Windows.MotivoInternacaoPin2;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.MotivoInternacaoPin2
{   
    public partial class ucPin : UserControlBase, IUserControl
    {
        public ucPin()
        {
            InitializeComponent(); 
        }
        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                this.DataContext = new vmMotivoInternacaoPim2((pData as Atendimento), App.Usuario,true);
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

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {          
            winCadMotivoInternacaoPin2 winCadPin = new winCadMotivoInternacaoPin2(this.DataContext as vmMotivoInternacaoPim2,1);
            (this.DataContext as vmMotivoInternacaoPim2).vmPin2.NovoPIN2();   
            winCadPin.Show();
        }

        private void gdPimConsulta_MouseDown(object sender, MouseButtonEventArgs e)
        {
            (sender as GridControl).View.FocusedRow = (sender as GridControl).View.FocusedRow;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            rptPim2 pim = new rptPim2();
            (this.DataContext as vmMotivoInternacaoPim2).ImprimiuORelatorio();
            pim.sCabecalho.ReportSource.DataSource = (this.DataContext as vmMotivoInternacaoPim2).vmPin2.RelCabecalho;
            pim.sListaValores.ReportSource.DataSource = (this.DataContext as vmMotivoInternacaoPim2).vmPin2.RelListaValores;
            pim.sMotivoInternacao.ReportSource.DataSource = (this.DataContext as vmMotivoInternacaoPim2).vmMotivoDeInternacao.RelMotivoInternacao;
            winRelatorio win = new winRelatorio(pim, true, "Pediatria", false);
            win.ShowDialog(base.OwnerBase);
        }

        public void RefreshMotivoInternacao()
        {
            (this.DataContext as vmMotivoInternacaoPim2).RefreshMotivoInternacao();
        }
    }
}
