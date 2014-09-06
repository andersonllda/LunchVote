using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.DevExpress.v12._1.Report;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM.Internado;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM.Pediatrico;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM.PrimeiraConsulta;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM.Reconsulta;
using StructureMap;
using StructureMap.Pipeline;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Constant;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.WCF.Interfaces.Acesso;
using HMV.Core.DTO;
using System.Text;
using HMV.PEP.ViewModel.PEP;
using System.Xaml;
using System.Windows.Controls;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM;


namespace HMV.PEP.WPF.ResumoProntuario
{
    /// <summary>
    /// Interaction logic for ucRelSumarioAvaliacaoMedica.xaml
    /// </summary>
    public partial class ucRelSumarioAvaliacaoMedicaNovo : UserControlBase, IUserControl
    {
        SumarioAvaliacaoMedica _SumarioAvaliacaoMedica;

        public xReportBase report;
        public bool mostraRTF;
        FlowDocument doc = null;
        private wrpSumarioAvaliacaoMedicaCollection _listaSumarioAvaliacao;
        //private int totalLinhas;
        private Dictionary<int, int> linhaAtendimento;
        RTFHelper rtf;

        public ucRelSumarioAvaliacaoMedicaNovo()
        {
            InitializeComponent();
            linhaAtendimento = new Dictionary<int,int>();
        }

        public void SetListaDeSumarios(wrpSumarioAvaliacaoMedicaCollection vm)
        {
            _listaSumarioAvaliacao = vm;
        }

        public TextRange FindTextInRange(TextRange searchRange, string searchText)
        {
            // Search the text with IndexOf
            int offset = searchRange.Text.IndexOf(searchText);
            if(offset<0)
            return null;  // Not found
            int linha = 0;

            // Try to select the text as a contiguous range
            for(TextPointer start = searchRange.Start.GetPositionAtOffset(offset); start != searchRange.End; start = start.GetPositionAtOffset(1))
            {
                linha++;
                TextRange result = new TextRange(start, start.GetPositionAtOffset(searchText.Length));
                if (result.Text == searchText)
                {
                    result.Text += "Z" + linha + "Z";
                    return result;
                }
            }
            return null;
        }

