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
using System.Reflection;

namespace HMV.Core.Framework.WPF
{
    public static class HMVEmail
    {
        /// <summary>
        /// Envia um e-mail com o template de erro, informando os dados da aplicação e do erro. 
        /// </summary>
        /// <param name="pAssembly">this.GetType().Assembly</param>
        /// <param name="pErro">Exception</param>
        public static void SendErro(System.Exception pErro) { 
            HMVEmail.SendErro(pErro, null);
        }

        public static void SendErro(System.Exception pErro, Assembly pAssembly)
        {
            string _nomeErro = string.Empty;
            int _countErro = 0;

            //ObjectFactory.GetInstance<ILogger>().LogException(pErro);
            try
            {
                if (_nomeErro.IsEmpty())
                    _nomeErro = new Random().Next().ToString();
                else
                    _countErro++;

                string nomeArquivo = "Erro_" + _nomeErro + "_" + _countErro;
                string nomeArquivoImg = ConfigurationManager.AppSettings["PathImgErro"] + nomeArquivo + ".png";
                string nomeArquivoTXT = ConfigurationManager.AppSettings["PathImgErro"] + nomeArquivo + ".txt";

                #region --- Envia o email com os arquivos em anexo ---
                Hashtable objHash = new Hashtable();
                objHash["TitutoMensagem"] = "ERRO: " + _nomeErro + " | Exception: " + pErro.Source;
                objHash["Sistema"] = pAssembly.GetName().Name;
                objHash["UsuarioWindows"] = Environment.UserName;
                objHash["Maquina"] = Environment.MachineName;
                objHash["Versao"] = pAssembly.GetName().Version.ToString();
                objHash["Mensagem"] = pErro.Message + " mais detalhes do erro em anexo.";

                IList<string> at = new List<string>();
                at.Add(nomeArquivoImg);
                at.Add(nomeArquivoTXT);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                IEmailService emailService = ObjectFactory.GetInstance<IEmailService>();

                #region --- Gera arquivo txt com os detalhes do erro ---
                IList<String> erros = new List<String>() {
                                                           "MESSAGE =>" + Environment.NewLine + pErro.Message + Environment.NewLine + "==========================="
                                                          ,"STACKTRACE =>" + Environment.NewLine + pErro.StackTrace + Environment.NewLine + "==========================="
                                                          ,"INNEREXCEPTION =>" + Environment.NewLine + (pErro.InnerException != null ? pErro.InnerException.ToString() : String.Empty) + Environment.NewLine + "==========================="
                                                          ,"TARGETSITE.NAME =>" + Environment.NewLine + (pErro.TargetSite != null ? pErro.TargetSite.Name : String.Empty) + Environment.NewLine + "==========================="
                                                         };
                Helpers.ExportFileHelper.ExportStringToTXT(nomeArquivoTXT, erros.ToArray());
                #endregion

                // Thread.Sleep(7000);
                # region --- Gera imagem com o print da tela
                Helpers.ExportFileHelper.ExportPrintScreemToPng(nomeArquivoImg);
                #endregion

                emailService.SendEmail(HMV.Core.Framework.WPF.Properties.Resources.Template_Email_Erro, System.Configuration.ConfigurationManager.AppSettings["emailErroTratado"], null, objHash, at.ToArray());

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
        }
    }
}
