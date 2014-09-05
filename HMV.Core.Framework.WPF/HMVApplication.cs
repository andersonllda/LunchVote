using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Threading;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Extensions;
using HMV.Core.Interfaces;
using NHibernate;
using OmarALZabir.AspectF;
using StructureMap;
using System.Data.OracleClient;
using System.Configuration;
using System.Printing;
using NHibernate.Exceptions;

namespace HMV.Core.Framework.WPF
{
    public class HMVApplication : Application
    {
        private string _nomeErro = string.Empty;
        private int _countErro = 0;

        public HMVApplication()
        {
            DispatcherUnhandledException += new System.Windows.Threading.DispatcherUnhandledExceptionEventHandler(HMVApplication_DispatcherUnhandledException);
        }

        void HMVApplication_DispatcherUnhandledException(object sender1, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            if (System.Windows.Application.Current == null)
            {
                new System.Windows.Application
                {
                    ShutdownMode = ShutdownMode.OnExplicitShutdown
                };
            }

            #region --- Exception para exibir a msg na tela ---
            if (e.Exception.GetType() == typeof(BusinessMsgException))
            {
                Dictionary<MessageImage, MessageBoxImage> messageBoxImage = new Dictionary<MessageImage, MessageBoxImage>();
                messageBoxImage.Add(MessageImage.Asterisk, MessageBoxImage.Asterisk);
                messageBoxImage.Add(MessageImage.Error, MessageBoxImage.Error);
                messageBoxImage.Add(MessageImage.Exclamation, MessageBoxImage.Exclamation);
                messageBoxImage.Add(MessageImage.Hand, MessageBoxImage.Hand);
                messageBoxImage.Add(MessageImage.Information, MessageBoxImage.Information);
                messageBoxImage.Add(MessageImage.None, MessageBoxImage.None);
                messageBoxImage.Add(MessageImage.Question, MessageBoxImage.Question);
                messageBoxImage.Add(MessageImage.Stop, MessageBoxImage.Stop);
                messageBoxImage.Add(MessageImage.Warning, MessageBoxImage.Warning);

                BusinessMsgException exp = e.Exception as BusinessMsgException;
                MessageBox.Show(e.Exception.Message, "Atenção", MessageBoxButton.OK, messageBoxImage[(MessageImage)exp.MessageImage]);
                return;
            }

            if (e.Exception.GetType() == typeof(ApplicationWebResponseException))
            {
                MessageBox.Show("Sistema em atualização, execute a pesquisa novamente.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (e.Exception.GetType() == typeof(BusinessValidatorException))
            {
                BusinessValidatorException exp = e.Exception as BusinessValidatorException;
                MessageBox.Show(e.Exception.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (e.Exception.GetType() == typeof(NotSupportedException) && PrinterSettings.InstalledPrinters.Count.Equals(0))
            {
                MessageBox.Show("Não há uma impressora instalada no seu computador!\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (e.Exception.InnerException.IsNotNull())
            {
                if (e.Exception.InnerException.GetType() == typeof(NotSupportedException) && PrinterSettings.InstalledPrinters.Count.Equals(0))
                {
                    MessageBox.Show("Não há uma impressora instalada no seu computador!\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (e.Exception.InnerException.GetType() == typeof(PrintQueueException))
                {
                    MessageBox.Show("Problemas ao enviar documento para a impressora!\nEntre em contato com o setor responsável e solicite a instalação do driver.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (e.Exception.InnerException.GetType() == typeof(AccessViolationException))
                {
                    MessageBox.Show("Problemas ao Iniciar uma Aplicação VB6!\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (e.Exception.InnerException.GetType() == typeof(PrintServerException))
                {
                    MessageBox.Show("Problemas ao encontrar a impressora padrão!\nSe o problema persistir entre em contato com o setor responsável.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //TRATA O ERRO DA ATUALIZACAO DO BOLETIM
                if (e.Exception.InnerException.Message.IndexOf("ORA-20999: ERRO - REVER") > 0)
                {
                    return;
                }
                if (e.Exception.InnerException.Message.IndexOf("DBAHMV.TRG_HMV_ALTA_BOLETIM") > 0)
                {
                    return;
                }

                //TRATA O ERRO DA IMPRESSORA PADRÃO
                if (e.Exception.InnerException.Message.IndexOf("HMV.Core.Framework.DevExpress.v12._1.Extensions.PrintExtension.GetDefaultPrintQueue()") > 0)
                {
                    MessageBox.Show("Problemas na impressora padrão, selecione a impressora padrão correta!\nSe o problema persistir entre em contato com o setor responsável.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (e.Exception.GetType() == typeof(PrintQueueException))
            {
                MessageBox.Show("Problemas ao enviar documento para a impressora!\nEntre em contato com o setor responsável e solicite a instalação do driver.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (e.Exception.GetType() == typeof(AccessViolationException))
            {
                MessageBox.Show("Problemas ao Iniciar uma Aplicação VB6!\nEntre em contato com o setor responsável e solicite a instalação.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (e.Exception.GetType() == typeof(PrintServerException) || e.Exception.Message == "No printer has been found on the machine")
            {
                MessageBox.Show("Problemas ao encontrar a impressora padrão!\nSe o problema persistir entre em contato com o setor responsável.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //TRATA O ERRO DA ATUALIZACAO DO BOLETIM
            if (e.Exception.Source == "NHibernate" || e.Exception.Source == "mscorlib")
                if (e.Exception.Message.IndexOf("could not update: [HMV.Core.Domain.Model.BoletimDeEmergencia#") >= 0)
                    return;

            //TRATA O ERRO DO DxDocking para a aplicação não fechar.
            if (e.Exception.Source == "DevExpress.Xpf.Docking.v11.1")
                if (e.Exception.StackTrace.IndexOf("DevExpress.Xpf.Docking.ClosedItemsPanel") >= 0)
                    return;

            //TRATA O ERRO DO DxDocking para a aplicação não fechar.
            if (e.Exception.Source == "DevExpress.Xpf.Docking.v12.1")
                if (e.Exception.StackTrace.IndexOf("DevExpress.Xpf.Docking.ClosedItemsPanel") >= 0
                    || e.Exception.StackTrace.IndexOf("DevExpress.Xpf.Docking.DockLayoutManager.Update()") >= 0
                    || e.Exception.StackTrace.IndexOf("DevExpress.Xpf.Docking.Platform.MouseEventSubscriber") >= 0)
                    return;

            if (e.Exception.Source == "PresentationFramework")
            {
                //Trata o erro do fechar ao ficar janela aberta
                if (e.Exception.StackTrace.IndexOf("System.Windows.Window.VerifyNotClosing()") >= 0)
                    return;

                //Trata o erro da chamado do compoente externo.
                if (e.Exception.StackTrace.IndexOf("System.Windows.Markup.XamlReader.RewrapException(Exception e, Uri baseUri)") >= 0)
                {
                    MessageBox.Show("ERRO!" + Environment.NewLine + "Houve um problema ao chamar um componente externo, o sistema será fechado, por favor tente novamente.\nSe o problema persistir entre em contato com o setor responsável."
                    + Environment.NewLine + Environment.NewLine + e.Exception.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown(-1);
                    return;
                }
            }

            //Verifica se tem rede.
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("ERRO!" + Environment.NewLine + "Houve um problema de conexão de rede, contate o suporte."
                    + Environment.NewLine + Environment.NewLine + e.Exception.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(-1);
                return;
            }

            //Verifica se foi uma excecao do ORACLE.
            if (!e.Exception.InnerException.IsNull())
            {
                OracleException oraex = null;

                if (e.Exception.InnerException.GetType() == typeof(OracleException))
                    oraex = (OracleException)e.Exception.InnerException;
                else if (e.Exception.InnerException.GetType() == typeof(GenericADOException))
                {
                    GenericADOException gen = (GenericADOException)e.Exception.InnerException;
                    oraex = (OracleException)gen.InnerException;
                }
                else if (e.Exception.InnerException.GetType() == typeof(ADOException))
                {
                    ADOException gen = (ADOException)e.Exception.InnerException;
                    oraex = (OracleException)gen.InnerException;
                }

                if (oraex.IsNotNull())
                {
                    // Códigos de erros quando perde a sessão do oracle por OCIOSIDADE OU por TIMEOUT OU SEM CONEXÃO.
                    if (oraex.Code == 3135 || oraex.Code == 12170 || oraex.Code == 3114 || oraex.Code == 03135)
                    {
                        MessageBox.Show("O sistema ficou muito tempo ocioso e por segurança a conexão com o banco de dados foi fechada, por favor abra novamente o sistema."
                            + Environment.NewLine + Environment.NewLine + e.Exception.InnerException.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown(-1);
                        return;
                    }
                    // Código de erro quando perde a sessão do oracle por cair a REDE e voltar.
                    if (oraex.Code == 12571 || oraex.Code == 12537 || oraex.Code == 12545 || oraex.Code == 12505)
                    {
                        MessageBox.Show("Ocorreu uma falha na REDE e por segurança a conexão com o banco de dados foi fechada, por favor abra novamente o sistema e repita a operação."
                            + Environment.NewLine + Environment.NewLine + e.Exception.InnerException.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown(-1);
                        return;
                    }
                }
            }

            //Verifica se ouve erro ao chamar tela do VB ou componente externo
            if (e.Exception.Message == "BuildWindowCore failed to return the hosted child window handle."
                || e.Exception.Message == "Falha de BuildWindowCore ao retornar o identificador da janela filho hospedada."
                || e.Exception.Message == "O identificador da janela é inválido")
            {
                MessageBox.Show("Ocorreu um erro ao abrir um componente externo do PEP, feche a tela principal e tente novamente!\nSe o problema persistir entre em contato com o setor responsável.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (e.Exception.Message == "The net printer is unavailable."
                || e.Exception.Message == "Printing was canceled. Win32 error: O servidor RPC não está disponível."
                || e.Exception.Message == "The printer name is invalid. Please check the printer settings.")
            {
                MessageBox.Show("Ocorreu um erro na impressão, tente novamente!\nSe o problema persistir entre em contato com o setor responsável.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
            #endregion

            ObjectFactory.GetInstance<ILogger>().LogException(e.Exception);
            try
            {
                if (this._nomeErro.IsEmpty())
                    this._nomeErro = new Random().Next().ToString();
                else
                    this._countErro++;

                string nomeArquivo = "Erro_" + this._nomeErro + "_" + _countErro;
                string nomeArquivoImg = ConfigurationManager.AppSettings["PathImgErro"] + nomeArquivo + ".png";
                string nomeArquivoTXT = ConfigurationManager.AppSettings["PathImgErro"] + nomeArquivo + ".txt";

                #region --- Envia o email com os arquivos em anexo ---
                Hashtable objHash = new Hashtable();
                objHash["TitutoMensagem"] = "ERRO: " + this._nomeErro + " | Exception: " + e.Exception.Source;
                objHash["Sistema"] = this.GetType().Assembly.GetName().Name;
                objHash["UsuarioWindows"] = Environment.UserName;
                objHash["Maquina"] = Environment.MachineName;
                objHash["Versao"] = this.GetType().Assembly.GetName().Version.ToString();
                objHash["Mensagem"] = e.Exception.Message + " mais detalhes do erro em anexo.";

                IList<string> at = new List<string>();
                at.Add(nomeArquivoImg);
                at.Add(nomeArquivoTXT);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                IEmailService emailService = ObjectFactory.GetInstance<IEmailService>();

                #region --- Gera arquivo txt com os detalhes do erro ---
                IList<String> erros = new List<String>() {
                                                           "MESSAGE =>" + Environment.NewLine + e.Exception.Message + Environment.NewLine + "==========================="                                                        
                                                          ,"STACKTRACE =>" + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine + "==========================="
                                                          ,"INNEREXCEPTION =>" + Environment.NewLine + (e.Exception.InnerException != null ? e.Exception.InnerException.ToString() : String.Empty) + Environment.NewLine + "==========================="
                                                          ,"TARGETSITE.NAME =>" + Environment.NewLine + (e.Exception.TargetSite != null ? e.Exception.TargetSite.Name : String.Empty) + Environment.NewLine + "==========================="
                                                         };
                Helpers.ExportFileHelper.ExportStringToTXT(nomeArquivoTXT, erros.ToArray());
                #endregion

                // Thread.Sleep(7000);
                # region --- Gera imagem com o print da tela
                Helpers.ExportFileHelper.ExportPrintScreemToPng(nomeArquivoImg);
                #endregion

                emailService.SendEmail(HMV.Core.Framework.WPF.Properties.Resources.Template_Email_Erro, System.Configuration.ConfigurationManager.AppSettings["emailErro"], null, objHash, at.ToArray());

                #endregion

                string[] arquivos = Directory.GetFiles(ConfigurationManager.AppSettings["PathImgErro"], "Erro*");
                foreach (string arquivo in arquivos)
                {
                    try
                    {
                        File.Delete(arquivo);
                    }
                    // em alguns casos o arquivo que foi enviado por e-mail fica em uso.
                    catch { }
                }
            }
            // Não deve dar erro, no envio de e-mail no tratamento de erro. 
            catch { }

            MessageBox.Show("Ocorreu um erro não esperado. A Equipe de informática já foi informada." + Environment.NewLine + e.Exception.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);

            Shutdown(-1);
        }
    }
}
