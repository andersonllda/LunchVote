using System.Windows;
using System.Windows.Controls;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.UrgenciaP;
using HMV.PEP.WPF.Windows;
using HMV.PEP.WPF.Report;
using HMV.PEP.DTO;
using System.Text;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucUrgenciaPediatrica.xaml
    /// </summary>
    public partial class ucUrgenciaPediatrica : UserControlBase, IUserControl
    {
        public ucUrgenciaPediatrica() { }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winUrgenciaPediatrica win = new winUrgenciaPediatrica((vmUrgenciaPediatrica)this.DataContext);
            win.ShowDialog(base.OwnerBase);
        }             

        public void SetData(object pData)
        {
            this.DataContext = new vmUrgenciaPediatrica(pData as Atendimento);
            InitializeComponent();
        }

        public bool CancelClose { get; set; } 
         
        private void btnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            Relatorio();
        }

        private void Relatorio()
        {
            if (gdUrgenciaPediatrica.View.FocusedRow != null)
            {
                UrgenciaPediatricaDTO dto = new UrgenciaPediatricaDTO();
                rptUrgenciaPediatrica rpt = new rptUrgenciaPediatrica();

                rpt.srUrgenciaItens.ReportSource.DataSource = (DataContext as vmUrgenciaPediatrica).GetUrgenciaItensRel();

                // Etiqueta
                if ((DataContext as vmUrgenciaPediatrica).Atendimento.IsNotNull())
                {
                    if ((DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.IsNotNull())
                    {
                        rpt.BindNomePaciente.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.Nome;
                        rpt.BindIDPaciente.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.ID.ToString();
                    }

                    rpt.BindNomeResumo.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.Leito.IsNotNull() ? (DataContext as vmUrgenciaPediatrica).Atendimento.Leito.Descricao : string.Empty;

                    if ((DataContext as vmUrgenciaPediatrica).Atendimento.Prestador.IsNotNull())
                    {
                        rpt.BindNomePrestador.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.Prestador.Nome;
                        rpt.BindRegistro.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.Prestador.Registro;
                    }

                    rpt.BindCodigoBarras.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.ID.ToString();
                }
                else
                    rpt.BindCodigoBarras.Visible = false;
                //

                rpt.lbPacienteNome.Text = "[" + (DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.ID + "]" + (DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.Nome;
                rpt.lbPacienteDataNasc.Text = (DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.DataNascimento.HasValue ? (DataContext as vmUrgenciaPediatrica).Atendimento.Paciente.DataNascimento.Value.ToShortDateString() : string.Empty;
                rpt.lbPacienteIdade.Text = (DataContext as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.IdadeInclusao;
                rpt.lbPacienteDataFicha.Text = (DataContext as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DataInclusao.ToShortDateString();
                rpt.lbPacientePeso.Text = (DataContext as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.Peso.ToString() + " kg";
                rpt.lbPacienteSC.Text = (DataContext as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.SC.ToString() + " m²";

                rpt.lblTuboTraqueal.Text = (DataContext as vmUrgenciaPediatrica).GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.Tubotraqueal);
                rpt.lblLaminaLaringoscopio.Text = (DataContext as vmUrgenciaPediatrica).GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.LaminaLaringo);
                rpt.lblInsercao.Text = (DataContext as vmUrgenciaPediatrica).GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.Insercao);
                rpt.lblAMBU.Text = (DataContext as vmUrgenciaPediatrica).GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.AMBU);
                rpt.lblSondaSucccao.Text = (DataContext as vmUrgenciaPediatrica).GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.SondaSuccao);
                rpt.lblFluxoO2.Text = (DataContext as vmUrgenciaPediatrica).GetUrgenciaCabecalhoItens(vmUrgenciaPediatrica.CabecalhoItem.FluxoO2);
                winRelatorio win = new winRelatorio(rpt, true, "Relatório Urgência Pediátrica", false);
                win.ShowDialog(base.OwnerBase);
            }
        }
    }
}