        public bool DoSearch(RichTextBox richTextBox, string searchText, bool searchNext)
        {
            TextRange searchRange;

            // Get the range to search
            if (searchNext)
                searchRange = new TextRange(
                    richTextBox.Selection.Start.GetPositionAtOffset(1),
                    richTextBox.Document.ContentEnd);
            else
                searchRange = new TextRange(
                    richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd);

            // Do the search
            TextRange foundRange = FindTextInRange(searchRange, searchText);
            if (foundRange == null)
                return false;

            // Select the found range
            richTextBox.BeginInit();
            richTextBox.Selection.Select(foundRange.Start, foundRange.End);
            richTextBox.CaretPosition = foundRange.Start;
            richTextBox.EndInit();
            //richTextBox.Selection. foundRange.End
            //richTextBox.SelectionOpacity = 10.00;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MostraBarra">Exibir a barra de impressão (Sim/Não)</param>
        public void SetData(object pData)
        {
            if (mostraRTF)
            {
                if (doc == null)
                {
                    doc = getDataRTF(_listaSumarioAvaliacao);
                }
            }

            this._SumarioAvaliacaoMedica = (pData as SumarioAvaliacaoMedica);

            List<SumarioAvaliacaoMedicaDTO> _Dados = new List<SumarioAvaliacaoMedicaDTO>();
            _Dados.Add(new SumarioAvaliacaoMedicaDTO() { SumarioDTO = this._SumarioAvaliacaoMedica });

            #region --- vars ---
            XRSubreport identificacao = new XRSubreport();
            XRSubreport anamnese = new XRSubreport();
            XRSubreport exameFisico = new XRSubreport();
            XRSubreport planoDiagnosticoterapeutico = new XRSubreport();
            XRSubreport hipoteseDiagnostica = new XRSubreport();
            XRSubreport notasPessoais = new XRSubreport();
            XRSubreport assinatura = new XRSubreport();
            XRSubreport subjetivo = new XRSubreport();
            XRSubreport objetivo = new XRSubreport();
            XRSubreport impressao = new XRSubreport();
            XRSubreport alergias = new XRSubreport();
            XRSubreport examesrealizados = new XRSubreport();
            XRSubreport diagnosticos = new XRSubreport();
            XRSubreport planodiagnosticoterapeutico = new XRSubreport();
            XRSubreport rodape = new XRSubreport();

            List<samAnamnese> anamnes = srLAnamnese;
            List<samAnamneseCompleto> anm = srLAnamneseCompleto;
            List<samDadosDoPaciente> dadospac = srLDadosPaciente;
            List<samExameFisico> exafisico = srLExameFisico;
            List<samExamesRealizados> exareal = srLExamesRealizados;
            List<samHipoteseDiagnostica> hipdiagnost = srLHipoteseDiagnostica;
            List<samDiagnosticos> diagnost = srLDiagnosticos;
            List<samPlanoDiagnosticoTerapeutico> planodiagterap = srLPlanoDiagnosticoTerapeutico;
            List<samPlanoDiagnosticoTerapeuticoIP> planodiagIP = srLPlanoDiagnosticoTerapeuticoIP;
            List<samNotasPessoaisDoMedico> notaspessoais = srLNotasPessoaisDoMedico;
            List<samAssinatura> assin = srLAssinatura;
            List<samSubjetivo> subje = srLSubjetivo;
            List<samObjetivo> obje = srLObjetivo;
            List<samImpressao> impress = srLImpressao;
            List<samRodape> lstRodape = srLRodape();

            #endregion

            switch (this._SumarioAvaliacaoMedica.Tipo.ID)
            {
                case 1:

                    #region Primeira Consulta
                    report = new rptSumarioAvaliacaoMedicaPrimeiraConsulta();

                    identificacao = report.FindControl("xsrIdentificacao", true) as XRSubreport;
                    identificacao.ReportSource.DataSource = dadospac;

                    anamnese = report.FindControl("xsrAnamnese", true) as XRSubreport;
                    anamnese.ReportSource.DataSource = anamnes;

                    exameFisico = report.FindControl("xsrExameFisico", true) as XRSubreport;
                    exameFisico.ReportSource.DataSource = exafisico;

                    planoDiagnosticoterapeutico = report.FindControl("xsrPlanoDiagnostico", true) as XRSubreport;
                    planoDiagnosticoterapeutico.ReportSource.DataSource = planodiagterap;

                    hipoteseDiagnostica = report.FindControl("xsrHipoteseDiagnostica", true) as XRSubreport;
                    hipoteseDiagnostica.ReportSource.DataSource = hipdiagnost;

                    notasPessoais = report.FindControl("xsrNotasPessoais", true) as XRSubreport;
                    notasPessoais.ReportSource.DataSource = notaspessoais;

                    assinatura = report.FindControl("xsrAssinatura", true) as XRSubreport;
                    assinatura.ReportSource.DataSource = assin;

                    #endregion

                    break;
                case 2:

                    #region Reconsulta
                    report = new rptSumarioAvaliacaoMedicaReconsulta();

                    identificacao = report.FindControl("xsrIdentificacao", true) as DevExpress.XtraReports.UI.XRSubreport;
                    identificacao.ReportSource.DataSource = dadospac;

                    subjetivo = report.FindControl("xsrSubjetivo", true) as DevExpress.XtraReports.UI.XRSubreport;
                    subjetivo.ReportSource.DataSource = subje;

                    objetivo = report.FindControl("xsrObjetivo", true) as DevExpress.XtraReports.UI.XRSubreport;
                    objetivo.ReportSource.DataSource = obje;

                    impressao = report.FindControl("xsrImpressao", true) as DevExpress.XtraReports.UI.XRSubreport;
                    impressao.ReportSource.DataSource = impress;

                    planoDiagnosticoterapeutico = report.FindControl("xsrPlanoDiagnostico", true) as DevExpress.XtraReports.UI.XRSubreport;
                    planoDiagnosticoterapeutico.ReportSource.DataSource = planodiagterap;

                    hipoteseDiagnostica = report.FindControl("xsrHipoteseDiagnostica", true) as DevExpress.XtraReports.UI.XRSubreport;
                    hipoteseDiagnostica.ReportSource.DataSource = hipdiagnost;

                    notasPessoais = report.FindControl("xsrNotasPessoais", true) as DevExpress.XtraReports.UI.XRSubreport;
                    notasPessoais.ReportSource.DataSource = notaspessoais;

                    assinatura = report.FindControl("xsrAssinatura", true) as DevExpress.XtraReports.UI.XRSubreport;
                    assinatura.ReportSource.DataSource = assin;

                    #endregion

                    break;
                case 3:

                    #region Adulto - Internado

                    report = new rptSumarioAvaliacaoMedicaInternado();

                    identificacao = report.FindControl("xsrIdentificacao", true) as XRSubreport;
                    identificacao.ReportSource.DataSource = dadospac;

                    if (this._SumarioAvaliacaoMedica.Atendimento.IsNull())
                    {
                        (identificacao.ReportSource as srIdentificacao).lblAtend.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).lblCor.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).lblDtAtendimento.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).lblIdade.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).lblProntuario.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).lblSexo.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).BindAtendimento.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).BindCor.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).BindDataAtendimento.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).BindIdade.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).BindProntuario.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).BindSexo.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).cbCodigoBarras.Visible = false;
                        (identificacao.ReportSource as srIdentificacao).lblMsg.Visible = true;

                        (report.Report as rptSumarioAvaliacaoMedicaInternado).xsrRodape.Visible = false;
                        (report.Report as rptSumarioAvaliacaoMedicaInternado).xsrAssinatura.Visible = false;                        
                    }
                        
                    anamnese = report.FindControl("xsrAnamnese", true) as XRSubreport;
                    anamnese.ReportSource.DataSource = anm;

                    exameFisico = report.FindControl("xsrExameFisico", true) as XRSubreport;
                    exameFisico.ReportSource.DataSource = exafisico;

                    examesrealizados = report.FindControl("xsrExamesRealizados", true) as XRSubreport;
                    examesrealizados.ReportSource.DataSource = exareal;

                    diagnosticos = report.FindControl("xsrDiagnosticos", true) as XRSubreport;
                    diagnosticos.ReportSource.DataSource = diagnost;

                    planodiagnosticoterapeutico = report.FindControl("xsrPlanoDiagnosticoTerapeuticoIP", true) as XRSubreport;
                    planodiagnosticoterapeutico.ReportSource.DataSource = planodiagIP;

                    assinatura = report.FindControl("xsrAssinatura", true) as XRSubreport;
                    assinatura.ReportSource.DataSource = assin;

                    rodape = report.FindControl("xsrRodape", true) as XRSubreport;
                    rodape.ReportSource.DataSource = lstRodape;

                    #endregion

                    break;
                case 4: //

                    #region Pediatrico
                    report = new rptSumarioAvaliacaoMedicaPediatrico();

                    identificacao = report.FindControl("xsrIdentificacao", true) as XRSubreport;
                    identificacao.ReportSource.DataSource = dadospac;

                    anamnese = report.FindControl("xsrAnamnese", true) as XRSubreport;
                    anamnese.ReportSource.DataSource = anm;

                    exameFisico = report.FindControl("xsrExameFisico", true) as XRSubreport;
                    exameFisico.ReportSource.DataSource = exafisico;

                    examesrealizados = report.FindControl("xsrExamesRealizados", true) as XRSubreport;
                    examesrealizados.ReportSource.DataSource = exareal;

                    diagnosticos = report.FindControl("xsrDiagnosticos", true) as XRSubreport;
                    diagnosticos.ReportSource.DataSource = diagnost;

                    planodiagnosticoterapeutico = report.FindControl("xsrPlanoDiagnosticoTerapeuticoIP", true) as XRSubreport;
                    planodiagnosticoterapeutico.ReportSource.DataSource = planodiagIP;

                    assinatura = report.FindControl("xsrAssinatura", true) as XRSubreport;
                    assinatura.ReportSource.DataSource = assin;

                    rodape = report.FindControl("xsrRodape", true) as XRSubreport;
                    rodape.ReportSource.DataSource = lstRodape;


                    #endregion

                    break;
            }

            if (mostraRTF)
            {
                this.RelRichText.Document = doc;

                DoSearch(this.RelRichText, this._SumarioAvaliacaoMedica.ID.ToString(), true);


                //TextPointer moveTo = this.RelRichText.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                //if (moveTo != null)
                //{
                //    this.RelRichText.CaretPosition = moveTo;
                //}
                //moveTo = this.RelRichText.CaretPosition.GetNextInsertionPosition(LogicalDirection.Forward);
                //moveTo.GetNextContextPosition(LogicalDirection.Forward);
                //moveTo.GetNextContextPosition(LogicalDirection.Forward);
                //moveTo.GetNextContextPosition(LogicalDirection.Forward);
                //moveTo.GetNextContextPosition(LogicalDirection.Forward);
                //moveTo.GetNextContextPosition(LogicalDirection.Forward);
                //moveTo.GetNextContextPosition(LogicalDirection.Forward);
                //this.RelRichText.CaretPosition = moveTo;
                
                /*int linha2 = linhaAtendimento[this._SumarioAvaliacaoMedica.ID];
                //this.RelRichText.ScrollToVerticalOffset(linha2 *15.00);

                string sss = doc.ToString();
                //string aa = doc.DataContext.ToString();
                StringBuilder bui = new StringBuilder();

                bui.Append("111");
                bui.Append("2222");
                bui.Append("333 " + Environment.NewLine + "444");
                bui.Append("555");

              
                var texto = "333 " + Environment.NewLine + "444" + Environment.NewLine + "5555" + Environment.NewLine + "6666";
                var ff = texto.Split('\n');

                var teee = "ssss";
                //decimal linha = linhaAtendimento[this._SumarioAvaliacaoMedica.ID];
                //double valor = decimal.ToDouble(linha);

                //this.RelRichText.ScrollToVerticalOffset(valor*10.85);

                int total = 0, linha = 0;

                TextPointer navigator = this.RelRichText.Document.ContentStart;
                while (navigator.CompareTo(this.RelRichText.Document.ContentEnd) < 0)
                {
                    TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                    Run run = navigator.Parent as Run;

                    total++;
                    if (context == TextPointerContext.ElementStart && run != null)
                    {
                        string runText = run.Text;
                        run.Text += "  xxxxx" + total;
                        if (runText.IndexOf(this._SumarioAvaliacaoMedica.ID.ToString()) > 0)
                        {
                            linha = total;
                            break;
                        }
                    }
                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }

                RichTextBox rtfBox = this.RelRichText;
                //this.RelRichText.ScrollToEnd();
                this.RelRichText.Document.sele

                rtfBox.CaretPosition.Parent 
                var textRange = new TextRange(rtfBox.Document.ContentStart, rtfBox.CaretPosition);
                plainTextBox.Focus();
                plainTextBox.CaretIndex = textRange.Text.Length;
                rtfBox.care*/
                /*
                this.RelRichText.CaretPosition = this.RelRichText.ScrollToEnd().Document.ContentEnd;

                //TextPointer caretPos = this.RelRichText.CaretPosition;
                //caretPos = caretPos.DocumentEnd;
               // this.RelRichText.CaretPosition = caretPos;

                this.RelRichText.BeginChange();
                if (this.RelRichText.Selection.Text != string.Empty)
                {
                    this.RelRichText.Selection.Text = string.Empty;
                }
                TextPointer tp = this.RelRichText.CaretPosition.GetPositionAtOffset(1500, LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);

                //this.RelRichText.CaretPosition.InsertTextInRun(tbxInsertionText.Text);
                this.RelRichText.CaretPosition = tp;
                this.RelRichText.EndChange();
                //Keyboard.Focus(rtbTextContent);*/






               // this.RelRichText.CaretPosition = navigator;//.GetNextContextPosition(LogicalDirection.Forward);
                //this.RelRichText.ScrollToVerticalOffset(linha);


                //this.RelRichText.CaretPosition = navigator; 
               /* TextPointer moveTo = this.RelRichText.CaretPosition.GetLineStartPosition(30);// .GetNextInsertionPosition(LogicalDirection.Forward);
                if (moveTo != null)
                {
                    this.RelRichText.CaretPosition = moveTo;
                }*/


               /* RelRichText ri = new System.Windows.Controls.RichTextBox();

                this.RelRichText.se

                //this.RelRichText.Document
                this.RelRichText.Document.se .FindName("28/11/2013 10:25");*/


                //SetDataRTF(vm.ListaSumarioDeAvaliacaoMedica);

              //  this.RelRichText.CaretPosition = this.RelRichText.CaretPosition.DocumentEnd;

            }


            if (this._SumarioAvaliacaoMedica.Atendimento.IsNotNull() && this._SumarioAvaliacaoMedica.Paciente.IsNotNull())
            {
                report.Watermark.Text = string.Empty;
                bool _incompleto = this._SumarioAvaliacaoMedica.DataEncerramento == null ? true : false;
                if (_incompleto)
                {
                    report.Watermark.Text = "Documento Incompleto";
                    report.Watermark.TextDirection = DirectionMode.ForwardDiagonal;
                    report.Watermark.Font = new Font(report.Watermark.Font.FontFamily, 48);
                    report.Watermark.ForeColor = System.Drawing.Color.Silver;
                    report.Watermark.TextTransparency = 150;
                    report.Watermark.ShowBehind = false;
                    report.Watermark.PageRange = "1,1-5";
                    if (this._SumarioAvaliacaoMedica.Tipo.ID > 2)
                        this.barManager.CreateStandardLayout = false;
                }

                if (mostraRTF)
                {
                    if (_incompleto)
                    {
                        this.RelRichText.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                        this.barManager.CreateStandardLayout = false;
                    }
                    else
                    {
                        this.RelRichText.Margin = new System.Windows.Thickness(0, 26, 0, 1);
                    }




                }
                else
                    this.RelRichText.Visibility = Visibility.Hidden;
            }

            XtraReportPreviewModel model = new XtraReportPreviewModel(report);
            relSAM.Model = model;
            relSAM.Model.Zoom = 90;

            report.CreateDocument(false);
        }

        public FlowDocument getDataRTF(wrpSumarioAvaliacaoMedicaCollection itens)
        {
            FlowDocument documento = null;
            rtf = new RTFHelper();
            documento = rtf.FlowDoc();

            foreach (var item in itens)
            {

                linhaAtendimento.Add(item.ID, 100/*rtf.getTotalLinhas*/);

                this._SumarioAvaliacaoMedica = item.DomainObject;

                List<SumarioAvaliacaoMedicaDTO> _Dados = new List<SumarioAvaliacaoMedicaDTO>();
                _Dados.Add(new SumarioAvaliacaoMedicaDTO() { SumarioDTO = this._SumarioAvaliacaoMedica });

                #region vars

                XRSubreport identificacao = new XRSubreport();
                XRSubreport anamnese = new XRSubreport();
                XRSubreport exameFisico = new XRSubreport();
                XRSubreport planoDiagnosticoterapeutico = new XRSubreport();
                XRSubreport hipoteseDiagnostica = new XRSubreport();
                XRSubreport notasPessoais = new XRSubreport();
                XRSubreport assinatura = new XRSubreport();
                XRSubreport subjetivo = new XRSubreport();
                XRSubreport objetivo = new XRSubreport();
                XRSubreport impressao = new XRSubreport();
                XRSubreport alergias = new XRSubreport();
                XRSubreport examesrealizados = new XRSubreport();
                XRSubreport diagnosticos = new XRSubreport();
                XRSubreport planodiagnosticoterapeutico = new XRSubreport();
                XRSubreport rodape = new XRSubreport();

                List<samAnamnese> anamnes = srLAnamnese;
                List<samAnamneseCompleto> anm = srLAnamneseCompleto;
                List<samDadosDoPaciente> dadospac = srLDadosPaciente;
                List<samExameFisico> exafisico = srLExameFisico;
                List<samExamesRealizados> exareal = srLExamesRealizados;
                List<samHipoteseDiagnostica> hipdiagnost = srLHipoteseDiagnostica;
                List<samDiagnosticos> diagnost = srLDiagnosticos;
                List<samPlanoDiagnosticoTerapeutico> planodiagterap = srLPlanoDiagnosticoTerapeutico;
                List<samPlanoDiagnosticoTerapeuticoIP> planodiagIP = srLPlanoDiagnosticoTerapeuticoIP;
                List<samNotasPessoaisDoMedico> notaspessoais = srLNotasPessoaisDoMedico;
                List<samAssinatura> assin = srLAssinatura;
                List<samSubjetivo> subje = srLSubjetivo;
                List<samObjetivo> obje = srLObjetivo;
                List<samImpressao> impress = srLImpressao;
                List<samRodape> lstRodape = srLRodape();

                #endregion

                if (mostraRTF)
                {
                    switch (this._SumarioAvaliacaoMedica.Tipo.ID)
                    {
                        case 1:

                            #region Primeira Consulta
                            if (mostraRTF)
                            {
                                #region rtf

                                #region dados pessoais
                                if (dadospac.Count > 0)
                                {
                                    rtf.criaTitulo("Identificação " + this._SumarioAvaliacaoMedica.ID);
                                    rtf.criaTexto("Paciente: " + dadospac[0].Nome.ToString() +
                                                                   ", " + dadospac[0].Idade.ToString() +
                                                                   ", " + dadospac[0].Sexo.ToString() +
                                                                   ", Fone: " + dadospac[0].Telefone.ToString());
                                    rtf.criaTexto("Endereço: " + dadospac[0].Endereco.ToString() +
                                                                  ", CEP: " + dadospac[0].CEP.ToString());
                                    rtf.criaTexto("Bairro: " + dadospac[0].Bairro.ToString() +
                                                                  ", Cidade: " + dadospac[0].Cidade.ToString());
                                    rtf.criaTexto("Cor: " + dadospac[0].Cor.ToString() +
                                                                  ", Estado Civil: " + dadospac[0].EstadoCivil.ToString() +
                                                                  ", Profissão: " + dadospac[0].Profissao.ToString() +
                                                                  ", Registro: " + dadospac[0].Prontuario.ToString());
                                }
                                #endregion dados pessoais

                                #region Anamnese
                                if (!anamnes.IsNull())
                                {
                                    if (anamnes.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Anamnese");

                                        if (!anamnes[0].QueixaPrincipal.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Queixa Principal");
                                            rtf.criaTexto(anamnes[0].QueixaPrincipal.ToString());
                                        }

                                        if (!anamnes[0].HistoriaDoencaAtual.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("História da Doença Atual");
                                            rtf.criaTexto(anamnes[0].HistoriaDoencaAtual.ToString());
                                        }

                                        if (!anamnes[0].RevisaoDeSistemas.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Revisão De Sistemas");
                                            rtf.criaTexto(anamnes[0].RevisaoDeSistemas.ToString());
                                        }

                                        if (!anamnes[0].HistoriaPregressa.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("História Pregressa");
                                            rtf.criaTexto(anamnes[0].HistoriaPregressa.ToString());
                                        }

                                        if (!anamnes[0].HistoriaFamiliar.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("História Familiar");
                                            rtf.criaTexto(anamnes[0].HistoriaFamiliar.ToString());
                                        }

                                        if (!anamnes[0].PerfilPsicoSocial.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Perfil Psico-Social");
                                            rtf.criaTexto(anamnes[0].PerfilPsicoSocial.ToString());
                                        }
                                    }
                                }
                                #endregion Anamnese

                                #region Exame Fisico
                                if (!exafisico.IsNull())
                                {
                                    if (exafisico.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Exame Físico");

                                        if (!exafisico[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exafisico[0].Descricao.ToString());
                                        }
                                        if (!exafisico[0].Observacao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exafisico[0].Observacao.ToString());
                                        }
                                    }
                                }
                                #endregion Exame Fisico

                                #region Plano Diagnostico
                                if (!planodiagterap.IsNull())
                                {
                                    if (planodiagterap.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Plano Diagnóstico/Terapêutico");

                                        if (!planodiagterap[0].ExamesSolicitados.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Exames Solicitados");
                                            rtf.criaTexto(planodiagterap[0].ExamesSolicitados.ToString());
                                        }

                                        if (!planodiagterap[0].Conduta.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Conduta");
                                            rtf.criaTexto(planodiagterap[0].Conduta.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Hipotese Diagnostico
                                if (!hipdiagnost.IsNull())
                                {
                                    if (hipdiagnost.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Hipótese Diagnóstica");

                                        if (!hipdiagnost[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(hipdiagnost[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Notas Pessoais
                                if (!notaspessoais.IsNull())
                                {
                                    if (notaspessoais.Count > 0)
                                    {
                                        if (!notaspessoais[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Notas Pessoais Do Médico");
                                            rtf.criaTexto(notaspessoais[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Assinatura
                                if (!assin.IsNull())
                                {
                                    if (assin.Count > 0)
                                    {
                                        if (!assin[0].Assinatura.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTexto(assin[0].Assinatura.ToString());
                                        }
                                    }
                                }
                                #endregion Assinatura

                                #endregion
                            }
                            #endregion

                            break;
                        case 2:

                            #region Reconsulta
                            if (mostraRTF)
                            {
                                #region rtf

                                #region dados pessoais
                                if (dadospac.Count > 0)
                                {
                                    rtf.criaTitulo("Identificação " + this._SumarioAvaliacaoMedica.ID);
                                    rtf.criaTexto("Paciente: " + dadospac[0].Nome.ToString() +
                                                                   ", " + dadospac[0].Idade.ToString() +
                                                                   ", " + dadospac[0].Sexo.ToString() +
                                                                   ", Fone: " + dadospac[0].Telefone.ToString());
                                    rtf.criaTexto("Endereço: " + dadospac[0].Endereco.ToString() +
                                                                  ", CEP: " + dadospac[0].CEP.ToString());
                                    rtf.criaTexto("Bairro: " + dadospac[0].Bairro.ToString() +
                                                                  ", Cidade: " + dadospac[0].Cidade.ToString());
                                    rtf.criaTexto("Cor: " + dadospac[0].Cor.ToString() +
                                                                  ", Estado Civil: " + dadospac[0].EstadoCivil.ToString() +
                                                                  ", Profissão: " + dadospac[0].Profissao.ToString() +
                                                                  ", Registro: " + dadospac[0].Prontuario.ToString());
                                }
                                #endregion dados pessoais

                                #region Subjetivo
                                if (!subje.IsNull())
                                {
                                    if (subje.Count > 0)
                                    {
                                        if (!subje[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Subjetivo");
                                            rtf.criaTexto(subje[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Subjetivo

                                #region Objetivo
                                if (!obje.IsNull())
                                {
                                    if (obje.Count > 0)
                                    {
                                        if (!obje[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Objetivo");
                                            rtf.criaTexto(obje[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Objetivo

                                #region Impressao
                                if (!impress.IsNull())
                                {
                                    if (impress.Count > 0)
                                    {
                                        if (!impress[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Impressao");
                                            rtf.criaTexto(impress[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Impressao

                                #region Plano Diagnostico
                                if (!planodiagterap.IsNull())
                                {
                                    if (planodiagterap.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Plano Diagnóstico/Terapêutico");

                                        if (!planodiagterap[0].ExamesSolicitados.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Exames Solicitados");
                                            rtf.criaTexto(planodiagterap[0].ExamesSolicitados.ToString());
                                        }

                                        if (!planodiagterap[0].Conduta.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Conduta");
                                            rtf.criaTexto(planodiagterap[0].Conduta.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Hipotese Diagnostico
                                if (!hipdiagnost.IsNull())
                                {
                                    if (hipdiagnost.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Plano Diagnóstico/Terapêutico");

                                        if (!hipdiagnost[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(hipdiagnost[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Notas Pessoais
                                if (!notaspessoais.IsNull())
                                {
                                    if (notaspessoais.Count > 0)
                                    {
                                        if (!notaspessoais[0].Descricao.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Notas Pessoais Do Médico");
                                            rtf.criaTexto(notaspessoais[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Notas Pessoais

                                #region Assinatura
                                if (!assin.IsNull())
                                {
                                    if (assin.Count > 0)
                                    {
                                        if (!assin[0].Assinatura.IsEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTexto(assin[0].Assinatura.ToString());
                                        }
                                    }
                                }
                                #endregion Assinatura

                                #endregion rtf
                            }

                            #endregion

                            break;
                        case 3:

                            #region Adulto - Internado
                            if (mostraRTF)
                            {
                                #region rtf

                                #region dados pessoais
                                if (dadospac.Count > 0)
                                {
                                    rtf.criaTitulo("Identificação " + this._SumarioAvaliacaoMedica.ID);
                                    rtf.criaTexto("Paciente: " + dadospac[0].Nome.ToString() +
                                                                   ", " + dadospac[0].Idade.ToString() +
                                                                   ", " + dadospac[0].Sexo.ToString() +
                                                                   ", Fone: " + dadospac[0].Telefone.ToString());
                                    rtf.criaTexto("Endereço: " + dadospac[0].Endereco.ToString() +
                                                                  ", CEP: " + dadospac[0].CEP.ToString());
                                    rtf.criaTexto("Bairro: " + dadospac[0].Bairro.ToString() +
                                                                  ", Cidade: " + dadospac[0].Cidade.ToString());
                                    rtf.criaTexto("Cor: " + dadospac[0].Cor.ToString() +
                                                                  ", Estado Civil: " + dadospac[0].EstadoCivil.ToString() +
                                                                  ", Profissão: " + dadospac[0].Profissao.ToString() +
                                                                  ", Registro: " + dadospac[0].Prontuario.ToString());
                                }
                                #endregion dados pessoais

                                #region anamnese
                                if (!anm.IsNull())
                                {
                                    if (anm.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Anamnese");

                                        if (anm[0].MotivoInternacao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Motivo Internação");
                                            rtf.criaTexto(anm[0].MotivoInternacao.ToString());
                                        }

                                        if (anm[0].HistoriaDoencaAtual.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("História da Doença Atual");
                                            rtf.criaTexto(anm[0].HistoriaDoencaAtual.ToString());
                                        }

                                        if (anm[0].RevisaoDeSistemas.IsNotEmptyOrWhiteSpace() || anm[0].RevisaoDeSistemasOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("Revisão De Sistemas");
                                        if (anm[0].RevisaoDeSistemas.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].RevisaoDeSistemas.ToString());
                                        }
                                        if (anm[0].RevisaoDeSistemasOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].RevisaoDeSistemasOutros.ToString());
                                        }

                                        if (anm[0].AlergiasRTF.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Alergias");
                                            rtf.criaTexto(anm[0].AlergiasRTF.ToString());
                                        }

                                        if (anm[0].HistoriaPregressa.IsNotEmptyOrWhiteSpace() || anm[0].HistoriaPregressaOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("História Pregressa");
                                        if (anm[0].HistoriaPregressa.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].HistoriaPregressa.ToString());
                                        }
                                        if (anm[0].HistoriaPregressaOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].HistoriaPregressaOutros.ToString());
                                        }

                                        if (anm[0].HistoriaFamiliar.IsNotEmptyOrWhiteSpace() || anm[0].HistoriaFamiliarOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("História Familiar");
                                        if (anm[0].HistoriaFamiliar.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].HistoriaFamiliar.ToString());
                                        }
                                        if (anm[0].HistoriaFamiliarOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].HistoriaFamiliarOutros.ToString());
                                        }

                                        if (anm[0].PerfilPsicoSocial.IsNotEmptyOrWhiteSpace() || anm[0].PerfilPsicoSocialOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("Perfil Psico-Social");
                                        if (anm[0].PerfilPsicoSocial.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].PerfilPsicoSocial.ToString());
                                        }
                                        if (anm[0].PerfilPsicoSocialOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].PerfilPsicoSocialOutros.ToString());
                                        }

                                        if (anm[0].MedicamentosEmUsoRTF.IsNotEmptyOrWhiteSpace())
                                        {
                                            //rtf.criaSubTituloSublinhado("Medicamentos em Uso");
                                            rtf.criaSubTituloSublinhado("Medicamentos Habituais");
                                            rtf.criaTexto(anm[0].MedicamentosEmUsoRTF.ToString());
                                        }
                                    }
                                }
                                #endregion

                                #region Exame Fisico
                                if (exafisico.IsNotNull())
                                {
                                    if (exafisico.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Exame Físico");

                                        if (exafisico[0].Descricao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exafisico[0].Descricao.ToString());
                                        }
                                        if (exafisico[0].Observacao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exafisico[0].Observacao.ToString());
                                        }
                                        if (exafisico[0].Outros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(exafisico[0].Outros.ToString());
                                        }
                                    }
                                }
                                #endregion Exame Fisico

                                #region Exames Realizados
                                if (exareal.IsNotNull())
                                {
                                    if (exareal.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Exames Realizados");

                                        if (exareal[0].ExamesRealizadosNega.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exareal[0].ExamesRealizadosNega);
                                        }
                                        else
                                            rtf.criaTexto(exareal[0].ExamesRealizados);
                                    }
                                }
                                #endregion

                                #region Plano Diagnostico
                                //if (srLPlanoDiagnosticoTerapeutico.IsNotNull())
                                //{
                                //    if (srLPlanoDiagnosticoTerapeutico.Count > 0)
                                //    {
                                //        rtf.criaLinhaEmBranco();
                                //        rtf.criaTitulo("Plano Diagnóstico/Terapêutico");

                                //        if (srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.IsNotEmptyOrWhiteSpace())
                                //        {
                                //            rtf.criaSubTituloSublinhado("Exames Solicitados");
                                //            rtf.criaTexto(srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.ToString());
                                //        }

                                //        if (srLPlanoDiagnosticoTerapeutico[0].Conduta.IsNotEmptyOrWhiteSpace())
                                //        {
                                //            rtf.criaSubTituloSublinhado("Conduta");
                                //            rtf.criaTexto(srLPlanoDiagnosticoTerapeutico[0].Conduta.ToString());
                                //        }
                                //    }
                                //}
                                #endregion Plano Diagnostico

                                #region Hipotese Diagnostico
                                if (diagnost.IsNotNull())
                                {
                                    if (diagnost.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Diagnóstico / Hipótese Diagnóstica");

                                        if (diagnost[0].CidPrincipal.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("CID Principal");
                                            rtf.criaTexto(diagnost[0].CidPrincipal.ToString());
                                        }
                                        if (diagnost[0].OutrosCids.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros CIDs");
                                            rtf.criaTexto(diagnost[0].OutrosCids);
                                        }
                                        if (diagnost[0].HipotesesDiagnosticas.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Hipóteses Diagnósticas");
                                            rtf.criaTexto(diagnost[0].HipotesesDiagnosticas);
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Plano Diagnostico Terapeutico
                                if (planodiagIP.IsNotNull())
                                {
                                    if (planodiagIP.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Plano Diagnóstico Terapêutico");

                                        if (planodiagIP[0].Conduta.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Conduta");
                                            rtf.criaTexto(planodiagIP[0].Conduta);
                                        }

                                        if (planodiagIP[0].Exames.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Exames");
                                            rtf.criaTexto(planodiagIP[0].Exames);
                                        }

                                        if (planodiagIP[0].CirurgiaProposta.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Cirurgia Proposta");
                                            rtf.criaTexto(planodiagIP[0].CirurgiaProposta);
                                        }
                                    }
                                }
                                #endregion

                                #region Notas Pessoais
                                if (notaspessoais.IsNotNull())
                                {
                                    if (notaspessoais.Count > 0)
                                    {
                                        if (notaspessoais[0].Descricao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Notas Pessoais Do Médico");
                                            rtf.criaTexto(notaspessoais[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Assinatura
                                if (assin.IsNotNull())
                                {
                                    if (assin.Count > 0)
                                    {
                                        if (assin[0].Assinatura.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTexto(assin[0].Assinatura.ToString());
                                        }
                                    }
                                }
                                #endregion Assinatura

                                #endregion
                            }
                            #endregion

                            break;
                        case 4:

                            #region Pediatrico
                            if (mostraRTF)
                            {
                                #region rtf

                                #region dados pessoais
                                if (srLDadosPaciente.Count > 0)
                                {
                                    rtf.criaTitulo("Identificação " + this._SumarioAvaliacaoMedica.ID);
                                    rtf.criaTexto("Paciente: " + srLDadosPaciente[0].Nome.ToString() +
                                                                   ", " + srLDadosPaciente[0].Idade.ToString() +
                                                                   ", " + srLDadosPaciente[0].Sexo.ToString() +
                                                                   ", Fone: " + srLDadosPaciente[0].Telefone.ToString());
                                    rtf.criaTexto("Endereço: " + srLDadosPaciente[0].Endereco.ToString() +
                                                                  ", CEP: " + srLDadosPaciente[0].CEP.ToString());
                                    rtf.criaTexto("Bairro: " + srLDadosPaciente[0].Bairro.ToString() +
                                                                  ", Cidade: " + srLDadosPaciente[0].Cidade.ToString());
                                    rtf.criaTexto("Cor: " + srLDadosPaciente[0].Cor.ToString() +
                                                                  ", Estado Civil: " + srLDadosPaciente[0].EstadoCivil.ToString() +
                                                                  ", Profissão: " + srLDadosPaciente[0].Profissao.ToString() +
                                                                  ", Registro: " + srLDadosPaciente[0].Prontuario.ToString());
                                }
                                #endregion dados pessoais

                                #region anamnese
                                if (!anm.IsNull())
                                {
                                    if (anm.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Anamnese");

                                        if (anm[0].MotivoInternacao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Motivo Internação");
                                            rtf.criaTexto(anm[0].MotivoInternacao.ToString());
                                        }

                                        if (anm[0].HistoriaDoencaAtual.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("História da Doença Atual");
                                            rtf.criaTexto(anm[0].HistoriaDoencaAtual.ToString());
                                        }
                                        /////////////////////////////////////
                                        if (anm[0].HistoriaPerinatal.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("História Perinatal");
                                            rtf.criaTexto(anm[0].HistoriaPerinatal.ToString());
                                        }

                                        if (anm[0].ImunizacoesRTF.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Imunizações");
                                            rtf.criaTexto(anm[0].ImunizacoesRTF.ToString());
                                        }

                                        if (anm[0].OutrasVacinasRTF.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outras Vacinas");
                                            rtf.criaTexto(anm[0].OutrasVacinasRTF.ToString());
                                        }

                                        /////////////////////////////////////
                                        if (anm[0].RevisaoDeSistemas.IsNotEmptyOrWhiteSpace() || anm[0].RevisaoDeSistemasOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("Revisão De Sistemas");
                                        if (anm[0].RevisaoDeSistemas.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].RevisaoDeSistemas.ToString());
                                        }
                                        if (anm[0].RevisaoDeSistemasOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].RevisaoDeSistemasOutros.ToString());
                                        }

                                        if (anm[0].AlergiasRTF.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Alergias");
                                            rtf.criaTexto(anm[0].AlergiasRTF.ToString());
                                        }

                                        if (anm[0].HistoriaPregressa.IsNotEmptyOrWhiteSpace() || anm[0].HistoriaPregressaOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("História Pregressa");
                                        if (anm[0].HistoriaPregressa.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].HistoriaPregressa.ToString());
                                        }
                                        if (anm[0].HistoriaPregressaOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].HistoriaPregressaOutros.ToString());
                                        }

                                        if (anm[0].HistoriaFamiliar.IsNotEmptyOrWhiteSpace() || anm[0].HistoriaFamiliarOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("História Familiar");
                                        if (anm[0].HistoriaFamiliar.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].HistoriaFamiliar.ToString());
                                        }
                                        if (anm[0].HistoriaFamiliarOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].HistoriaFamiliarOutros.ToString());
                                        }

                                        if (anm[0].PerfilPsicoSocial.IsNotEmptyOrWhiteSpace() || anm[0].PerfilPsicoSocialOutros.IsNotEmptyOrWhiteSpace())
                                            rtf.criaSubTituloSublinhado("Perfil Psico-Social");
                                        if (anm[0].PerfilPsicoSocial.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(anm[0].PerfilPsicoSocial.ToString());
                                        }
                                        if (anm[0].PerfilPsicoSocialOutros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(anm[0].PerfilPsicoSocialOutros.ToString());
                                        }

                                        if (anm[0].MedicamentosEmUsoRTF.IsNotEmptyOrWhiteSpace())
                                        {
                                            //rtf.criaSubTituloSublinhado("Medicamentos em Uso");
                                            rtf.criaSubTituloSublinhado("Medicamentos Habituais");
                                            rtf.criaTexto(anm[0].MedicamentosEmUsoRTF.ToString());
                                        }
                                    }
                                }
                                #endregion

                                #region Exame Fisico
                                if (exafisico.IsNotNull())
                                {
                                    if (exafisico.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Exame Físico");

                                        if (exafisico[0].Descricao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exafisico[0].Descricao.ToString());
                                        }
                                        if (exafisico[0].Observacao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exafisico[0].Observacao.ToString());
                                        }
                                        if (exafisico[0].Outros.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros");
                                            rtf.criaTexto(exafisico[0].Outros.ToString());
                                        }
                                    }
                                }
                                #endregion Exame Fisico

                                #region Exames Realizados
                                if (exareal.IsNotNull())
                                {
                                    if (exareal.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Exames Realizados");

                                        if (exareal[0].ExamesRealizadosNega.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaTexto(exareal[0].ExamesRealizadosNega);
                                        }
                                        else
                                            rtf.criaTexto(exareal[0].ExamesRealizados);
                                    }
                                }
                                #endregion

                                #region Plano Diagnostico
                                //if (srLPlanoDiagnosticoTerapeutico.IsNotNull())
                                //{
                                //    if (srLPlanoDiagnosticoTerapeutico.Count > 0)
                                //    {
                                //        rtf.criaLinhaEmBranco();
                                //        rtf.criaTitulo("Plano Diagnóstico/Terapêutico");

                                //        if (srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.IsNotEmptyOrWhiteSpace())
                                //        {
                                //            rtf.criaSubTituloSublinhado("Exames Solicitados");
                                //            rtf.criaTexto(srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.ToString());
                                //        }

                                //        if (srLPlanoDiagnosticoTerapeutico[0].Conduta.IsNotEmptyOrWhiteSpace())
                                //        {
                                //            rtf.criaSubTituloSublinhado("Conduta");
                                //            rtf.criaTexto(srLPlanoDiagnosticoTerapeutico[0].Conduta.ToString());
                                //        }
                                //    }
                                //}
                                #endregion Plano Diagnostico

                                #region Hipotese Diagnostico
                                if (diagnost.IsNotNull())
                                {
                                    if (diagnost.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Diagnóstico / Hipótese Diagnóstica");

                                        if (diagnost[0].CidPrincipal.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("CID Principal");
                                            rtf.criaTexto(diagnost[0].CidPrincipal.ToString());
                                        }
                                        if (diagnost[0].OutrosCids.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Outros CIDs");
                                            rtf.criaTexto(diagnost[0].OutrosCids);
                                        }
                                        if (diagnost[0].HipotesesDiagnosticas.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Hipóteses Diagnósticas");
                                            rtf.criaTexto(diagnost[0].HipotesesDiagnosticas);
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Plano Diagnostico Terapeutico
                                if (planodiagIP.IsNotNull())
                                {
                                    if (planodiagIP.Count > 0)
                                    {
                                        rtf.criaLinhaEmBranco();
                                        rtf.criaTitulo("Plano Diagnóstico Terapêutico");

                                        if (planodiagIP[0].Conduta.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Conduta");
                                            rtf.criaTexto(planodiagIP[0].Conduta);
                                        }

                                        if (planodiagIP[0].Exames.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Exames");
                                            rtf.criaTexto(planodiagIP[0].Exames);
                                        }

                                        if (planodiagIP[0].CirurgiaProposta.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaSubTituloSublinhado("Cirurgia Proposta");
                                            rtf.criaTexto(planodiagIP[0].CirurgiaProposta);
                                        }
                                    }
                                }
                                #endregion

                                #region Notas Pessoais
                                if (notaspessoais.IsNotNull())
                                {
                                    if (notaspessoais.Count > 0)
                                    {
                                        if (notaspessoais[0].Descricao.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTitulo("Notas Pessoais Do Médico");
                                            rtf.criaTexto(notaspessoais[0].Descricao.ToString());
                                        }
                                    }
                                }
                                #endregion Plano Diagnostico

                                #region Assinatura
                                if (assin.IsNotNull())
                                {
                                    if (assin.Count > 0)
                                    {
                                        if (assin[0].Assinatura.IsNotEmptyOrWhiteSpace())
                                        {
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaLinhaEmBranco();
                                            rtf.criaTexto(assin[0].Assinatura.ToString());
                                        }
                                    }
                                }
                                #endregion Assinatura

                                #endregion
                            }

                            #endregion

                            break;
                    }

                }

                rtf.criaLinhaEmBranco();
                rtf.criaLinhaEmBranco();

            }

            //totalLinhas = 10;// rtf.getTotalLinhas;

            return documento;
        }

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {
            report.Imprime();
        }

        public bool CancelClose
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Show()
        {
            ReportPrintTool printTool = new ReportPrintTool(report);
            printTool.ShowPreviewDialog();
        }

        internal void Imprimir()
        {
            report.Imprime();
            App.Log(this.GetType().Assembly, "SUMARIOAVALIACAO", this._SumarioAvaliacaoMedica.Atendimento.ID, "Finalizou Sumario");
        }

        #region RELATORIO

        #region DadosDoPaciente
        public class samDadosDoPaciente
        {
            public string Nome { get; set; }
            public string Sexo { get; set; }
            public string Cor { get; set; }
            public string EstadoCivil { get; set; }
            public string Telefone { get; set; }
            public string Idade { get; set; }
            public string Profissao { get; set; }
            public string Endereco { get; set; }
            public string Bairro { get; set; }
            public string Cidade { get; set; }
            public string Estado { get; set; }
            public string CEP { get; set; }
            public string Prontuario { get; set; }
            public string Atendimento { get; set; }
            public string DataAtendimento { get; set; }
        }

        public List<samDadosDoPaciente> srLDadosPaciente
        {
            get
            {
                List<samDadosDoPaciente> qry = new List<samDadosDoPaciente>();
                if (this._SumarioAvaliacaoMedica.Paciente.IsNotNull())
                {
                    Paciente pac = this._SumarioAvaliacaoMedica.Paciente;
                    qry.Add(new samDadosDoPaciente()
                    {
                        Nome = pac.Nome.ToString(),
                        Sexo = pac.Sexo.ToString(),
                        Cor = pac.Cor.ToString(),
                        EstadoCivil = pac.EstadoCivil.ToString(),
                        Telefone = (pac.Telefone == null ? "" : (pac.DDDTelefone == null ? "" : pac.DDDTelefone.ToString().PadLeft(3, '0').Substring(0, 3) + " - ") + (pac.Telefone == null ? "" : pac.Telefone.ToString().PadLeft(8, '0').Substring(0, 8))),
                        Idade = pac.Idade.GetDate(this._SumarioAvaliacaoMedica.DataProntuario),
                        Profissao = pac.Profissao.IsNull() ? string.Empty : pac.Profissao.Descricao.ToString(),
                        Endereco = pac.Endereco.IsNull() ? string.Empty : pac.Endereco.ToString() + ", " + pac.Numero + " Comp: " + (pac.Complemento.IsNull() ? string.Empty : pac.Complemento),
                        Bairro = pac.Bairro.IsNull() ? string.Empty : pac.Bairro.ToString(),
                        Cidade = pac.Cidade.IsNull() ? string.Empty : pac.Cidade.Descricao.ToString(),
                        Estado = pac.Cidade.IsNull() ? string.Empty : pac.Cidade.Estado.Descricao.ToString(),
                        CEP = pac.CEP.IsNull() ? string.Empty : pac.CEP.ToString(),
                        Prontuario = pac.ID.ToString(),
                        Atendimento = this._SumarioAvaliacaoMedica.Atendimento == null ? this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.IsNull() ? string.Empty : this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.ID.ToString() : this._SumarioAvaliacaoMedica.Atendimento.ID.ToString(),
                        DataAtendimento = this._SumarioAvaliacaoMedica.Atendimento == null ? this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.IsNull() ? string.Empty : this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.DataAtendimento.ToShortDateString() : this._SumarioAvaliacaoMedica.Atendimento.DataAtendimento.ToShortDateString()
                    });
                }
                else
                {
                    qry.Add(new samDadosDoPaciente()
                    {
                        Nome = this._SumarioAvaliacaoMedica.NomePaciente,
                        Idade = this._SumarioAvaliacaoMedica.Idade.ToString(),
                        Telefone = this._SumarioAvaliacaoMedica.Telefone
                    });
                }
                return qry;
            }
        }
        #endregion

        #region Subjetivo
        public class samSubjetivo
        {
            public string Descricao { get; set; }
        }

        public List<samSubjetivo> srLSubjetivo
        {
            get
            {
                List<samSubjetivo> qry = new List<samSubjetivo>();
               
                if (!this._SumarioAvaliacaoMedica.Subjetivo.IsEmpty())
                {
                    var sub = XmlCharacterWhitelist(this._SumarioAvaliacaoMedica.Subjetivo);
                    qry.Add(new samSubjetivo()
                    {
                        Descricao = sub
                    });
                }
                return qry;
            }
        }

        public static string XmlCharacterWhitelist(string in_string)
        {
            if (in_string == null) return null;

            StringBuilder sbOutput = new StringBuilder();
            char ch;

            for (int i = 0; i < in_string.Length; i++)
            {
                ch = in_string[i];
                if ((ch >= 0x0020 && ch <= 0xD7FF) ||
                        (ch >= 0xE000 && ch <= 0xFFFD) ||
                        ch == 0x0009 ||
                        ch == 0x000A ||
                        ch == 0x000D)
                {
                    sbOutput.Append(ch);
                }
            }
            return sbOutput.ToString();
        }   

        #endregion

        #region Objetivo
        public class samObjetivo
        {
            public string Descricao { get; set; }
        }

        public List<samObjetivo> srLObjetivo
        {
            get
            {
                List<samObjetivo> qry = new List<samObjetivo>();

                if (!this._SumarioAvaliacaoMedica.Objetivo.IsEmpty())
                {
                    qry.Add(new samObjetivo()
                    {
                        Descricao = this._SumarioAvaliacaoMedica.Objetivo.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.Objetivo
                    });
                }
                return qry;
            }
        }

        #endregion

        #region Impressao
        public class samImpressao
        {
            public string Descricao { get; set; }
        }

        public List<samImpressao> srLImpressao
        {
            get
            {
                List<samImpressao> qry = new List<samImpressao>();

                if (!this._SumarioAvaliacaoMedica.Impressao.IsEmpty())
                {
                    qry.Add(new samImpressao()
                    {
                        Descricao = this._SumarioAvaliacaoMedica.Impressao.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.Impressao
                    });
                }
                return qry;
            }
        }

        #endregion

        #region PlanoDiagnosticoTerapeutico
        public class samPlanoDiagnosticoTerapeutico
        {
            public string ExamesSolicitados { get; set; }
            public string Conduta { get; set; }
        }

        public List<samPlanoDiagnosticoTerapeutico> srLPlanoDiagnosticoTerapeutico
        {
            get
            {
                List<samPlanoDiagnosticoTerapeutico> qry = new List<samPlanoDiagnosticoTerapeutico>();

                if ((!this._SumarioAvaliacaoMedica.PlanoDiagnosticoExamesSolicitados.IsEmpty()) || (!this._SumarioAvaliacaoMedica.PlanoDiagnosticoConduta.IsEmpty()))
                {
                    qry.Add(new samPlanoDiagnosticoTerapeutico()
                    {
                        ExamesSolicitados = this._SumarioAvaliacaoMedica.PlanoDiagnosticoExamesSolicitados.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.PlanoDiagnosticoExamesSolicitados,
                        Conduta = this._SumarioAvaliacaoMedica.PlanoDiagnosticoConduta.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.PlanoDiagnosticoConduta
                    });
                }
                return qry;
            }
        }

        #endregion

        #region PlanoDiagnosticoTerapeuticoIP
        public class samPlanoDiagnosticoTerapeuticoIP
        {
            public string Conduta { get; set; }
            public string Exames { get; set; }
            public string CirurgiaProposta { get; set; }
        }

        public List<samPlanoDiagnosticoTerapeuticoIP> srLPlanoDiagnosticoTerapeuticoIP
        {
            get
            {
                string _Conduta = string.Empty;
                _Conduta = this._SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.Conduta;
                string _Exames = string.Empty;
                _Exames = this._SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.ExamesSolicitados;
                string _CirurgiaProposta = string.Empty;
                _CirurgiaProposta = this._SumarioAvaliacaoMedica.PlanoDiagnosticoTerapeutico.CirurgiaProposta;

                List<samPlanoDiagnosticoTerapeuticoIP> qry = new List<samPlanoDiagnosticoTerapeuticoIP>();
                qry.Add(new samPlanoDiagnosticoTerapeuticoIP()
                {
                    Conduta = _Conduta,
                    Exames = _Exames,
                    CirurgiaProposta = _CirurgiaProposta
                });

                return qry;
            }
        }
        #endregion

        #region HipoteseDiagnostica
        public class samHipoteseDiagnostica
        {
            public string Descricao { get; set; }
            public string Observacao { get; set; }
        }

        public List<samHipoteseDiagnostica> srLHipoteseDiagnostica
        {
            get
            {
                List<samHipoteseDiagnostica> qry = null;
                if (this._SumarioAvaliacaoMedica.Diagnosticos == null)
                    qry = null;
                else
                    if (this._SumarioAvaliacaoMedica.Diagnosticos.Count == 0)
                        qry = null;
                    else
                        qry = (from Lista in this._SumarioAvaliacaoMedica.Diagnosticos
                               where !Lista.Cid.IsNull() //.Where(x => x..Equals(true))
                               select new samHipoteseDiagnostica
                               {
                                   Descricao = (Lista.Cid.Id.IsEmpty() ? "" : Lista.Cid.Id) + " - " + (Lista.Cid.Descricao.IsEmpty() ? "" : Lista.Cid.Descricao),
                                   Observacao = Lista.Complemento.IsEmpty() ? "" : Lista.Complemento
                               }).ToList();
                return qry;
            }
        }

        #endregion

        #region Historia Perinatal

        public List<samHistoriaPerinatal> srLHistoriaPerinatal
        {
            get
            {
                string _HistoriaPerinatal = string.Empty;

                string idadeGestacional = "";
                string aleitamentoMaterno = "";
                string intercorrenciaPerinatal = "";

                if (!_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IsNull())
                {

                    if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.InfoDisponivel.Equals(SimNao.Sim))
                        _HistoriaPerinatal = "<< Informações não disponíveis >>";
                    else
                    {

                        if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IdadeGestacionalTermo == SimNao.Sim)
                            idadeGestacional = "Idade Gestacional: A termo";
                        else if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IdadeGestacionalTermo == SimNao.Nao && _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.PrematuridadeSemanas != 0)
                            idadeGestacional = "Prematuridade: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.PrematuridadeSemanas + " semanas";

                        if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.AleitamentoMaterno == SimNao.Sim)
                            aleitamentoMaterno = "Aleitamento Marteno: Sim " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.AleitamentoMaternoMeses + " meses";
                        else if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.AleitamentoMaterno == SimNao.Nao)
                            aleitamentoMaterno = "Aleitamento Marteno: Não";

                        if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IntercorrenciaPerinatal == SimNao.Sim)
                            intercorrenciaPerinatal = "Intercorrências Perinatais: Sim " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Intercorrencia;
                        else if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IntercorrenciaPerinatal == SimNao.Nao)
                            intercorrenciaPerinatal = "Intercorrências Perinatais: Não";

                        string HistoriaP = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",

                        _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.GestacaoNormal == SimNao.Sim ? "Gestação: Normal" + Environment.NewLine : string.Empty,
                        !string.IsNullOrWhiteSpace(_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.GestacaoIntercorrencia) ? "Intercorrências: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.GestacaoIntercorrencia + Environment.NewLine : string.Empty,
                        idadeGestacional + Environment.NewLine,
                        _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.PartoVaginal == SimNao.Sim ? "Parto: Vaginal" + Environment.NewLine : _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaEletiva == SimNao.Sim ? "Parto: Cesariana Eletiva" + Environment.NewLine : _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaUrgencia == SimNao.Sim ? "Parto: Cesariana Urgência" + Environment.NewLine : string.Empty,
                        !string.IsNullOrWhiteSpace(_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaUrgenciaMotivo) ? "Motivo: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaUrgenciaMotivo : string.Empty,
                        _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Peso != null ? "Peso: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Peso + " g" + Environment.NewLine : string.Empty,
                        _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Comprimento != null ? "Comprimento : " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Comprimento + " cm" + Environment.NewLine : string.Empty,
                        intercorrenciaPerinatal + Environment.NewLine,
                        aleitamentoMaterno);

                        _HistoriaPerinatal = HistoriaP;
                    }
                }
                List<samHistoriaPerinatal> qry = new List<samHistoriaPerinatal>();
                try
                {
                    if (!_HistoriaPerinatal.IsEmptyOrWhiteSpace())
                    {
                        qry.Add(new samHistoriaPerinatal()
                        {
                            Descricao = _HistoriaPerinatal.ToString()
                        });
                    }
                }
                catch { qry = null; }
                return qry;
            }
        }
        #endregion

        #region NotasPessoaisDoMedico
        public class samNotasPessoaisDoMedico
        {
            public string Descricao { get; set; }
            public string IDUsuario { get; set; }
        }

        public List<samNotasPessoaisDoMedico> srLNotasPessoaisDoMedico
        {
            get
            {
                List<samNotasPessoaisDoMedico> qry = new List<samNotasPessoaisDoMedico>();
                try
                {
                    if ((!this._SumarioAvaliacaoMedica.NotasPessoaisMedico.IsEmpty()) || (!this._SumarioAvaliacaoMedica.Usuario.cd_usuario.IsEmpty()))
                    {
                        qry.Add(new samNotasPessoaisDoMedico()
                        {
                            Descricao = this._SumarioAvaliacaoMedica.NotasPessoaisMedico.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.NotasPessoaisMedico,
                            IDUsuario = this._SumarioAvaliacaoMedica.Usuario.cd_usuario.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.Usuario.cd_usuario
                        });
                    }
                }
                catch { qry = null; }
                return qry;
            }
        }

        #endregion

        #region Anamnese (Primeita Consulta e Reconsulta)
        public class samAnamnese
        {
            public string QueixaPrincipal { get; set; }
            public string HistoriaDoencaAtual { get; set; }
            public string RevisaoDeSistemas { get; set; }
            public string HistoriaPregressa { get; set; }
            public string HistoriaFamiliar { get; set; }
            public string PerfilPsicoSocial { get; set; }
        }

        public List<samAnamnese> srLAnamnese
        {
            get
            {
                List<samAnamnese> qry = new List<samAnamnese>();

                if ((!this._SumarioAvaliacaoMedica.QueixaPrincipal.IsEmpty()) || (!this._SumarioAvaliacaoMedica.HistoriaDoencaAtual.IsEmpty()) ||
                    (!this._SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros.IsEmpty()) || (!this._SumarioAvaliacaoMedica.HistoriaPregressa.Outros.IsEmpty()) ||
                    (!this._SumarioAvaliacaoMedica.HistoriaFamiliar.Outros.IsEmpty()) || (!this._SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros.IsEmpty()))
                {
                    qry.Add(new samAnamnese()
                    {
                        QueixaPrincipal = this._SumarioAvaliacaoMedica.QueixaPrincipal.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.QueixaPrincipal,
                        HistoriaDoencaAtual = this._SumarioAvaliacaoMedica.HistoriaDoencaAtual.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.HistoriaDoencaAtual,
                        RevisaoDeSistemas = this._SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros,
                        HistoriaPregressa = this._SumarioAvaliacaoMedica.HistoriaPregressa.Outros.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.HistoriaPregressa.Outros,
                        HistoriaFamiliar = this._SumarioAvaliacaoMedica.HistoriaFamiliar.Outros.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.HistoriaFamiliar.Outros,
                        PerfilPsicoSocial = this._SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros
                    });
                }
                return qry;
            }
        }
        #endregion

        #region Anamnese Completo (Adulto e Pediatrico)
        public class samAnamneseCompleto
        {
            public string MotivoInternacao { get; set; }
            public string HistoriaDoencaAtual { get; set; }

            public string RevisaoDeSistemas { get; set; }
            public string RevisaoDeSistemasOutros { get; set; }

            public List<samImunizacoes> Imunizacoes { get; set; }
            public string ImunizacoesRTF { get; set; }

            public List<samOutrasVacinas> OutrasVacinas { get; set; }
            public string OutrasVacinasRTF { get; set; }

            public List<samAlergias> Alergias { get; set; }
            public string AlergiasRTF { get; set; }

            public string HistoriaPregressa { get; set; }
            public string HistoriaPregressaOutros { get; set; }

            public string HistoriaFamiliar { get; set; }
            public string HistoriaFamiliarOutros { get; set; }

            public string PerfilPsicoSocial { get; set; }
            public string PerfilPsicoSocialOutros { get; set; }

            public List<samMedicamentosEmUso> MedicamentosEmUso { get; set; }
            public string MedicamentosEmUsoRTF { get; set; }
            public string MedicamentosEmUsoNaoFazUso { get; set; }

            public string HistoriaPerinatal { get; set; }
        }

        public class samOutrasVacinas
        {
            public string Data { get; set; }
            public string Descricao { get; set; }
        }

        public class samImunizacoes
        {
            public string DataInclusao { get; set; }
            public string CalendarioCompleto { get; set; }
            public string Observacao { get; set; }
        }

        public class samAlergias
        {
            public string Agente { get; set; }
            public string Descricao { get; set; }
            public string Data { get; set; }
            public string Status { get; set; }
            public string Profissional { get; set; }
            public string Comentario { get; set; }
        }

        public class samMedicamentosEmUso
        {
            public string Medicamento { get; set; }
            public string Dose { get; set; }
            public string Frequencia { get; set; }
            public string Via { get; set; }
            public string Status { get; set; }
        }

        public class samHistoriaPerinatal
        {
            public string Descricao { get; set; }
        }

        public List<samAnamneseCompleto> srLAnamneseCompleto
        {
            get
            {
                #region RevisaoDeSistemas
                IList<ITEM_REL> _revsist = (from T in _SumarioAvaliacaoMedica.RevisaoDeSistemas.ItensRevisaoDeSistemas
                                            where T.SemParticularidades == false &&
                                            string.IsNullOrEmpty(T.Observacoes) == false
                                            orderby T.Ordem
                                            select new ITEM_REL { DescricaoRelatorio = (T.Descricao.Combine(": ").Combine(T.Observacoes)), Ordem = T.Ordem }).ToList();

                string _RevisaoDeSistemas = string.Empty;
                foreach (var item in _revsist)
                {
                    _RevisaoDeSistemas += _RevisaoDeSistemas.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                    _RevisaoDeSistemas += item.DescricaoRelatorio;
                }

                _RevisaoDeSistemas += _RevisaoDeSistemas.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;

                string _RevisaoDeSistemasSP = string.Empty;
                foreach (var item in _SumarioAvaliacaoMedica.RevisaoDeSistemas.ItensRevisaoDeSistemas.Where(x => x.SemParticularidades == true).OrderBy(x => x.Ordem))
                {
                    _RevisaoDeSistemasSP += item.Descricao.Combine(", ");
                }
                if (_RevisaoDeSistemasSP.IsNotEmptyOrWhiteSpace())
                    _RevisaoDeSistemasSP = "Sem particularidades: ".Combine(_RevisaoDeSistemasSP.Trim().Remove(_RevisaoDeSistemasSP.ToString().Trim().Length - 1));

                _RevisaoDeSistemas += _RevisaoDeSistemasSP;
                #endregion RevisaoDeSistemas

                #region Imunizacoes
                List<samImunizacoes> _lstImunizacoes = new List<samImunizacoes>();
                string _Imunizacoes = string.Empty;

                if (this._SumarioAvaliacaoMedica.Imunizacoes.IsNotNull())
                {
                    _lstImunizacoes = (from T in this._SumarioAvaliacaoMedica.Imunizacoes
                                       orderby T.Observacao
                                       select new samImunizacoes
                                       {
                                           Observacao = T.Observacao,
                                           CalendarioCompleto = T.CalendarioCompleto.ToString(),
                                           DataInclusao = T.DataInclusao.ToString("dd/MM/yyyy")
                                       }).ToList();

                    foreach (var item in _lstImunizacoes)
                    {
                        _Imunizacoes += _Imunizacoes.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                        _Imunizacoes += "Data Inclusão: ".Combine(item.DataInclusao).Combine(" Calendário Completo: ").Combine(item.CalendarioCompleto).Combine(" Observação: ").Combine(item.Observacao);
                    }
                }
                #endregion

                #region OutrasVacinas
                List<samOutrasVacinas> _lstOutrasVacinas = new List<samOutrasVacinas>();
                string _OutrasVacinas = string.Empty;

                if (this._SumarioAvaliacaoMedica.OutrasVacinas.IsNotNull())
                {
                    _lstOutrasVacinas = (from T in this._SumarioAvaliacaoMedica.OutrasVacinas
                                         select new samOutrasVacinas
                                         {
                                             Data = T.Data.HasValue ? T.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                             Descricao = T.Descricao
                                         }).ToList();

                    foreach (var item in _lstOutrasVacinas)
                    {
                        _OutrasVacinas += _OutrasVacinas.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                        _OutrasVacinas += "Data: ".Combine(item.Data).Combine(" Descrição: ").Combine(item.Descricao);
                    }
                }
                #endregion

                #region Alergias
                List<samAlergias> _lstAlergias = new List<samAlergias>();

                string _AlergiasRTF = string.Empty;

                if (this._SumarioAvaliacaoMedica.Paciente.IsNotNull())
                {
                    wrpAlergiaEventoCollection _AlergiaCollection = null;

                    IRepositorioDeEventoAlergias repa = ObjectFactory.GetInstance<IRepositorioDeEventoAlergias>();
                    repa.OndeChaveIgual(this._SumarioAvaliacaoMedica.ID);
                    repa.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica);
                    var reta = repa.List();
                    if (reta.IsNotNull())
                        _AlergiaCollection = new wrpAlergiaEventoCollection(reta);

                    if (_AlergiaCollection.HasItems())
                        foreach (var item in this._SumarioAvaliacaoMedica.Paciente.Alergias)
                        {
                            if (_AlergiaCollection.Count(x => x.Chave == this._SumarioAvaliacaoMedica.ID && x.Alergia.ID == item.ID && x.Alergia.Status == StatusAlergiaProblema.Ativo) > 0)
                            {
                                if (item.Agente.IsNotEmptyOrWhiteSpace() && item.Agente.Equals(Constantes.coSemAlergiasConhecidas))
                                {
                                    _AlergiasRTF = Constantes.coSemAlergiasConhecidas;
                                }
                                else
                                {
                                    if (!_AlergiasRTF.IsEmptyOrWhiteSpace())
                                        _AlergiasRTF += Environment.NewLine;
                                    _AlergiasRTF += (item.Agente.IsEmptyOrWhiteSpace() ? string.Empty : "Agente: " + item.Agente) +
                                              (item.AlergiaTipo.IsNull() ? string.Empty : "   Tipo: " + item.AlergiaTipo.Descricao) +
                                              (item.DataInclusao.IsNull() ? string.Empty : "   Data: " + item.DataInclusao.ToString("dd/MM/yyyy")) +
                                              (item.Status.IsNull() ? string.Empty : "   Status: " + item.Status.ToString()) +
                                              (item.NomeUsuario.IsEmptyOrWhiteSpace() ? item.Profissional.IsNotNull() ? item.Profissional.nome : string.Empty : "   Profissional: " + item.NomeUsuario) +
                                              (item.Comentario.IsEmptyOrWhiteSpace() ? string.Empty : Environment.NewLine + "Comentários: " + item.Comentario);

                                    _lstAlergias.Add(new samAlergias
                                        {
                                            Agente = item.Agente,
                                            Descricao = item.AlergiaTipo.Descricao,
                                            Data = item.DataInclusao.ToString("dd/MM/yyyy"),
                                            Status = item.Status.ToString(),
                                            Profissional = item.NomeUsuario.IsNotEmptyOrWhiteSpace() ? item.NomeUsuario : item.Profissional.IsNotNull() ? item.Profissional.nome : string.Empty,
                                            Comentario = item.Comentario
                                        });
                                }
                            }
                        }
                }
                else
                {
                    foreach (var item in this._SumarioAvaliacaoMedica.Alergias)
                    {
                        _lstAlergias.Add(new samAlergias
                        {
                            Agente = item.Agente,
                            Descricao = item.AlergiaTipo.Descricao,
                            Data = item.DataInclusao.ToString("dd/MM/yyyy"),
                            Status = item.Status.ToString(),
                            Profissional = item.NomeUsuario.IsNotEmptyOrWhiteSpace() ? item.NomeUsuario : item.Profissional.IsNotNull() ? item.Profissional.nome : string.Empty,
                            Comentario = item.Comentario
                        });
                    }
                }
                #endregion

                #region HistoriaPregressa
                IList<ITEM_REL> _histpregr = (from T in _SumarioAvaliacaoMedica.HistoriaPregressa.ItensHistoriaPregressa
                                              where T.SemParticularidades == false &&
                                              string.IsNullOrEmpty(T.Observacoes) == false
                                              orderby T.Ordem
                                              select new ITEM_REL { DescricaoRelatorio = (T.Descricao.Combine(": ").Combine(T.Observacoes)) }).ToList();

                string _HistoriaPregressa = string.Empty;
                foreach (var item in _histpregr)
                {
                    _HistoriaPregressa += _HistoriaPregressa.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                    _HistoriaPregressa += item.DescricaoRelatorio;
                }

                _HistoriaPregressa += _HistoriaPregressa.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;

                string _HistoriaPregressaNG = string.Empty;
                foreach (var item in _SumarioAvaliacaoMedica.HistoriaPregressa.ItensHistoriaPregressa.Where(x => x.SemParticularidades == true).OrderBy(x => x.Ordem))
                {
                    _HistoriaPregressaNG += item.Descricao.Combine(", ");
                }

                if (_HistoriaPregressaNG.IsNotEmptyOrWhiteSpace())
                    _HistoriaPregressaNG = "Nega: ".Combine(_HistoriaPregressaNG.Trim().Remove(_HistoriaPregressaNG.ToString().Trim().Length - 1));

                _HistoriaPregressa += _HistoriaPregressaNG;

                #endregion HistoriaPregressa

                #region HistoriaFamiliar
                IList<ITEM_REL> _histfamil = (from T in _SumarioAvaliacaoMedica.HistoriaFamiliar.ItensHistoriaFamiliar
                                              where T.SemParticularidades == false &&
                                              string.IsNullOrEmpty(T.Observacoes) == false
                                              orderby T.Ordem
                                              select new ITEM_REL { DescricaoRelatorio = (T.Descricao.Combine(": ").Combine(T.Observacoes)) }).ToList();

                string _HistoriaFamiliar = string.Empty;
                foreach (var item in _histfamil)
                {
                    _HistoriaFamiliar += _HistoriaFamiliar.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                    _HistoriaFamiliar += item.DescricaoRelatorio;
                }

                _HistoriaFamiliar += _HistoriaFamiliar.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;

                string _HistoriaFamiliarNG = string.Empty;
                foreach (var item in _SumarioAvaliacaoMedica.HistoriaFamiliar.ItensHistoriaFamiliar.Where(x => x.SemParticularidades == true).OrderBy(x => x.Descricao))
                {
                    _HistoriaFamiliarNG += item.Descricao.Combine(", ");
                }

                if (_HistoriaFamiliarNG.IsNotEmptyOrWhiteSpace())
                    _HistoriaFamiliarNG = "Nega: ".Combine(_HistoriaFamiliarNG.Trim().Remove(_HistoriaFamiliarNG.ToString().Trim().Length - 1));

                _HistoriaFamiliar += _HistoriaFamiliarNG;
                #endregion HistoriaFamiliar

                #region PerfilPsicoSocial
                IList<ITEM_REL> _PerfPsico = (from T in _SumarioAvaliacaoMedica.PerfilPsicoSocial.ItensPerfilPsicoSocialSemParticularidades
                                              where T.SemParticularidades == false &&
                                              string.IsNullOrEmpty(T.Observacoes) == false
                                              orderby T.Ordem
                                              select new ITEM_REL { DescricaoRelatorio = (T.Descricao.Combine(": ").Combine(T.Observacoes)) }).ToList()
                                             .Union(
                                              from T in _SumarioAvaliacaoMedica.PerfilPsicoSocial.ItensPerfilPsicoSocialNega
                                              where T.SemParticularidades == false && string.IsNullOrEmpty(T.Observacoes) == false
                                              orderby T.Ordem
                                              select new ITEM_REL { DescricaoRelatorio = (T.Descricao.Combine(": ").Combine(T.Observacoes)) }).ToList();

                string _PerfilPsicoSocial = string.Empty;
                foreach (var item in _PerfPsico)
                {
                    _PerfilPsicoSocial += _PerfilPsicoSocial.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                    _PerfilPsicoSocial += item.DescricaoRelatorio;
                }

                _PerfilPsicoSocial += _PerfilPsicoSocial.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;

                string _PerfilPsicoSocialSP = string.Empty;
                foreach (var item in _SumarioAvaliacaoMedica.PerfilPsicoSocial.ItensPerfilPsicoSocialSemParticularidades.Where(x => x.SemParticularidades == true).OrderBy(x => x.Descricao))
                {
                    _PerfilPsicoSocialSP += item.Descricao.Combine(", ");
                }

                string _PerfilPsicoSocialNG = string.Empty;
                foreach (var item in _SumarioAvaliacaoMedica.PerfilPsicoSocial.ItensPerfilPsicoSocialNega.Where(x => x.SemParticularidades == true).OrderBy(x => x.Descricao))
                {
                    _PerfilPsicoSocialNG += item.Descricao.Combine(", ");
                }

                if (_PerfilPsicoSocialSP.IsNotEmptyOrWhiteSpace())
                    _PerfilPsicoSocialSP = "Sem particularidades: ".Combine(_PerfilPsicoSocialSP.Trim().Remove(_PerfilPsicoSocialSP.ToString().Trim().Length - 1));

                _PerfilPsicoSocial += _PerfilPsicoSocialSP;
                _PerfilPsicoSocial += _PerfilPsicoSocial.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;

                if (_PerfilPsicoSocialNG.IsNotEmptyOrWhiteSpace())
                    _PerfilPsicoSocialNG = "Nega: ".Combine(_PerfilPsicoSocialNG.Trim().Remove(_PerfilPsicoSocialNG.ToString().Trim().Length - 1));

                _PerfilPsicoSocial += _PerfilPsicoSocialNG;
                #endregion

                #region MedicamentosEmUso

                List<samMedicamentosEmUso> _lstMedicamentosEmUso = new List<samMedicamentosEmUso>();
                    string _MedicamentosEmUsoRTF = string.Empty;
                    string _MedicamentosEmUsoNaoFazUso = string.Empty;
                if (this._SumarioAvaliacaoMedica.Paciente.IsNotNull())
                {                    
                    wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;

                    IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                    repp.OndeChaveIgual(this._SumarioAvaliacaoMedica.ID);
                    repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedica);
                    var ret = repp.List();
                    if (ret.IsNotNull())
                        _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);

                    if (_MedicamentosCollection.HasItems())
                        foreach (var item in this._SumarioAvaliacaoMedica.Paciente.MedicamentosEmUso)
                        {
                            if (_MedicamentosCollection.Count(x => x.MedicamentosEmUso.IsNotNull() && x.Chave == this._SumarioAvaliacaoMedica.ID && x.MedicamentosEmUso.ID == item.ID) > 0)
                            {
                                if (item.Medicamento == Constantes.coNaoUsaMedicamentos)
                                {
                                    _MedicamentosEmUsoRTF = Constantes.coNaoUsaMedicamentos;
                                    _MedicamentosEmUsoNaoFazUso = "<<Não faz uso de medicamentos>>";
                                }
                                else
                                {
                                    if (!_MedicamentosEmUsoRTF.IsEmpty())
                                        _MedicamentosEmUsoRTF += Environment.NewLine;
                                    _MedicamentosEmUsoRTF += (item.Medicamento.IsEmptyOrWhiteSpace() ? string.Empty : item.Medicamento.ToString()) +
                                              (item.Dose.IsEmptyOrWhiteSpace() ? string.Empty : "  Dose: " + item.Dose.ToString()) +
                                              (item.Frequencia.IsEmptyOrWhiteSpace() ? string.Empty : "  Frequência: " + item.Frequencia.ToString()) +
                                              (item.Via.IsEmptyOrWhiteSpace() ? string.Empty : "  Via: " + item.Via.ToString()) +
                                              (item.Status.IsNull() ? string.Empty : "  Status: " + item.Status.ToString());

                                    _lstMedicamentosEmUso.Add(new samMedicamentosEmUso
                                                                              {
                                                                                  Medicamento = item.Medicamento,
                                                                                  Dose = item.Dose,
                                                                                  Frequencia = item.Frequencia,
                                                                                  Status = item.Status.CustomDisplay(),
                                                                                  Via = item.Via
                                                                              });
                                }
                            }
                        }
                }
                else
                {
                    foreach (var item in this._SumarioAvaliacaoMedica.MedicamentosEmUso.ItensMedicamentosEmUso)
                    {
                        _lstMedicamentosEmUso.Add(new samMedicamentosEmUso
                        {
                            Medicamento = item.Descricao,
                            Dose = item.Dose,
                            Frequencia = item.Frequencia,                               
                            Via = item.Via
                        });
                    }
                }
                #endregion

                #region Historia Perinatal
                string _HistoriaPerinatal = string.Empty;
                string idadeGestacional = string.Empty;
                string aleitamentoMaterno = string.Empty;
                string intercorrenciaPerinatal = string.Empty;

                if (_SumarioAvaliacaoMedica.Paciente.IsNotNull())
                    if (!_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IsNull())
                    {

                        if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.InfoDisponivel.Equals(SimNao.Sim))
                            _HistoriaPerinatal = "<< Informações não disponíveis >>";
                        else
                        {

                            if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IdadeGestacionalTermo == SimNao.Sim)
                                idadeGestacional = "Idade Gestacional: A termo";
                            else if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IdadeGestacionalTermo == SimNao.Nao && _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.PrematuridadeSemanas != 0)
                                idadeGestacional = "Prematuridade: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.PrematuridadeSemanas + " semanas";

                            if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.AleitamentoMaterno == SimNao.Sim)
                                aleitamentoMaterno = "Aleitamento Marteno: Sim " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.AleitamentoMaternoMeses + " meses";
                            else if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.AleitamentoMaterno == SimNao.Nao)
                                aleitamentoMaterno = "Aleitamento Marteno: Não";

                            if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IntercorrenciaPerinatal == SimNao.Sim)
                                intercorrenciaPerinatal = "Intercorrências Perinatais: Sim " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Intercorrencia;
                            else if (_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.IntercorrenciaPerinatal == SimNao.Nao)
                                intercorrenciaPerinatal = "Intercorrências Perinatais: Não";

                            string HistoriaP = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}",

                            _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.GestacaoNormal == SimNao.Sim ? "Gestação: Normal" + Environment.NewLine : string.Empty,
                            !string.IsNullOrWhiteSpace(_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.GestacaoIntercorrencia) ? "Intercorrências: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.GestacaoIntercorrencia + Environment.NewLine : string.Empty,
                            idadeGestacional + Environment.NewLine,
                            _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.PartoVaginal == SimNao.Sim ? "Parto: Vaginal" + Environment.NewLine : _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaEletiva == SimNao.Sim ? "Parto: Cesariana Eletiva" + Environment.NewLine : _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaUrgencia == SimNao.Sim ? "Parto: Cesariana Urgência" + Environment.NewLine : string.Empty,
                            !string.IsNullOrWhiteSpace(_SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaUrgenciaMotivo) ? "Motivo: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.CesarianaUrgenciaMotivo : string.Empty,
                            _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Peso != null ? "Peso: " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Peso + " g" + Environment.NewLine : string.Empty,
                            _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Comprimento != null ? "Comprimento : " + _SumarioAvaliacaoMedica.Paciente.HistoriaPerinatal.Comprimento + " cm" + Environment.NewLine : string.Empty,
                            intercorrenciaPerinatal + Environment.NewLine,
                            aleitamentoMaterno);

                            _HistoriaPerinatal = HistoriaP;
                        }
                    }
                //List<samHistoriaPerinatal> _histPerin = new List<samHistoriaPerinatal>();
                //try
                //{
                //    if (!_HistoriaPerinatal.IsEmptyOrWhiteSpace())
                //    {
                //        _histPerin.Add(new samHistoriaPerinatal()
                //        {
                //            Descricao = _HistoriaPerinatal.ToString()
                //        });
                //    }
                //}
                #endregion

                List<samAnamneseCompleto> qry = new List<samAnamneseCompleto>();
                if (this._SumarioAvaliacaoMedica.IsNotNull())
                {
                    qry.Add(new samAnamneseCompleto()
                    {
                        MotivoInternacao = this._SumarioAvaliacaoMedica.MotivoInternacao.IsNotEmptyOrWhiteSpace() ? this._SumarioAvaliacaoMedica.MotivoInternacao : string.Empty,
                        HistoriaDoencaAtual = this._SumarioAvaliacaoMedica.HistoriaDoencaAtual.IsNotEmptyOrWhiteSpace() ? this._SumarioAvaliacaoMedica.HistoriaDoencaAtual : string.Empty,

                        HistoriaPerinatal = _HistoriaPerinatal,

                        Imunizacoes = _lstImunizacoes,
                        ImunizacoesRTF = _Imunizacoes,

                        OutrasVacinas = _lstOutrasVacinas,
                        OutrasVacinasRTF = _OutrasVacinas,

                        Alergias = _lstAlergias,
                        AlergiasRTF = _AlergiasRTF,

                        RevisaoDeSistemas = _RevisaoDeSistemas,
                        RevisaoDeSistemasOutros = this._SumarioAvaliacaoMedica.RevisaoDeSistemas.IsNull() ? string.Empty : (this._SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros.IsEmptyOrWhiteSpace() ? string.Empty : this._SumarioAvaliacaoMedica.RevisaoDeSistemas.Outros),

                        HistoriaPregressa = _HistoriaPregressa,
                        HistoriaPregressaOutros = this._SumarioAvaliacaoMedica.HistoriaPregressa.IsNull() ? string.Empty : (this._SumarioAvaliacaoMedica.HistoriaPregressa.Outros.IsEmptyOrWhiteSpace() ? string.Empty : this._SumarioAvaliacaoMedica.HistoriaPregressa.Outros),

                        HistoriaFamiliar = _HistoriaFamiliar,
                        HistoriaFamiliarOutros = this._SumarioAvaliacaoMedica.HistoriaFamiliar.IsNull() ? string.Empty : (this._SumarioAvaliacaoMedica.HistoriaFamiliar.Outros.IsEmptyOrWhiteSpace() ? string.Empty : this._SumarioAvaliacaoMedica.HistoriaFamiliar.Outros),

                        PerfilPsicoSocial = _PerfilPsicoSocial,
                        PerfilPsicoSocialOutros = this._SumarioAvaliacaoMedica.PerfilPsicoSocial.IsNull() ? string.Empty : (this._SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros.IsEmptyOrWhiteSpace() ? string.Empty : this._SumarioAvaliacaoMedica.PerfilPsicoSocial.Outros),

                        MedicamentosEmUso = _lstMedicamentosEmUso,
                        MedicamentosEmUsoRTF = _MedicamentosEmUsoRTF,
                        MedicamentosEmUsoNaoFazUso = _MedicamentosEmUsoNaoFazUso
                    });
                }
                return qry;
            }
        }
        #endregion

        #region Exames Realizados
        public class samExamesRealizados
        {
            public string ExamesRealizados { get; set; }
            public string ExamesRealizadosNega { get; set; }
        }

        public List<samExamesRealizados> srLExamesRealizados
        {
            get
            {
                string _ExamesRealizados = string.Empty;
                _ExamesRealizados = this._SumarioAvaliacaoMedica.ExamesRealizados.Descricao;

                List<samExamesRealizados> qry = new List<samExamesRealizados>();
                if (this._SumarioAvaliacaoMedica.ExamesRealizados.IsNotNull())
                {
                    if (_ExamesRealizados.IsNotNull() || _SumarioAvaliacaoMedica.ExamesRealizados.NaoForamRealizadosExames.IsNotNull())
                        qry.Add(new samExamesRealizados()
                        {
                            ExamesRealizados = _ExamesRealizados,
                            ExamesRealizadosNega = _SumarioAvaliacaoMedica.ExamesRealizados.NaoForamRealizadosExames.Equals(SimNao.Sim) ? "<<Não Foram Realizados Exames>>" : string.Empty
                        });
                }
                return qry;
            }
        }
        #endregion

        #region Diagnostico
        public class samDiagnosticos
        {
            public string CidPrincipal { get; set; }
            public string OutrosCids { get; set; }
            public string HipotesesDiagnosticas { get; set; }
        }

        public List<samDiagnosticos> srLDiagnosticos
        {
            get
            {
                string _CidPrincipal = string.Empty;
                if (this._SumarioAvaliacaoMedica.Atendimento.IsNotNull())
                    _CidPrincipal = this._SumarioAvaliacaoMedica.Atendimento.Cid.IsNotNull() ? this._SumarioAvaliacaoMedica.Atendimento.Cid.Id.Combine(" - ").Combine(this._SumarioAvaliacaoMedica.Atendimento.Cid.Descricao) : string.Empty;

                string _OutrosCids = string.Empty;
                if (this._SumarioAvaliacaoMedica.Diagnosticos.IsNotNull())
                    foreach (var item in this._SumarioAvaliacaoMedica.Diagnosticos)
                    {
                        if (item.IsNotNull())
                        {
                            _OutrosCids += _OutrosCids.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                            _OutrosCids += item.Cid.IsNotNull() ? item.Cid.Descricao.Combine(" - ").Combine(item.Cid.Id).Combine(" - ").Combine(item.Complemento) : string.Empty;
                        }
                    }

                string _HipotesesDiagnosticas = string.Empty;
                if (this._SumarioAvaliacaoMedica.Hipoteses.IsNotNull())
                    foreach (var item in this._SumarioAvaliacaoMedica.Hipoteses)
                    {
                        if (item.IsNotNull())
                        {
                            _HipotesesDiagnosticas += _HipotesesDiagnosticas.IsNotEmptyOrWhiteSpace() ? Environment.NewLine : string.Empty;
                            _HipotesesDiagnosticas += item.Complemento;
                        }
                    }

                List<samDiagnosticos> qry = new List<samDiagnosticos>();
                qry.Add(new samDiagnosticos()
                {
                    CidPrincipal = _CidPrincipal,
                    OutrosCids = _OutrosCids,
                    HipotesesDiagnosticas = _HipotesesDiagnosticas
                });
                return qry;
            }
        }

        #endregion

        #region ExameFisico
        public class samExameFisico
        {
            public string Descricao { get; set; }
            public string Observacao { get; set; }
            public string Outros { get; set; }
        }

        public List<samExameFisico> srLExameFisico
        {
            get
            {
                #region Exames
                string exames = string.Empty;
                exames = string.Format(@"PA: {0}/{1} mmHg   FC: {2} bpm  FR: {3} mpm  Tax: {4} °C  SAT: {12}  Peso: {5} Kg  Altura: {6} cm  SC: {7}  IMC: {8}  Estado Geral: {9}  Mucosa: {10}{11}",
                                            this._SumarioAvaliacaoMedica.ExameFisico.PressaoArterial == null ? 0 : this._SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Alta == null ? 0 : this._SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Alta,
                                            this._SumarioAvaliacaoMedica.ExameFisico.PressaoArterial == null ? 0 : this._SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Baixa == null ? 0 : this._SumarioAvaliacaoMedica.ExameFisico.PressaoArterial.Baixa,
                                            this._SumarioAvaliacaoMedica.ExameFisico.FrequenciaCardiaca,
                                            this._SumarioAvaliacaoMedica.ExameFisico.FrequenciaRespiratoria,
                                            this._SumarioAvaliacaoMedica.ExameFisico.TemperaturaAxila == null ? 0 : this._SumarioAvaliacaoMedica.ExameFisico.TemperaturaAxila.Value,
                                            this._SumarioAvaliacaoMedica.ExameFisico.Peso == null ? "0" : this._SumarioAvaliacaoMedica.ExameFisico.Peso == 0 ? "-" : this._SumarioAvaliacaoMedica.ExameFisico.Peso.Value.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.Altura.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.SC.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.IMC.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.EstadoGeral == null ? "0" : this._SumarioAvaliacaoMedica.ExameFisico.EstadoGeral.Value.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.MucosaEstado == null ? "" : !this._SumarioAvaliacaoMedica.ExameFisico.MucosaSituacao.IsNull() ? this._SumarioAvaliacaoMedica.ExameFisico.MucosaEstado.Value.ToString().CombineNotEmpty("/") : this._SumarioAvaliacaoMedica.ExameFisico.MucosaEstado.Value.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.MucosaSituacao == null ? " " : this._SumarioAvaliacaoMedica.ExameFisico.MucosaSituacao.Value.ToString(),
                                            this._SumarioAvaliacaoMedica.ExameFisico.Saturacao == null ? 0 : this._SumarioAvaliacaoMedica.ExameFisico.Saturacao);
                #endregion

                #region retira zerados!

                exames = exames.Replace("PA: 0/0 mmHg   ", "")
                                         .Replace("FC: 0 bpm  ", "")
                                         .Replace("FR: 0 mpm  ", "")
                                         .Replace("Tax: 0 °C  ", "")
                                         .Replace("SAT: 0  ", "")
                                         .Replace("Peso: - Kg  ", "")
                                         .Replace("Peso: 0 Kg  ", "")
                                         .Replace("Altura: 0 cm  ", "")
                                         .Replace("SC: 0  ", "")
                                         .Replace("IMC: 0  ", "")
                                         .Replace("Estado Geral: 0  ", "")
                                         .Replace("Mucosa:  ", "");
                #endregion

                #region Itens com Observação, Sem Particularidades e Não Avaliados.
                string obsExameFisico = "";
                string ItensObservacao = "";
                string ItensSemParticularidade = "";
                string ItensNaoAvaliados = "";

                #region Itens com Observação
                var Ts = (from T in this._SumarioAvaliacaoMedica.ExameFisico.ItensExameFisico
                          where T.SemParticularidades == false &&
                          !string.IsNullOrWhiteSpace(T.Observacoes)
                          orderby T.Ordem
                          select new ITEM_REL { DescricaoRelatorio = (T.Descricao + ": " + (string.IsNullOrWhiteSpace(T.Observacoes) ? (T.NaoAvaliado == true ? "Não Avaliado" : "Nega") : T.Observacoes)), Ordem = T.Ordem }).ToList();
                foreach (var item in Ts.OrderBy(x => x.Ordem))
                {
                    if (ItensObservacao.IsEmpty())
                        ItensObservacao = item.DescricaoRelatorio;
                    else
                        ItensObservacao += ", " + item.DescricaoRelatorio;
                }
                #endregion

                #region Sem Particularidade
                Ts = (from T in this._SumarioAvaliacaoMedica.ExameFisico.ItensExameFisico
                      where T.SemParticularidades == true
                      orderby T.Ordem
                      select new ITEM_REL { DescricaoRelatorio = (T.Descricao), Ordem = T.Ordem }).ToList();
                foreach (var item in Ts.OrderBy(x => x.Ordem))
                {
                    if (ItensSemParticularidade.IsEmpty())
                        ItensSemParticularidade = "Sem Particularidade: " + item.DescricaoRelatorio;
                    else
                        ItensSemParticularidade += ", " + item.DescricaoRelatorio;
                }
                #endregion

                #region Não Avaliados
                Ts = (from T in this._SumarioAvaliacaoMedica.ExameFisico.ItensExameFisico
                      where T.NaoAvaliado == true
                      orderby T.Ordem
                      select new ITEM_REL { DescricaoRelatorio = (T.Descricao), Ordem = T.Ordem }).ToList();
                foreach (var item in Ts.OrderBy(x => x.Ordem))
                {
                    if (ItensNaoAvaliados.IsEmpty())
                        ItensNaoAvaliados = "Não Avaliado: " + item.DescricaoRelatorio;
                    else
                        ItensNaoAvaliados += ", " + item.DescricaoRelatorio;
                }
                #endregion

                obsExameFisico += (ItensObservacao.IsEmpty()) ? string.Empty : ItensObservacao;

                obsExameFisico += (!obsExameFisico.IsEmpty() && !ItensSemParticularidade.IsEmpty()) ? Environment.NewLine + ItensSemParticularidade : ItensSemParticularidade;

                obsExameFisico += (!obsExameFisico.IsEmpty() && !ItensNaoAvaliados.IsEmpty()) ? Environment.NewLine + ItensNaoAvaliados : ItensNaoAvaliados;

                obsExameFisico += (!obsExameFisico.IsEmpty() && !this._SumarioAvaliacaoMedica.ExameFisico.Observacoes.IsEmpty()) ? Environment.NewLine + this._SumarioAvaliacaoMedica.ExameFisico.Observacoes : this._SumarioAvaliacaoMedica.ExameFisico.Observacoes;

                #endregion

                string _Outros = string.Empty;
                _Outros = this._SumarioAvaliacaoMedica.ExameFisico.Outros;

                List<samExameFisico> qry = new List<samExameFisico>();

                if ((!exames.IsEmpty()) || (!obsExameFisico.IsEmpty()))
                {
                    qry.Add(new samExameFisico()
                    {
                        Descricao = exames,
                        Observacao = obsExameFisico,
                        Outros = _Outros
                    });
                }
                return qry;
            }
        }

        #endregion

        #region Assinatura
        public class samAssinatura
        {
            public string Assinatura { get; set; }
        }

        public List<samAssinatura> srLAssinatura
        {
            get
            {
                var usu = new wrpUsuarios(this._SumarioAvaliacaoMedica.Usuario);
                List<samAssinatura> qry = new List<samAssinatura>();
                qry.Add(new samAssinatura() { Assinatura = usu.AssinaturaPadrao(pData:DateTime.Now)});
                return qry;                
            }
        }
        #endregion

        #region Rodapé

        public class samRodape
        {
            public string NomePaciente { get; set; }
            public string NomeResumo { get; set; }
            public int IDPaciente { get; set; }
            public string NomePrestador { get; set; }
            public string Registro { get; set; }
            public string CodigoBarras { get; set; }

            public bool MostraCodigoBarras { get; set; }
            public bool MostraIDPaciente { get; set; }
        }

        public List<samRodape> srLRodape()
        {
            List<samRodape> lista = new List<samRodape>();
            samRodape rodape = new samRodape();
            rodape.MostraCodigoBarras = false;
            rodape.MostraIDPaciente = false;

            // 3 - Sumário Adulto Internado e 4 - Sumário Pediátrico
            if (this._SumarioAvaliacaoMedica.Atendimento.IsNotNull() && this._SumarioAvaliacaoMedica.Tipo.ID == 3 || this._SumarioAvaliacaoMedica.Tipo.ID == 4)
            {
                if (this._SumarioAvaliacaoMedica.Paciente.IsNotNull())
                {
                    rodape.NomePaciente = this._SumarioAvaliacaoMedica.Paciente.Nome;
                    rodape.IDPaciente = this._SumarioAvaliacaoMedica.Paciente.ID;
                    rodape.MostraIDPaciente = true;
                }

                rodape.NomeResumo = this._SumarioAvaliacaoMedica.Atendimento.Leito.IsNotNull() ? this._SumarioAvaliacaoMedica.Atendimento.Leito.Descricao : string.Empty;
                rodape.CodigoBarras = this._SumarioAvaliacaoMedica.Atendimento.ID.ToString();
                rodape.MostraCodigoBarras = true;

                if (this._SumarioAvaliacaoMedica.Atendimento.Prestador.IsNotNull())
                {
                    rodape.NomePrestador = this._SumarioAvaliacaoMedica.Atendimento.Prestador.Nome;
                    rodape.Registro = this._SumarioAvaliacaoMedica.Atendimento.Prestador.Registro;
                }
            }
            else if (this._SumarioAvaliacaoMedica.Tipo.ID == 3 || this._SumarioAvaliacaoMedica.Tipo.ID == 4)
            {
                rodape.NomePaciente = this._SumarioAvaliacaoMedica.NomePaciente;
            }

            lista.Add(rodape);

            return lista;
        }

        #endregion

        #endregion
    }
}
