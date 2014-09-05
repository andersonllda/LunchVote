using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.WPF;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.WPF.Report;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Helper;
using HMV.PEP.WPF.Report.SumarioAvaliacaoM;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winResumoDoProntuario.xaml
    /// </summary>
    public partial class winResumoDoProntuario : WindowBase
    {
        public winResumoDoProntuario(vmResumoDoProntuario pData)
        {
            InitializeComponent();
            this.DataContext = pData;
            dtpDtInicial.SelectedDate = DateTime.Now;
            dtpDtFinal.SelectedDate = DateTime.Now;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            //rptResumoDoProntuario report = new rptResumoDoProntuario();
            //report.DataSource = (this.DataContext as vmResumoDoProntuario).ListaSumarioDeAvaliacaoMedica.Single();
            //report.ShowPreviewDialog();
            //ucResumoAvaliacaoMedica1.report.Imprime();

            //ucRelSumarioAvaliacaoMedica1.report.Imprime();


            winRelSumarioAvaliacaoMedicaDialog win = new winRelSumarioAvaliacaoMedicaDialog();

            win.SetData((this.DataContext as vmResumoDoProntuario).SumarioDeAvalicaoSelecionado.DomainObject, false);
            win.ShowDialog(base.OwnerBase);

                
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public bool CancelClose
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        private void tblSumarios_FocusedRowChanged(object sender, DevExpress.Xpf.Grid.FocusedRowChangedEventArgs e)
        {
            //ucResumoAvaliacaoMedica1.report.DetailHisDoeAtual.Visible = false;
            //ucResumoAvaliacaoMedica1.SetData((this.DataContext as vmResumoDoProntuario).SumarioDeAvalicaoSelecionado.DomainObject);

            ucRelSumarioAvaliacaoMedica1.mostraRTF = true;
            ucRelSumarioAvaliacaoMedica1.SetData((this.DataContext as vmResumoDoProntuario).SumarioDeAvalicaoSelecionado.DomainObject);

            ucRelSumarioAvaliacaoMedica1.Visibility = Visibility.Visible;
            relRichText.Visibility = Visibility.Hidden;
        }

        private void btnPesquisarPeriodo_Click(object sender, RoutedEventArgs e)
        {
            //paciente para testes 1002861

            ucRelSumarioAvaliacaoMedica1.Visibility = Visibility.Hidden;
            relRichText.Visibility = Visibility.Visible;

            DateTime? dtinicio = null;
            DateTime? dtfim = null;

            if (chkInfPeriodo.IsChecked.Equals(true))
            {
                if ((!dtpDtInicial.SelectedDate.IsNull()) && (!dtpDtFinal.SelectedDate.IsNull()))
                {
                    dtinicio = Convert.ToDateTime(dtpDtInicial.SelectedDate.Value.ToString("dd/MM/yyyy"));
                    dtfim = Convert.ToDateTime(dtpDtFinal.SelectedDate.Value.ToString("dd/MM/yyyy" + " 23:59:59"));
                    if (dtfim < dtinicio)
                        DXMessageBox.Show("A data final não pode ser menor que a inicial", "Atenção.", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    DXMessageBox.Show("Informe o Período Inicial e Final", "Atenção.", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            (this.DataContext as vmResumoDoProntuario).Filtros(dtinicio, dtfim, new wrpUsuarios(App.Usuario));

            var _listaResumos = (this.DataContext as vmResumoDoProntuario).SumarioavaliacaoMedicaPorPeriodo;

            RTFHelper rtfResumoProntuario = new RTFHelper();
            this.relRichText.Document = rtfResumoProntuario.FlowDoc();

            foreach (var item in _listaResumos)
            {
                _SumarioAvaliacaoMedica = item.DomainObject;                
                
                switch(item.Tipo.ID)
                {
                    case 1:
                        #region rtf
                        //this.relRichText.Document = rtfResumoProntuario.FlowDoc();

                        #region Cabecalho
                        if (srLCabecalho.Count > 0)
                        {
                            rtfResumoProntuario.criaTexto("Data Abertura: " + srLCabecalho[0].DataAbertura +
                                                          "  Hora Abertura: " + srLCabecalho[0].HoraAbertura);
                            rtfResumoProntuario.criaTexto("Data Encerramento: " + srLCabecalho[0].DataEncerramento);
                            rtfResumoProntuario.criaTexto("Realizado por: " + srLCabecalho[0].RealizadoPor);

                        }
                        #endregion Cabecalho

                        #region dados pessoais
                        //if (srLDadosPaciente.Count > 0)
                        //{
                        //    rtfResumoProntuario.criaTitulo("Identificação");
                        //    rtfResumoProntuario.criaTexto("Paciente: " + srLDadosPaciente[0].Nome.ToString() +
                        //                                    ", " + srLDadosPaciente[0].Idade.ToString() +
                        //                                    ", " + srLDadosPaciente[0].Sexo.ToString() +
                        //                                    ", Fone: " + srLDadosPaciente[0].Telefone.ToString());
                        //    rtfResumoProntuario.criaTexto("Endereço: " + srLDadosPaciente[0].Endereco.ToString() +
                        //                                    ", CEP: " + srLDadosPaciente[0].CEP.ToString());
                        //    rtfResumoProntuario.criaTexto("Bairro: " + srLDadosPaciente[0].Bairro.ToString() +
                        //                                    ", Cidade: " + srLDadosPaciente[0].Cidade.ToString());
                        //    rtfResumoProntuario.criaTexto("Cor: " + srLDadosPaciente[0].Cor.ToString() +
                        //                                    ", Estado Civil: " + srLDadosPaciente[0].EstadoCivil.ToString() +
                        //                                    ", Profissão: " + srLDadosPaciente[0].Profissao.ToString() +
                        //                                    ", Registro: " + srLDadosPaciente[0].Prontuario.ToString());
                        //}
                        #endregion dados pessoais

                        #region Anamnese
                        if (!srLAnamnese.IsNull())
                        {
                            if (srLAnamnese.Count > 0)
                            {
                                rtfResumoProntuario.criaLinhaEmBranco();
                                rtfResumoProntuario.criaTitulo("Anamnese");

                                if (!srLAnamnese[0].QueixaPrincipal.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Queixa Principal");
                                    rtfResumoProntuario.criaTexto(srLAnamnese[0].QueixaPrincipal.ToString());
                                }

                                if (!srLAnamnese[0].HistoriaDoencaAtual.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("História da Doença Atual");
                                    rtfResumoProntuario.criaTexto(srLAnamnese[0].HistoriaDoencaAtual.ToString());
                                }

                                if (!srLAnamnese[0].RevisaoDeSistemas.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Revisão De Sistemas");
                                    rtfResumoProntuario.criaTexto(srLAnamnese[0].RevisaoDeSistemas.ToString());
                                }

                                if (!srLAnamnese[0].HistoriaPregressa.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("História Pregressa");
                                    rtfResumoProntuario.criaTexto(srLAnamnese[0].HistoriaPregressa.ToString());
                                }

                                if (!srLAnamnese[0].HistoriaFamiliar.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("História Familiar");
                                    rtfResumoProntuario.criaTexto(srLAnamnese[0].HistoriaFamiliar.ToString());
                                }

                                if (!srLAnamnese[0].PerfilPsicoSocial.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Perfil Psico-Social");
                                    rtfResumoProntuario.criaTexto(srLAnamnese[0].PerfilPsicoSocial.ToString());
                                }
                            }
                        }
                        #endregion Anamnese

                        #region Exame Fisico
                        if (!srLExameFisico.IsNull())
                        {
                            if (srLExameFisico.Count > 0)
                            {
                                rtfResumoProntuario.criaLinhaEmBranco();
                                rtfResumoProntuario.criaTitulo("Exame Físico");

                                if (!srLExameFisico[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaTexto(srLExameFisico[0].Descricao.ToString());
                                }
                                if (!srLExameFisico[0].Observacao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaTexto(srLExameFisico[0].Observacao.ToString());
                                }
                            }
                        }
                        #endregion Exame Fisico

                        #region Plano Diagnostico
                        if (!srLPlanoDiagnosticoTerapeutico.IsNull())
                        {
                            if (srLPlanoDiagnosticoTerapeutico.Count > 0)
                            {
                                rtfResumoProntuario.criaLinhaEmBranco();
                                rtfResumoProntuario.criaTitulo("Plano Diagnóstico/Terapêutico");

                                if (!srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Exames Solicitados");
                                    rtfResumoProntuario.criaTexto(srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.ToString());
                                }

                                if (!srLPlanoDiagnosticoTerapeutico[0].Conduta.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Conduta");
                                    rtfResumoProntuario.criaTexto(srLPlanoDiagnosticoTerapeutico[0].Conduta.ToString());
                                }
                            }
                        }
                        #endregion Plano Diagnostico 

                        #region Hipotese Diagnostico
                        if (!srLHipoteseDiagnostica.IsNull())
                        {
                            if (srLHipoteseDiagnostica.Count > 0)
                            {
                                rtfResumoProntuario.criaLinhaEmBranco();
                                rtfResumoProntuario.criaTitulo("Hipótese Diagnóstica");

                                if (!srLHipoteseDiagnostica[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaTexto(srLHipoteseDiagnostica[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Plano Diagnostico 

                        #region Notas Pessoais
                        if (!srLNotasPessoaisDoMedico.IsNull())
                        {
                            if (srLNotasPessoaisDoMedico.Count > 0)
                            {
                                if (!srLNotasPessoaisDoMedico[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaLinhaEmBranco();
                                    rtfResumoProntuario.criaTitulo("Notas Pessoais Do Médico");
                                    rtfResumoProntuario.criaTexto(srLNotasPessoaisDoMedico[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Plano Diagnostico 
                    
                        #region Assinatura
                        //if (!srLAssinatura.IsNull())
                        //{
                        //    if (srLAssinatura.Count > 0)
                        //    {
                        //        if (!srLAssinatura[0].Assinatura.IsEmptyOrWhiteSpace())
                        //        {
                        //            rtfResumoProntuario.criaLinhaEmBranco();
                        //            rtfResumoProntuario.criaLinhaEmBranco();
                        //            rtfResumoProntuario.criaLinhaEmBranco();
                        //            rtfResumoProntuario.criaTexto(srLAssinatura[0].Assinatura.ToString());
                        //        }
                        //    }
                        //}
                        #endregion Assinatura

                        #endregion
                    break;

                    case 2:
                        #region rtf
                        //this.relRichText.Document = rtfResumoProntuario.FlowDoc();

                        #region Cabecalho
                        if (srLCabecalho.Count > 0)
                        {
                            rtfResumoProntuario.criaTexto("Data Abertura: " + srLCabecalho[0].DataAbertura +
                                                          "  Hora Abertura: " + srLCabecalho[0].HoraAbertura);
                            rtfResumoProntuario.criaTexto("Data Encerramento: " + srLCabecalho[0].DataEncerramento);
                            rtfResumoProntuario.criaTexto("Realizado por: " + srLCabecalho[0].RealizadoPor);

                        }
                        #endregion Cabecalho

                        #region dados pessoais
                        //if (srLDadosPaciente.Count > 0)
                        //{
                        //    rtfResumoProntuario.criaTitulo("Identificação");
                        //    rtfResumoProntuario.criaTexto("Paciente: " + srLDadosPaciente[0].Nome.ToString() +
                        //                                   ", " + srLDadosPaciente[0].Idade.ToString() +
                        //                                   ", " + srLDadosPaciente[0].Sexo.ToString() +
                        //                                   ", Fone: " + srLDadosPaciente[0].Telefone.ToString());
                        //    rtfResumoProntuario.criaTexto("Endereço: " + srLDadosPaciente[0].Endereco.ToString() +
                        //                                  ", CEP: " + srLDadosPaciente[0].CEP.ToString());
                        //    rtfResumoProntuario.criaTexto("Bairro: " + srLDadosPaciente[0].Bairro.ToString() +
                        //                                  ", Cidade: " + srLDadosPaciente[0].Cidade.ToString());
                        //    rtfResumoProntuario.criaTexto("Cor: " + srLDadosPaciente[0].Cor.ToString() +
                        //                                  ", Estado Civil: " + srLDadosPaciente[0].EstadoCivil.ToString() +
                        //                                  ", Profissão: " + srLDadosPaciente[0].Profissao.ToString() +
                        //                                  ", Registro: " + srLDadosPaciente[0].Prontuario.ToString());
                        //}
                        #endregion dados pessoais

                        #region Subjetivo
                        if (!srLSubjetivo.IsNull())
                        {
                            if (srLSubjetivo.Count > 0)
                            {
                                if (!srLSubjetivo[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaLinhaEmBranco();
                                    rtfResumoProntuario.criaTitulo("Subjetivo");
                                    rtfResumoProntuario.criaTexto(srLSubjetivo[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Subjetivo

                        #region Objetivo
                        if (!srLObjetivo.IsNull())
                        {
                            if (srLObjetivo.Count > 0)
                            {
                                if (!srLObjetivo[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaLinhaEmBranco();
                                    rtfResumoProntuario.criaTitulo("Objetivo");
                                    rtfResumoProntuario.criaTexto(srLObjetivo[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Objetivo

                        #region Impressao
                        if (!srLImpressao.IsNull())
                        {
                            if (srLImpressao.Count > 0)
                            {
                                if (!srLImpressao[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaLinhaEmBranco();
                                    rtfResumoProntuario.criaTitulo("Impressao");
                                    rtfResumoProntuario.criaTexto(srLImpressao[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Impressao

                        #region Plano Diagnostico
                        if (!srLPlanoDiagnosticoTerapeutico.IsNull())
                        {
                            if (srLPlanoDiagnosticoTerapeutico.Count > 0)
                            {
                                rtfResumoProntuario.criaLinhaEmBranco();
                                rtfResumoProntuario.criaTitulo("Plano Diagnóstico/Terapêutico");

                                if (!srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Exames Solicitados");
                                    rtfResumoProntuario.criaTexto(srLPlanoDiagnosticoTerapeutico[0].ExamesSolicitados.ToString());
                                }

                                if (!srLPlanoDiagnosticoTerapeutico[0].Conduta.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaSubTituloSublinhado("Conduta");
                                    rtfResumoProntuario.criaTexto(srLPlanoDiagnosticoTerapeutico[0].Conduta.ToString());
                                }
                            }
                        }
                        #endregion Plano Diagnostico 

                        #region Hipotese Diagnostico
                        if (!srLHipoteseDiagnostica.IsNull())
                        {
                            if (srLHipoteseDiagnostica.Count > 0)
                            {
                                rtfResumoProntuario.criaLinhaEmBranco();
                                rtfResumoProntuario.criaTitulo("Plano Diagnóstico/Terapêutico");

                                if (!srLHipoteseDiagnostica[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaTexto(srLHipoteseDiagnostica[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Plano Diagnostico 
                    
                        #region Notas Pessoais
                        if (!srLNotasPessoaisDoMedico.IsNull())
                        {
                            if (srLNotasPessoaisDoMedico.Count > 0)
                            {
                                if (!srLNotasPessoaisDoMedico[0].Descricao.IsEmptyOrWhiteSpace())
                                {
                                    rtfResumoProntuario.criaLinhaEmBranco();
                                    rtfResumoProntuario.criaTitulo("Notas Pessoais Do Médico");
                                    rtfResumoProntuario.criaTexto(srLNotasPessoaisDoMedico[0].Descricao.ToString());
                                }
                            }
                        }
                        #endregion Plano Diagnostico 
                    
                        #region Assinatura
                        //if (!srLAssinatura.IsNull())
                        //{
                        //    if (srLAssinatura.Count > 0)
                        //    {
                        //        if (!srLAssinatura[0].Assinatura.IsEmptyOrWhiteSpace())
                        //        {
                        //            rtfResumoProntuario.criaLinhaEmBranco();
                        //            rtfResumoProntuario.criaLinhaEmBranco();
                        //            rtfResumoProntuario.criaLinhaEmBranco();
                        //            rtfResumoProntuario.criaTexto(srLAssinatura[0].Assinatura.ToString());
                        //        }
                        //    }
                        //}
                        #endregion Assinatura

                        #endregion rtf
                    break;
                }

                rtfResumoProntuario.criaLinhaEmBranco();
                rtfResumoProntuario.criaTexto("                                                     -----------------------------------------------------");
                rtfResumoProntuario.criaTexto("                                                     -----------------------------------------------------");
                rtfResumoProntuario.criaLinhaEmBranco();
            }

            if (_listaResumos.Count == 0)
            {
                rtfResumoProntuario.criaTexto("                                                     -------------------------------------------------------------------");
                rtfResumoProntuario.criaTexto("                                                     --------------NENHUM REGISTRO ENCONTRADO-------------");
                rtfResumoProntuario.criaTexto("                                                     ----------------PARA A PESQUISA INFORMADA----------------");
                rtfResumoProntuario.criaTexto("                                                     -------------------------------------------------------------------");
            }
        }

        private void grdSumarios_GotFocus(object sender, RoutedEventArgs e)
        {
            ucRelSumarioAvaliacaoMedica1.Visibility = Visibility.Visible;
            relRichText.Visibility = Visibility.Hidden;
        }

        #region RELATORIO
       
        SumarioAvaliacaoMedica _SumarioAvaliacaoMedica;

        #region Cabeçalho
        public class samCabecalho
        {
            public string DataAbertura { get; set; }
            public string HoraAbertura { get; set; }
            public string DataEncerramento { get; set; }
            public string RealizadoPor { get; set; }
        }

        public List<samCabecalho> srLCabecalho
        {
            get
            {
                List<samCabecalho> qry = new List<samCabecalho>();

                qry.Add(new samCabecalho()
                {
                    DataAbertura =     this._SumarioAvaliacaoMedica.DataProntuario.IsNull()   ? string.Empty : this._SumarioAvaliacaoMedica.DataProntuario.ToString("dd/MM/yyyy"),
                    HoraAbertura = this._SumarioAvaliacaoMedica.HoraProntuario.IsNull() ? string.Empty : this._SumarioAvaliacaoMedica.HoraProntuario,
                    DataEncerramento = this._SumarioAvaliacaoMedica.DataEncerramento.IsNull() ? string.Empty : this._SumarioAvaliacaoMedica.DataEncerramento.Value.ToString("dd/MM/yyyy"),
                    RealizadoPor = this._SumarioAvaliacaoMedica.Usuario.Prestador.Registro.ToString() + " - " + this._SumarioAvaliacaoMedica.Usuario.Prestador.Nome.ToString()
                });

                return qry;
            }
        }
        #endregion Cabeçalho

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
                Paciente pac = this._SumarioAvaliacaoMedica.Paciente;
                List<samDadosDoPaciente> qry = new List<samDadosDoPaciente>();

                qry.Add(new samDadosDoPaciente()
                {
                    Nome = pac.Nome.ToString(),
                    Sexo = pac.Sexo.ToString(),
                    Cor = pac.Cor.ToString(),
                    EstadoCivil = pac.EstadoCivil.ToString(),
                    Telefone = (pac.Telefone == null ? "" : (pac.DDDTelefone == null ? "" : pac.DDDTelefone.ToString().PadLeft(3, '0').Substring(0, 3) + " - ") + (pac.Telefone == null ? "" : pac.Telefone.ToString().PadLeft(8, '0').Substring(0, 8))),
                    Idade = pac.Idade.GetDate(this._SumarioAvaliacaoMedica.DataProntuario),
                    Profissao = pac.Profissao.IsNull() ? string.Empty : pac.Profissao.Descricao.ToString(),
                    Endereco = pac.Endereco.IsNull() ? string.Empty : pac.Endereco.ToString() + ", " + pac.Numero + " Comp:" + (pac.Complemento.IsNull() ? string.Empty : pac.Complemento),
                    Bairro = pac.Bairro.IsNull() ? string.Empty : pac.Bairro.ToString(),
                    Cidade = pac.Cidade.IsNull() ? string.Empty : pac.Cidade.Descricao.ToString(),
                    Estado = pac.Cidade.IsNull() ? string.Empty : pac.Cidade.Estado.Descricao.ToString(),
                    CEP = pac.CEP.IsNull() ? string.Empty : pac.CEP.ToString(),
                    Prontuario = pac.ID.ToString(),
                    Atendimento = this._SumarioAvaliacaoMedica.Atendimento == null ? this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.IsNull() ? string.Empty : this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.ID.ToString() : this._SumarioAvaliacaoMedica.Atendimento.ID.ToString(),
                    DataAtendimento = this._SumarioAvaliacaoMedica.Atendimento == null ? this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.IsNull() ? string.Empty : this._SumarioAvaliacaoMedica.SigaAtendimento.Atendimento.DataAtendimento.ToShortDateString() : this._SumarioAvaliacaoMedica.Atendimento.DataAtendimento.ToShortDateString()
                });

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
                    qry.Add(new samSubjetivo()
                    {
                        Descricao = this._SumarioAvaliacaoMedica.Subjetivo.IsEmpty() ? "" : this._SumarioAvaliacaoMedica.Subjetivo
                    });
                }
                return qry;
            }
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

        #region Anamnese
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

        #region ExameFisico
        public class samExameFisico
        {
            public string Descricao { get; set; }
            public string Observacao { get; set; }
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
                var Ts = (from T in this._SumarioAvaliacaoMedica.ExameFisico.ItensExameFisicoNovo
                          where T.SemParticularidades == false &&
                          !string.IsNullOrWhiteSpace(T.Observacoes)
                          orderby T.Ordem
                          select new ITEM_REL { DescricaoRelatorio = (T.Descricao + ": " + (string.IsNullOrWhiteSpace(T.Observacoes) ? (T.NaoAvaliado == true ? "Não Avaliado" : "Nega") : T.Observacoes)), Ordem = T.Ordem }).ToList();
                foreach (var item in Ts)
                {
                    if (ItensObservacao.IsEmpty())
                        ItensObservacao = item.DescricaoRelatorio;
                    else
                        ItensObservacao += ", " + item.DescricaoRelatorio;
                }
                #endregion

                #region Sem Particularidade
                Ts = (from T in this._SumarioAvaliacaoMedica.ExameFisico.ItensExameFisicoNovo
                      where T.SemParticularidades == true
                      orderby T.Ordem
                      select new ITEM_REL { DescricaoRelatorio = (T.Descricao), Ordem = T.Ordem }).ToList();
                foreach (var item in Ts)
                {
                    if (ItensSemParticularidade.IsEmpty())
                        ItensSemParticularidade = "Sem Particularidade: " + item.DescricaoRelatorio;
                    else
                        ItensSemParticularidade += ", " + item.DescricaoRelatorio;
                }
                #endregion

                #region Não Avaliados
                Ts = (from T in this._SumarioAvaliacaoMedica.ExameFisico.ItensExameFisicoNovo
                      where T.NaoAvaliado == true
                      orderby T.Ordem
                      select new ITEM_REL { DescricaoRelatorio = (T.Descricao), Ordem = T.Ordem }).ToList();
                foreach (var item in Ts)
                {
                    if (ItensNaoAvaliados.IsEmpty())
                        ItensNaoAvaliados = "Não Avaliado: " + item.DescricaoRelatorio;
                    else
                        ItensNaoAvaliados += ", " + item.DescricaoRelatorio;
                }
                #endregion

                obsExameFisico += (ItensObservacao.IsEmpty()) ? "" : ItensObservacao;

                obsExameFisico += (!obsExameFisico.IsEmpty() && !ItensSemParticularidade.IsEmpty()) ? Environment.NewLine + ItensSemParticularidade : ItensSemParticularidade;

                obsExameFisico += (!obsExameFisico.IsEmpty() && !ItensNaoAvaliados.IsEmpty()) ? Environment.NewLine + ItensNaoAvaliados : ItensNaoAvaliados;

                obsExameFisico += (!obsExameFisico.IsEmpty() && !this._SumarioAvaliacaoMedica.ExameFisico.Observacoes.IsEmpty()) ? Environment.NewLine + this._SumarioAvaliacaoMedica.ExameFisico.Observacoes : this._SumarioAvaliacaoMedica.ExameFisico.Observacoes;


                #endregion

                List<samExameFisico> qry = new List<samExameFisico>();

                if ((!exames.IsEmpty()) || (!obsExameFisico.IsEmpty()))
                {
                    qry.Add(new samExameFisico()
                    {
                        Descricao = exames,
                        Observacao = obsExameFisico
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
                qry.Add(new samAssinatura() { Assinatura = usu.Assinatura });
                return qry;               
            }
        }
        #endregion

        

        #endregion

    }
}
