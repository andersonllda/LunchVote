using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.Drawing;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.WPF.Report;
using DevExpress.XtraReports.UI;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucResumoAltaMedica.xaml
    /// </summary>
    public partial class ucResumoAltaMedica : UserControl, IUserControl
    {
        //bool tranferencia;
        bool temalta;
        public rptResumoAltaMedica report = new rptResumoAltaMedica();
        vmSumarioAltaRelatorio vmSAR;

        public bool CancelClose { get; set; }

        public void Print()
        {
            report.Imprime();
        }

        public void SetData(Object pData)
        {
            temalta = (pData as vmSumarioAlta).JaTemAlta;
            //tranferencia = (pData as vmSumarioAlta).HabilitaAbaTransferencia;
            //if (vmSAR == null || vmSAR.TipoAtendimento == null)
            vmSAR = new vmSumarioAltaRelatorio((pData as vmSumarioAlta).SumarioAlta
                , (pData as vmSumarioAlta).vmDadosNascimento.IsNotNull() ? (pData as vmSumarioAlta).vmDadosNascimento.DadosNascimento : null
                , App.Usuario, (pData as vmSumarioAlta).HabilitaTransfRelatorio);

            //Report();

            //SubReports();

            

            //List<vmSumarioAltaRelatorio> l = new List<vmSumarioAltaRelatorio>();
            //l.Add(vmSAR);
            //report.DataSource = l;
            report.DataSource = vmSAR.ListaSumarioAlta;
            //
            if (vmSAR.DataAlta.IsEmpty())
            {
                RelRichText.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                barManager.CreateStandardLayout = false;
            }
            else
            {
                RelRichText.Margin = new System.Windows.Thickness(0, 26, 0, 0);
                barManager.CreateStandardLayout = true;
            }         

            //montaRTF();
            XtraReportPreviewModel model = new XtraReportPreviewModel(report);
            rSumarioAltaMedica.Model = model;
            report.CreateDocument(false);            
        }

        public ucResumoAltaMedica()
        {
            InitializeComponent();
        }

        //private void SubReports()
        //{
        //    report.srMedicosAssistente.ReportSource.DataSource = vmSAR.MedicoAssistenteCRMCRO;

        //    if (vmSAR.IsRN)
        //    {
        //        report.lbTitulo.Text = "Sumário de Alta do Recém-Nascido";
        //        report.lbTituloRodape.Text = "Sumário de Alta do Recém-Nascido";
        //    }

        //    #region ---------- CIDs ----------

        //    report.lbDiagnosticoCidPrincipal.Text = vmSAR.CidPrincipal;

        //    report.DetailDiagnostico.Visible = true;
        //    report.DetailDiagnostico2.Visible = true;

        //    if (vmSAR.Cids != null)
        //        report.srDiagnosticos.ReportSource.DataSource = vmSAR.Cids;
        //    else
        //    {
        //        report.DetailDiagnostico2.Visible = false;
        //    }

        //    vmSAR.VisibleDiagnosticos = report.DetailDiagnostico.Visible;

        //    #endregion

        //    #region ---------- Condicao alta ----------

        //    report.DetailCondicaoAlta.Visible = true;

        //    if (!vmSAR.Consulta.IsEmpty())
        //    {
        //        report.lbConsultaMedica.Text = vmSAR.Consulta;
        //        report.DetailConsultaMedica.Visible = true;
        //    }
        //    else
        //        report.DetailConsultaMedica.Visible = false;

        //    if (!vmSAR.CondicaoAlta.IsEmpty())
        //        report.DetailCondicaoAlta.Visible = true;
        //    else
        //        report.DetailCondicaoAlta.Visible = false;

        //    vmSAR.VisibleCondicaoAlta = report.DetailCondicaoAlta.Visible;

        //    #endregion

        //    #region ---------- Evolução ----------

        //    report.DetailEvolucao.Visible = true;
        //    if (!string.IsNullOrEmpty(vmSAR.Evolucao))
        //        report.lbEvolucao.Text = vmSAR.Evolucao;
        //    else
        //        report.DetailEvolucao.Visible = false;

        //    vmSAR.VisibleEvolucao = report.DetailEvolucao.Visible;

        //    #endregion

        //    #region ---------- Causa Externa ----------

        //    if (vmSAR.IsInternado && !vmSAR.IsRN)
        //    {
        //        report.DetailCausaExterna.Visible = true;

        //        if (vmSAR.CausaExterna == null)
        //        {
        //            report.DetailCausaExterna.Visible = false;
        //        }
        //        else if (vmSAR.CausaExterna[0].Descricao == "Não se Aplica")
        //        {
        //            report.PanelCausaExterna.Visible = true;
        //            report.srCausaExterna.Visible = false;
        //        }
        //        else
        //        {
        //            report.srCausaExterna.Visible = true;
        //            report.PanelCausaExterna.Visible = false;
        //            report.srCausaExterna.ReportSource.DataSource = vmSAR.CausaExterna;
        //        }
        //    }
        //    else
        //        report.DetailCausaExterna.Visible = false;

        //    vmSAR.VisibleCausaExterna = report.DetailCausaExterna.Visible;

        //    #endregion

        //    #region ---------- Dados Nascimento ----------

        //    report.DetailDadosNascimento.Visible = false;
        //    if (vmSAR.IsRN)
        //    {
        //        vmSAR.VisibleDadosNascimento = false;
        //        if (vmSAR.DadosDoNascimento.IsNotNull())
        //        {
        //            if (vmSAR.DadosDoNascimento.MostraDadosNascimento)
        //            {
        //                List<vmSumarioAltaRelatorio.DadosNascimento> dados = new List<vmSumarioAltaRelatorio.DadosNascimento>();
        //                dados.Add(vmSAR.DadosDoNascimento);
        //                report.srDadosNascimento.ReportSource.DataSource = dados;
        //                vmSAR.VisibleDadosNascimento = true;
        //                report.DetailDadosNascimento.Visible = true;
        //            }
        //        }
        //    }

        //    #endregion

        //    #region ---------- Procedimentos ----------

        //    report.DetailProcedimentos.Visible = true;
        //    report.srProcedimentos.Visible = true;
        //    if (vmSAR.Procedimentos == null)
        //    {
        //        report.DetailProcedimentos.Visible = false;
        //        report.srProcedimentos.Visible = false;
        //    }
        //    else if (vmSAR.Procedimentos[0].ProcedimentoNome == "Não se Aplica")
        //    {
        //        report.PanelProcedimentos.Visible = true;
        //        report.srProcedimentos.Visible = false;
        //    }
        //    else if (vmSAR.Procedimentos != null)
        //    {
        //        report.PanelProcedimentos.Visible = false;
        //        report.srProcedimentos.Visible = true;
        //        report.srProcedimentos.ReportSource.DataSource = vmSAR.Procedimentos;
        //    }
        //    vmSAR.VisibleProcedimentos = report.DetailProcedimentos.Visible || report.srProcedimentos.Visible;

        //    #endregion

        //    #region ---------- Farmacos ----------

        //    report.DetailFarmaco.Visible = true;
        //    if (vmSAR.FarmacosInternacao == null)
        //        report.DetailFarmaco.Visible = false;
        //    else if (vmSAR.FarmacosInternacao[0].Descricao == "Não se Aplica")
        //    {
        //        report.PanelFarmaco.Visible = true;
        //        report.srFarmacos.Visible = false;
        //    }
        //    else
        //    {
        //        report.PanelFarmaco.Visible = false;
        //        report.srFarmacos.Visible = true;
        //        report.srFarmacos.ReportSource.DataSource = vmSAR.FarmacosInternacao;
        //    }

        //    report.rfFarmacosObservacao.Visible = true;
        //    if (vmSAR.ObservacaoFarmacos == null)
        //        report.rfFarmacosObservacao.Visible = false;
        //    else
        //        report.lbObservacaoFarmaco.Text = vmSAR.ObservacaoFarmacos;

        //    vmSAR.VisibleFarmaco = report.DetailFarmaco.Visible || report.srFarmacos.Visible;

        //    #endregion

        //    #region ---------- Exames ------------

        //    if (vmSAR.IsInternado)
        //    {
        //        report.DetailExame.Visible = true;

        //        if (vmSAR.Exames.Count() == 0)
        //        {
        //            report.DetailExame.Visible = false;
        //        }
        //        else if (vmSAR.Exames[0].Observacao == "Exames sem particularidades" || vmSAR.Exames[0].Observacao == "Não foram realizados exames durante a internação")
        //        {
        //            report.PanelExame.Visible = true;
        //            report.srExame.Visible = false;
        //            report.PanelExameTexto.Text = vmSAR.Exames[0].Observacao;
        //        }
        //        else if (vmSAR.Exames[0].Observacao.Contains("Exames sem particularidades\r\n") || vmSAR.Exames[0].Observacao.Contains("Não foram realizados exames durante a internação\r\n"))
        //        {
        //            report.PanelExame.Visible = true;
        //            report.srExame.Visible = false;
        //            report.PanelExameTexto.Text = vmSAR.Exames[0].Observacao;
        //        }
        //        else if (vmSAR.Exames[0].Observacao.StartsWith("SO_"))
        //        {
        //            report.PanelExame.Visible = true;
        //            report.srExame.Visible = false;
        //            report.PanelExameTexto.Text = vmSAR.Exames[0].Observacao.Mid(3);
        //        }
        //        else
        //        {
        //            report.PanelExame.Visible = false;
        //            report.srExame.Visible = true;
        //            report.srExame.ReportSource.DataSource = vmSAR.Exames;

        //            if (vmSAR.Exames[0].Observacao.IsEmptyOrWhiteSpace())
        //            {
        //                report.MostraObservacaoExames = false;
        //            }
        //            else
        //                report.MostraObservacaoExames = true;
        //        }
        //    }
        //    else report.DetailExame.Visible = false;

        //    vmSAR.VisibleExame = report.DetailExame.Visible || report.srExame.Visible;

        //    #endregion

        //    #region ---------- Recomendação ----------

        //    report.DetailPlanoAlta2.Visible = true;
        //    if (vmSAR.Recomendacao == null)
        //    {
        //        report.DetailPlanoAlta2.Visible = false;
        //    }
        //    else if (vmSAR.Recomendacao[0].Descricao == "Sem Recomendações")
        //    {
        //        report.PanelRecomendacao.Visible = true;
        //        report.srRecomendacao.Visible = false;
        //    }
        //    else
        //    {
        //        report.srRecomendacao.Visible = true;
        //        report.PanelRecomendacao.Visible = false;
        //        report.srRecomendacao.ReportSource.DataSource = vmSAR.Recomendacao;
        //    }

        //    vmSAR.VisiblePlanoAlta2 = report.DetailPlanoAlta2.Visible;

        //    #endregion

        //    #region ---------- Medicamentos Pos Alta ----------

        //    if (vmSAR.IsObito)
        //    {
        //        report.DetailMedicamentosOutros.Visible = false;
        //        report.DetailPlanoAlta.Visible = false;
        //    }

        //    else if (vmSAR.IsInternado)
        //    {
        //        report.DetailMedicamentosOutros.Visible = false;
        //        report.DetailPlanoAlta.Visible = true;
        //        if (vmSAR.MedicamentoPosAlta == null)
        //        {
        //            report.DetailPlanoAlta.Visible = false;
        //        }
        //        else if (vmSAR.MedicamentoPosAlta[0].Nome == "Não se Aplica")
        //        {
        //            report.PanelPosAlta.Visible = true;
        //            report.srMedicamentos.Visible = false;
        //        }
        //        else
        //        {
        //            report.PanelPosAlta.Visible = false;
        //            report.srMedicamentos.Visible = true;
        //            report.srMedicamentos.ReportSource.DataSource = vmSAR.MedicamentoPosAlta;
        //        }
        //    }
        //    else
        //    {
        //        report.DetailPlanoAlta.Visible = false;
        //        report.DetailMedicamentosOutros.Visible = true;
        //        if (vmSAR.MedicamentoPosAlta == null)
        //        {
        //            report.DetailMedicamentosOutros.Visible = false;
        //        }
        //        else if (vmSAR.MedicamentoPosAlta[0].Nome == "Não se Aplica")
        //        {
        //            report.PanelMedicamentosSemPres.Visible = true;
        //            report.srMedicamentosOutros.Visible = false;
        //        }
        //        else
        //        {
        //            report.PanelMedicamentosSemPres.Visible = false;
        //            report.srMedicamentosOutros.Visible = true;
        //            report.srMedicamentosOutros.ReportSource.DataSource = vmSAR.MedicamentoPosAlta;
        //        }
        //    }
        //    vmSAR.VisibleMedPosAlta = report.DetailMedicamentosOutros.Visible;
        //    vmSAR.VisiblePlanoAlta = report.DetailPlanoAlta.Visible;

        //    #endregion

        //    vmSAR.SetaIndex();
        //}

        //private void Report()
        //{
        //    this.report.BindNomePaciente.Text = vmSAR.NomePaciente;
        //    this.report.BindNomeResumo.Text = vmSAR.NomeResumo;
        //    this.report.BindIDPaciente.Text = vmSAR.IDPaciente;
        //    this.report.BindNomePrestador.Text = vmSAR.NomePrestador;
        //    this.report.BindRegistro.Text = vmSAR.Registro;
        //    this.report.BindCodigoBarras.Text = vmSAR.CodigoBarras;
        //    this.report.BindCodigoBarras.Visible = vmSAR.MostraCodigoBarras;

        //    if (!vmSAR.IsInternado)
        //    {
        //        report.panelPlanoAtendOutros.Visible = true;
        //        report.DetailDtHrSaida.Visible = true;
        //        report.PanelDtHoraResponsavel.Visible = true;
        //        report.PanelInternacao.Visible = false;
        //        report.lbDataInternacao.Text = "Data Atendimento";
        //    }

        //    report.lbTitulo.Text = vmSAR.TipoAtendimento;
        //    report.lbTituloRodape.Text = vmSAR.TipoAtendimento;
        //    report.labelDR.Text = vmSAR.DR;

        //    if (!vmSAR.CasoUrgencia.IsEmpty())
        //    {
        //        report.lbCasoUrgencia.Text = vmSAR.CasoUrgencia;
        //        report.DetailUrgencia.Visible = true;
        //    }
        //    else
        //        report.DetailUrgencia.Visible = false;

        //    report.lbPacienteSexo.Text = vmSAR.PacienteSexo.ToString();
        //    report.lbPacienteNome.Text = vmSAR.PacienteNome;
        //    report.lbPacienteIdade.Text = vmSAR.PacienteIdade;

        //    if (!vmSAR.CondicaoInternacao.IsEmpty())
        //        report.lblCondicaoMotivoInternacao.Text = vmSAR.CondicaoInternacao;
        //    else
        //        report.DetailMotivoInternacao.Visible = false;

        //    vmSAR.VisibleMotivoInternacao = report.DetailMotivoInternacao.Visible;

        //    report.DetailCondicaoAlta.Visible = true;

        //    if (!vmSAR.CondicaoAlta.IsEmpty())
        //        report.lbCondicoesAlta.Text = vmSAR.CondicaoAlta;
        //    else
        //        report.DetailCondicaoAlta.Visible = false;

        //    vmSAR.VisibleCondicaoAlta = report.DetailCondicaoAlta.Visible;

        //    report.lbPacienteRegistro.Text = vmSAR.PacienteId.ToString();
        //    report.lbPacienteDataNasc.Text = vmSAR.PacienteDataNascimento.ToString();

        //    if (!vmSAR.IsInternado)
        //        report.lbPacienteDataAlta.Visible = false;
        //    else
        //        report.lbPacienteDataAlta.Text = vmSAR.DataAlta;

        //    report.Watermark.Text = "";

        //    if (!temalta)
        //    {
        //        report.Watermark.Text = "Documento Incompleto";
        //        report.Watermark.TextDirection = DirectionMode.ForwardDiagonal;
        //        report.Watermark.Font = new Font(report.Watermark.Font.FontFamily, 48);
        //        report.Watermark.ForeColor = Color.Silver;
        //        report.Watermark.TextTransparency = 150;
        //        report.Watermark.ShowBehind = false;
        //        report.Watermark.PageRange = "1,1-5";
        //    }
        //    report.lbPacienteHoraAlta.Text = vmSAR.HoraAlta;
        //    report.lbPacientePermanencia.Text = vmSAR.Permanencia;
        //    report.lbPacienteAtendimento.Text = vmSAR.Atendimento.ToString();
        //    report.lbPacienteDataInternacao.Text = vmSAR.DataInternacao;

        //    try
        //    {
        //        //Transferencia
        //        if (!tranferencia)
        //        {
        //            report.DetailTransferencia.Visible = false;
        //        }
        //        else
        //        {
        //            report.lbTitulo.Text = "Sumário de Transferência";
        //            report.lbTituloRodape.Text = "Sumário de Transferência";
        //            report.DetailTransferencia.Visible = true;
        //            report.lbTransAcompMedico.Text = vmSAR.AcompanhamentoMedico;
        //            report.lbTransContatoInstitu.Text = vmSAR.ContatoInstituicao;
        //            report.lbTransDestino.Text = vmSAR.Destino;
        //            report.lbTransFC.Text = vmSAR.FC;
        //            report.lbTransFR.Text = vmSAR.FR;
        //            report.lbTransHospitalDestino.Text = vmSAR.HospitalDestinor;
        //            report.lbTransMomentoTrans.Text = vmSAR.MomentoTransferencia;
        //            report.lbTransMotorizacao.Text = vmSAR.Monitorizacao;
        //            report.lbTransMunicipio.Text = vmSAR.Municipio;
        //            report.lbTransOxigenio.Text = vmSAR.Oxgenio;
        //            report.lbTransPA.Text = vmSAR.PA;
        //            report.lbTransReservaLeito.Text = vmSAR.QuemReservolLeito;
        //            report.lbTransTransporte.Text = vmSAR.Transporte;
        //            report.lbTransVentilacao.Text = vmSAR.Ventilacao;
        //        }
        //    }
        //    catch (Exception ex) { DXMessageBox.Show("Erro Transferencia Paciente" + ex.ToString(), "Relatorio:"); }
        //}

        //public void montaRTF()
        //{
        //    //RTFHelper rtfAlta = new RTFHelper();
        //    //this.RelRichText.Document = rtfAlta.FlowDoc();

        //    //#region Identificacao
        //    //rtfAlta.criaTitulo("Identificação");
        //    //rtfAlta.criaTexto((lstIdentificacao[0].Nome.IsEmptyOrWhiteSpace() ? string.Empty : "Nome: " + lstIdentificacao[0].Nome.ToString()) +
        //    //                  (lstIdentificacao[0].Nascimento.IsEmptyOrWhiteSpace() ? string.Empty : "   Data Nascimento: " + lstIdentificacao[0].Nascimento.ToString()) +
        //    //                  (lstIdentificacao[0].Sexo.IsEmptyOrWhiteSpace() ? string.Empty : "   Sexo: " + lstIdentificacao[0].Sexo.ToString()) +
        //    //                  (lstIdentificacao[0].Registro.IsEmptyOrWhiteSpace() ? string.Empty : "   Registro: " + lstIdentificacao[0].Registro.ToString()) +
        //    //                  (lstIdentificacao[0].Atendimento.IsEmptyOrWhiteSpace() ? string.Empty : "   Atendimento: " + lstIdentificacao[0].Atendimento.ToString()));
        //    //rtfAlta.criaTexto((lstIdentificacao[0].Idade.IsEmptyOrWhiteSpace() ? string.Empty : "Idade: " + lstIdentificacao[0].Idade.ToString()) +
        //    //                  (lstIdentificacao[0].DataInternacao.IsEmptyOrWhiteSpace() ? string.Empty : "   Data Internação: " + lstIdentificacao[0].DataInternacao.ToString()) +
        //    //                  (lstIdentificacao[0].DataAlta.IsEmptyOrWhiteSpace() ? string.Empty : "   Data Alta: " + lstIdentificacao[0].DataAlta.ToString()) +
        //    //                  (lstIdentificacao[0].HoraAlta.IsEmptyOrWhiteSpace() ? string.Empty : "   Hora Alta: " + lstIdentificacao[0].HoraAlta.ToString()) +
        //    //                  (lstIdentificacao[0].Permanencia.IsEmptyOrWhiteSpace() ? string.Empty : "   Permanência: " + lstIdentificacao[0].Permanencia.ToString()));



        //    //#endregion

        //    //#region Médico Assistente
        //    //if (lstMedicoAssistente.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Médico Assistente");
        //    //    rtfAlta.criaTexto(lstMedicoAssistente[0].Medico.ToString());
        //    //}
        //    //#endregion

        //    //#region Motivos de Internação
        //    //if (lstMotivoInternacao.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Motivo de Internação");
        //    //    rtfAlta.criaTexto(lstMotivoInternacao[0].Descricao.ToString());
        //    //}
        //    //#endregion

        //    //#region Condicoes de Alta
        //    //if (lstCondicoesDeAlta.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Condições de Alta");
        //    //    rtfAlta.criaTexto(lstCondicoesDeAlta[0].Descricao.ToString());
        //    //}
        //    //#endregion

        //    //#region Diagnosticos
        //    //if (lstDiagnosticos.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Diagnósticos");
        //    //    if (!lstDiagnosticos[0].CidPrincipal.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("CID Principal");
        //    //        rtfAlta.criaTexto(lstDiagnosticos[0].CidPrincipal.ToString());
        //    //    }
        //    //    if (!lstDiagnosticos[0].OutrosCids.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Outros CIDs");
        //    //        rtfAlta.criaTexto(lstDiagnosticos[0].OutrosCids.ToString());
        //    //    }
        //    //}
        //    //#endregion

        //    //#region Causa Externa
        //    //if (lstCausaExterna.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Causa Externa");
        //    //    rtfAlta.criaTexto(lstCausaExterna[0].Descricao.ToString());
        //    //}
        //    //#endregion

        //    //#region Dados do Nascimento
        //    ////if (vmSAR.DadosDoNascimento.IsNotNull())
        //    ////{
        //    ////    rtfAlta.criaLinhaEmBranco();
        //    ////    rtfAlta.criaTitulo("Dados do Nascimento");
        //    ////    //rtfAlta.criaTexto(lstCausaExterna[0].Descricao.ToString());
        //    ////}
        //    //#endregion

        //    //#region Procedimentos
        //    //if (lstProcedimentos.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Procedimentos");
        //    //    rtfAlta.criaTexto(lstProcedimentos[0].Procedimento.ToString());
        //    //}
        //    //#endregion

        //    //#region Principais Farmacos
        //    //if (lstPrincipaisFarmacos.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Principais Fármacos na Internação");
        //    //    if (!lstPrincipaisFarmacos[0].Farmacos.IsEmptyOrWhiteSpace())
        //    //        rtfAlta.criaTexto(lstPrincipaisFarmacos[0].Farmacos.ToString());
        //    //    if (!lstPrincipaisFarmacos[0].Observacao.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Observação");
        //    //        rtfAlta.criaTexto(lstPrincipaisFarmacos[0].Observacao.ToString());
        //    //    }
        //    //}
        //    //#endregion

        //    //#region Evolucao
        //    //if (lstEvolucao.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Evolução");
        //    //    rtfAlta.criaTexto(lstEvolucao[0].Evolucao.ToString());
        //    //}
        //    //#endregion

        //    //#region Exames Diagnosticos
        //    //if (lstExamesDiagnosticos.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Resultados Principais de Exames Diagnósticos");

        //    //    if (lstExamesDiagnosticos.FirstOrDefault().Exames.IsNotEmptyOrWhiteSpace())
        //    //    //if (!lstExamesDiagnosticos[0].Exames.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaTexto(lstExamesDiagnosticos.FirstOrDefault().Exames.ToString());
        //    //    }
        //    //    if (lstExamesDiagnosticos.FirstOrDefault().AnalisesClinicas.IsNotEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Análises Clínicas");
        //    //        rtfAlta.criaTexto(lstExamesDiagnosticos.FirstOrDefault().AnalisesClinicas.ToString());
        //    //    }
        //    //    if (lstExamesDiagnosticos.FirstOrDefault().Observacoes.IsNotEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Observações");
        //    //        rtfAlta.criaTexto(lstExamesDiagnosticos.FirstOrDefault().Observacoes.ToString());
        //    //    }
        //    //}
        //    //#endregion

        //    //#region Plano Pos-Alta
        //    ////Medicamentos
        //    //if (lstPlanoPosAlta.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTitulo("Plano Pós-Alta");
        //    //    rtfAlta.criaSubTituloSublinhado("Medicamentos");
        //    //    rtfAlta.criaTexto(lstPlanoPosAlta[0].Medicamentos.ToString());
        //    //}

        //    ////Recomendacoes
        //    //if (lstRecomendacoes.Count > 0)
        //    //{
        //    //    if (!lstRecomendacoes[0].Recomendacao.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Recomendações ao Paciente/Responsável");
        //    //        rtfAlta.criaTexto(lstRecomendacoes[0].Recomendacao.ToString());
        //    //    }
        //    //    if (!lstRecomendacoes[0].Consulta.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Consulta Médica");
        //    //        rtfAlta.criaTexto(lstRecomendacoes[0].Consulta.ToString());
        //    //    }
        //    //    if (!lstRecomendacoes[0].Urgencia.IsEmptyOrWhiteSpace())
        //    //    {
        //    //        rtfAlta.criaSubTituloSublinhado("Caso de Urgência");
        //    //        rtfAlta.criaTexto(lstRecomendacoes[0].Urgencia.ToString());
        //    //    }
        //    //}
        //    //#endregion

        //    //#region Assinatura
        //    //if (lstAssinatura.Count > 0)
        //    //{
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaLinhaEmBranco();
        //    //    rtfAlta.criaTexto(lstAssinatura[0].Assinatura.ToString());
        //    //}
        //    //#endregion
        //}

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            report.Imprime();
        }

        #region RelatorioEmRTF

        //Identificacao
        public class rIdentificacao
        {
            public string Nome { get; set; }
            public string Nascimento { get; set; }
            public string Sexo { get; set; }
            public string Registro { get; set; }
            public string Atendimento { get; set; }
            public string Idade { get; set; }
            public string DataInternacao { get; set; }
            public string DataAlta { get; set; }
            public string HoraAlta { get; set; }
            public string Permanencia { get; set; }
        }
        public List<rIdentificacao> lstIdentificacao
        {
            get
            {
                List<rIdentificacao> qry = new List<rIdentificacao>();

                //qry.Add(new rIdentificacao()
                //        {
                //            Nome = vmSAR.PacienteNome.ToString(),
                //            Nascimento = "" + vmSAR.PacienteDataNascimento.ToString(),
                //            Sexo = "" + vmSAR.PacienteSexo.ToString(),
                //            Registro = "" + vmSAR.PacienteId.ToString(),
                //            Atendimento = "" + vmSAR.Atendimento.ToString(),
                //            Idade = "" + vmSAR.PacienteIdade.ToString(),
                //            DataInternacao = "" + vmSAR.DataInternacao.ToString(),
                //            DataAlta = "" + vmSAR.DataAlta.ToString(),
                //            HoraAlta = "" + vmSAR.HoraAlta.ToString(),
                //            Permanencia = "" + vmSAR.Permanencia.ToString()
                //        });

                return qry;
            }
        }

        //Medico Assistente
        public class rMedicoAssistente
        {
            public string Medico { get; set; }
        }
        public List<rMedicoAssistente> lstMedicoAssistente
        {
            get
            {
                //string _Itens = string.Empty;
                //if (vmSAR.MedicoAssistenteCRMCRO.HasItems())
                //{
                //    foreach (var item in vmSAR.MedicoAssistenteCRMCRO)
                //    {
                //        if (!_Itens.IsEmptyOrWhiteSpace())
                //            _Itens += Environment.NewLine;
                //        _Itens += ((item.CRM.IsEmptyOrWhiteSpace()) ? string.Empty : "CRM: " + item.CRM.ToString()) +
                //            "    " + ((item.Nome.IsEmptyOrWhiteSpace()) ? string.Empty : "Nome: " + item.Nome.ToString());
                //    }
                //}
                List<rMedicoAssistente> qry = new List<rMedicoAssistente>();
                //try
                //{
                //    if (vmSAR.MedicoAssistenteCRMCRO != null)
                //    {
                //        qry.Add(new rMedicoAssistente()
                //        {
                //            Medico = _Itens.ToString()
                //        });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Diagnosticos
        public class rDiagnosticos
        {
            public string CidPrincipal { get; set; }
            public string OutrosCids { get; set; }
        }
        public List<rDiagnosticos> lstDiagnosticos
        {
            get
            {
                //string _CIDPrincipal = string.Empty;
                //if (!vmSAR.CidPrincipal.IsEmptyOrWhiteSpace())
                //{
                //    _CIDPrincipal = vmSAR.CidPrincipal;
                //}

                //string _OutrosCIDs = string.Empty;
                //if (!vmSAR.Cids.IsNull())
                //{
                //    foreach (var item in vmSAR.Cids)
                //    {
                //        if (!_OutrosCIDs.IsEmptyOrWhiteSpace())
                //            _OutrosCIDs += Environment.NewLine;
                //        _OutrosCIDs += item.CidDiagnostico.ToString();
                //    }
                //}

                List<rDiagnosticos> qry = new List<rDiagnosticos>();
                //try
                //{
                //    if ((!vmSAR.CidPrincipal.IsEmptyOrWhiteSpace()))
                //    {
                //        qry.Add(new rDiagnosticos()
                //                {
                //                    CidPrincipal = _CIDPrincipal.ToString(),
                //                    OutrosCids = _OutrosCIDs.ToString()
                //                });
                //    }

                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Causa Externa
        public class rCausaExterna
        {
            public string Descricao { get; set; }
        }
        public List<rCausaExterna> lstCausaExterna
        {
            get
            {
                //string _CausaExterna = string.Empty;
                //if (!vmSAR.CausaExterna.IsNull())
                //{
                //    foreach (var item in vmSAR.CausaExterna)
                //    {
                //        if (!_CausaExterna.IsEmptyOrWhiteSpace())
                //            _CausaExterna += Environment.NewLine;
                //        _CausaExterna += item.Descricao +
                //            "  Observação: " + item.Observacao;
                //    }
                //}

                List<rCausaExterna> qry = new List<rCausaExterna>();
                //try
                //{
                //    if (!_CausaExterna.IsEmptyOrWhiteSpace())
                //    {
                //        qry.Add(new rCausaExterna()
                //                    {
                //                        Descricao = _CausaExterna.ToString()
                //                    });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Procedimentos
        public class rProcedimentos
        {
            public string Procedimento { get; set; }
        }
        public List<rProcedimentos> lstProcedimentos
        {
            get
            {
                //string _Procedimentos = string.Empty;
                //if (!vmSAR.Procedimentos.IsNull())
                //    foreach (var item in vmSAR.Procedimentos)
                //    {
                //        if (item.ProcedimentoNome.Equals("Não se Aplica"))
                //        {
                //            _Procedimentos += item.ProcedimentoNome;
                //            break;
                //        }

                //        if (!_Procedimentos.IsEmptyOrWhiteSpace())
                //            _Procedimentos += Environment.NewLine;

                //        _Procedimentos += item.ProcedimentoNome.ToString() +
                //            "  Data: " + item.ProcedimentoData.ToString();
                //    }

                List<rProcedimentos> qry = new List<rProcedimentos>();
                //try
                //{
                //    if (!_Procedimentos.IsEmptyOrWhiteSpace())
                //    {
                //        qry.Add(new rProcedimentos()
                //                    {
                //                        Procedimento = _Procedimentos.ToString()
                //                    });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Principais Farmacos da Internacao
        public class rPrincipaisFarmacos
        {
            public string Farmacos { get; set; }
            public string Observacao { get; set; }
        }
        public List<rPrincipaisFarmacos> lstPrincipaisFarmacos
        {
            get
            {
                //string _Farmacos = string.Empty;
                //if (!vmSAR.FarmacosInternacao.IsNull())
                //{
                //    foreach (var item in vmSAR.FarmacosInternacao)
                //    {
                //        if (!_Farmacos.IsEmptyOrWhiteSpace())
                //            _Farmacos += Environment.NewLine;
                //        _Farmacos += item.Descricao.ToString();
                //    }
                //}

                //string _Observacao = string.Empty;
                //if (!vmSAR.ObservacaoFarmacos.IsEmptyOrWhiteSpace())
                //    _Observacao += vmSAR.ObservacaoFarmacos.ToString();

                List<rPrincipaisFarmacos> qry = new List<rPrincipaisFarmacos>();
                //try
                //{
                //    if ((!_Farmacos.IsEmptyOrWhiteSpace()) || (!_Observacao.IsEmptyOrWhiteSpace()))
                //    {
                //        qry.Add(new rPrincipaisFarmacos()
                //                {
                //                    Farmacos = _Farmacos.ToString(),
                //                    Observacao = _Observacao.ToString()
                //                });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Evolucao
        public class rEvolucao
        {
            public string Evolucao { get; set; }
        }
        public List<rEvolucao> lstEvolucao
        {
            get
            {
                //string _Evolucao = string.Empty;
                //if (!vmSAR.Evolucao.IsEmptyOrWhiteSpace())
                //    _Evolucao = vmSAR.Evolucao.ToString();

                List<rEvolucao> qry = new List<rEvolucao>();
                //try
                //{
                //    if (!_Evolucao.IsEmptyOrWhiteSpace())
                //    {
                //        qry.Add(new rEvolucao()
                //                {
                //                    Evolucao = _Evolucao.ToString()
                //                });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Resultados Principais de Exames Diagnosticos
        public class rExamesDiagnosticos
        {
            public string Exames { get; set; }
            public string Observacoes { get; set; }
            public string AnalisesClinicas { get; set; }


        }
        public List<rExamesDiagnosticos> lstExamesDiagnosticos
        {
            get
            {
                //string _Exames = string.Empty;
                //string _Observacoes = string.Empty;
                //string _AnalisesClinicas = string.Empty;
                //if (!vmSAR.Exames.IsNull())
                //{
                //    foreach (var item in vmSAR.Exames)
                //    {
                //        if (!_Exames.IsEmptyOrWhiteSpace())
                //            _Exames += Environment.NewLine;
                //        _Exames += (item.Procedimento.IsEmptyOrWhiteSpace() ? string.Empty : item.Procedimento.ToString()) +
                //            ((!item.Resultado.IsEmptyOrWhiteSpace()) ? "  Resultado: " + item.Resultado.ToString() : string.Empty);
                //        _Observacoes = item.Observacao.IsEmptyOrWhiteSpace() ? string.Empty : item.Observacao.ToString();
                //        _AnalisesClinicas = item.AnalisesClinicas.IsEmptyOrWhiteSpace() ? string.Empty : item.AnalisesClinicas.ToString();
                //    }
                //}

                List<rExamesDiagnosticos> qry = new List<rExamesDiagnosticos>();
                //try
                //{
                //    if ((!_Exames.IsEmptyOrWhiteSpace()) || (!_Observacoes.IsEmptyOrWhiteSpace()))
                //    {
                //        qry.Add(new rExamesDiagnosticos()
                //                {
                //                    Exames = _Exames.ToString(),
                //                    Observacoes = _Observacoes.ToString(),
                //                    AnalisesClinicas = _AnalisesClinicas.ToString()
                //                });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Condicoes de Alta
        public class rCondicoesDeAlta
        {
            public string Descricao { get; set; }
        }

        public List<rCondicoesDeAlta> lstCondicoesDeAlta
        {
            get
            {
                //string _Descricao = string.Empty;
                //if (!vmSAR.CondicaoAlta.IsEmptyOrWhiteSpace())
                //    _Descricao = vmSAR.CondicaoAlta.ToString();

                List<rCondicoesDeAlta> qry = new List<rCondicoesDeAlta>();
                //try
                //{
                //    if (!_Descricao.IsEmptyOrWhiteSpace())
                //    {
                //        qry.Add(new rCondicoesDeAlta()
                //                {
                //                    Descricao = _Descricao.ToString()
                //                });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Motivo Internacao
        public class rMotivoInternacao
        {
            public string Descricao { get; set; }
        }

        public List<rMotivoInternacao> lstMotivoInternacao
        {
            get
            {
                //string _Descricao = string.Empty;
                //if (!vmSAR.CondicaoInternacao.IsEmptyOrWhiteSpace())
                //    _Descricao = vmSAR.CondicaoInternacao;

                List<rMotivoInternacao> qry = new List<rMotivoInternacao>();
                //try
                //{
                //    if (!_Descricao.IsEmptyOrWhiteSpace())
                //    {
                //        qry.Add(new rMotivoInternacao()
                //        {
                //            Descricao = _Descricao
                //        });
                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Plano PosAlta
        //Medicamentos
        public class rPlanoPosAlta
        {
            public string Medicamentos { get; set; }
        }
        public List<rPlanoPosAlta> lstPlanoPosAlta
        {
            get
            {
                //string _Medicamentos = string.Empty;
                //if (!vmSAR.MedicamentoPosAlta.IsNull())
                //{
                //    foreach (var item in vmSAR.MedicamentoPosAlta)
                //    {
                //        if (!_Medicamentos.IsEmptyOrWhiteSpace())
                //            _Medicamentos += Environment.NewLine;
                //        _Medicamentos += (!item.Nome.IsEmptyOrWhiteSpace() ? item.Nome.ToString() : string.Empty) +
                //              (!item.Dose.IsEmptyOrWhiteSpace() ? " " + item.Dose.ToString() : string.Empty) +
                //              (!item.Via.IsEmptyOrWhiteSpace() ? " " + item.Via.ToString() : string.Empty) +
                //              (!item.Frequencia.IsEmptyOrWhiteSpace() ? " " + item.Frequencia.ToString() : string.Empty) +
                //              (!item.Tempo.IsEmptyOrWhiteSpace() ? " " + item.Tempo.ToString() : string.Empty);
                //    }
                //}

                List<rPlanoPosAlta> qry = new List<rPlanoPosAlta>();
                //try
                //{
                //    if (!_Medicamentos.IsEmptyOrWhiteSpace())
                //    {
                //        qry.Add(new rPlanoPosAlta()
                //                {
                //                    Medicamentos = _Medicamentos.ToString()
                //                });

                //    }
                //}
                //catch { qry = null; }
                return qry;
            }
        }

        //Recomendacoes
        public class rRecomendacoes
        {
            public string Recomendacao { get; set; }
            public string Consulta { get; set; }
            public string Urgencia { get; set; }
        }
        public List<rRecomendacoes> lstRecomendacoes
        {
            get
            {
                //string _Recomendacao = string.Empty;
                //if (!vmSAR.Recomendacao.IsNull())
                //{
                //    foreach (var item in vmSAR.Recomendacao)
                //    {
                //        if (!_Recomendacao.IsEmptyOrWhiteSpace())
                //            _Recomendacao += Environment.NewLine;
                //        _Recomendacao += (!item.Recomendacao.IsEmptyOrWhiteSpace() ? item.Recomendacao.ToString() : string.Empty) +
                //            (!item.Descricao.IsEmptyOrWhiteSpace() ? "  " + item.Descricao.ToString() : string.Empty);
                //    }
                //}

                //string _Consulta = string.Empty;
                //if (!vmSAR.Consulta.IsEmptyOrWhiteSpace())
                //    _Consulta = vmSAR.Consulta.ToString();

                //string _Urgencia = string.Empty;
                //if (!vmSAR.CasoUrgencia.IsEmptyOrWhiteSpace())
                //    _Urgencia = vmSAR.CasoUrgencia.ToString();

                List<rRecomendacoes> qry = new List<rRecomendacoes>();
                //try
                //{
                //    if ((!_Recomendacao.IsEmptyOrWhiteSpace()) | (!_Consulta.IsEmptyOrWhiteSpace()) | (!_Urgencia.IsEmptyOrWhiteSpace()))
                //    {
                //        qry.Add(new rRecomendacoes()
                //                {
                //                    Recomendacao = _Recomendacao.ToString(),
                //                    Consulta = _Consulta.ToString(),
                //                    Urgencia = _Urgencia.ToString()
                //                });
                //    }
                //}
                //catch { qry = null; }

                return qry;
            }
        }

        //Assinatura
        public class rAssinatura
        {
            public string Assinatura { get; set; }
        }

        public List<rAssinatura> lstAssinatura
        {
            get
            {
                //string _Assinatura = string.Empty;

                List<rAssinatura> qry = new List<rAssinatura>();
                //if (!vmSAR.DR.IsEmptyOrWhiteSpace())
                //{
                //    _Assinatura = vmSAR.DR.ToString();

                //    qry.Add(new rAssinatura()
                //            {
                //                Assinatura = _Assinatura.ToString()
                //            });
                //}
                return qry;
            }
        }

        #endregion
    }
}
