using System.Collections.Generic;
using System.Windows;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.UTINEOFolhaP;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.UTINEOFolhaP;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;
using System;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winUTINEOFolhaParada.xaml
    /// </summary>
    public partial class winUTINEOFolhaParada : WindowBase
    {
        public winUTINEOFolhaParada(vmUTINEOFolhaParada pVm, bool pRel)
        {
            this.DataContext = pVm;
            if (!pRel)
                if (!(this.DataContext as vmUTINEOFolhaParada).VerificaDataNascimento())
                {
                    this.Close();
                    return;
                }
                else
                    (this.DataContext as vmUTINEOFolhaParada).AddNovaFolha(App.Usuario);

            InitializeComponent();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCalcular_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmUTINEOFolhaParada).Calcula();
            Relatorio();
        }

        public void Relatorio()
        {

            if ((DataContext as vmUTINEOFolhaParada).UTINEOFolhaparadaSelecionada.IsNull())
            {
                DXMessageBox.Show("Selecione uma Folha de Parada para visualizar!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var source = new List<wrpUTINEOFolhaParada>();
            source.Add((DataContext as vmUTINEOFolhaParada).UTINEOFolhaparadaSelecionada);

            rptUTINEOFolhaParada rpt = new rptUTINEOFolhaParada();
            rpt.DataSource = source; 

            rpt.lbPacienteNome.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.Nome;
            rpt.lbPacienteDataNasc.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.DataNascimento.HasValue ?
                                            (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.DataNascimento.Value.ToShortDateString() : string.Empty;
            rpt.lbPacienteDataFicha.Text = (DataContext as vmUTINEOFolhaParada).UTINEOFolhaparadaSelecionada.DataInclusao.ToShortDateString();
            rpt.lbPacientePeso.Text = (DataContext as vmUTINEOFolhaParada).UTINEOFolhaparadaSelecionada.Peso.ToString() + " kg";
            rpt.lbAtendimento.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.ID.ToString();
            rpt.lblIdade.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.Idade.ToString(2);
            rpt.lblProntuario.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.ID.ToString();

            rpt.lbAssinatura.Text = new wrpUsuarios(App.Usuario).AssinaturaPadrao(ComTratamento: true, NovaLinha: false) + Environment.NewLine + DateTime.Now.ToString(); 

            // Etiqueta
            if ((DataContext as vmUTINEOFolhaParada).Atendimento.IsNotNull())
            {
                if ((DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.IsNotNull())
                {
                    rpt.BindNomePaciente.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.Nome;
                    rpt.BindIDPaciente.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Paciente.ID.ToString();
                }

                rpt.BindNomeResumo.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Leito.IsNotNull() ? (DataContext as vmUTINEOFolhaParada).Atendimento.Leito.Descricao : string.Empty;

                if ((DataContext as vmUTINEOFolhaParada).Atendimento.Prestador.IsNotNull())
                {
                    rpt.BindNomePrestador.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Prestador.Nome;
                    rpt.BindRegistro.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.Prestador.Registro;
                }

                rpt.BindCodigoBarras.Text = (DataContext as vmUTINEOFolhaParada).Atendimento.ID.ToString();
            }
            else
                rpt.BindCodigoBarras.Visible = false;
            //                                               

            winRelatorio win = new winRelatorio(rpt, true, "Relatório UTINEO Folha Parada", false);
            win.ShowDialog(base.OwnerBase);
        }
    }
}
