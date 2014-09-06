using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using HMV.Core.Interfaces;
using System.Configuration;
using System.Collections;
using HMV.PEP.Test.SendEmail;

namespace HMV.PEP.Test
{
    [TestClass]
    public class EmailTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

         [TestMethod]
        public void envia_email() {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            SendEmailServiceClient proxy = new SendEmailServiceClient();
              proxy.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["User"];
              proxy.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["Password"];

              try
              {
                  proxy.Send("rodrigo.zimmer@hmv.org.br", "teste_wcf", "teste_wcf");
                  proxy.Close();
                  Assert.IsTrue(true);
              }
              catch (Exception ex)
              {
                  proxy.Abort();
                  throw new EmailException(ex.Message);
              }
        }

         [TestMethod]
         public void testa_envio_de_email_via_wcf()
         {
             Hashtable objHash = new Hashtable();
             objHash["TitutoMensagem"] = "TESTE";
             objHash["UsuarioWindows"] = Environment.UserName;
             objHash["Maquina"] = Environment.MachineName;
             objHash["Mensagem"] = " mais detalhes do erro em anexo.";

             System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
             IEmailService emailService = ObjectFactory.GetInstance<IEmailService>();

             Assert.IsTrue(emailService.SendEmail(HMV.PEP.Test.Properties.Resources.Template_Email_Erro
                 , "rodrigo.zimmer@hmv.org.br;robson.jardim@hmv.org.br"
                 , null, objHash));


         }

   /*     [TestMethod]
        public void testa_envio_de_email_wcf_com_anexo()
        {
            string nomeArquivo = "Erro_" + new Random().Next().ToString();
            string nomeArquivoImg = System.Configuration.ConfigurationManager.AppSettings["PathImgErro"] + nomeArquivo + ".png";
            string nomeArquivoTXT = System.Configuration.ConfigurationManager.AppSettings["PathImgErro"] + nomeArquivo + ".txt";

            Hashtable objHash = new Hashtable();
            objHash["TitutoMensagem"] = "TESTE";
            objHash["UsuarioWindows"] = Environment.UserName;
            objHash["Maquina"] = Environment.MachineName;
            objHash["Mensagem"] = " mais detalhes do erro em anexo.";

            IList<string> at = new List<string>();
            at.Add(nomeArquivoImg);
            at.Add(nomeArquivoTXT);

            System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            IEmailService emailService = ObjectFactory.GetInstance<IEmailService>();

            #region --- Gera arquivo txt com os detalhes do erro ---
            IList<String> erros = new List<String>() { "erro", "erro", String.Empty };
            HMV.Core.Framework.WPF.Helpers.ExportFileHelper.ExportStringToTXT(nomeArquivoTXT, erros.ToArray());
            #endregion

            # region --- Gera imagem com o print da tela
            HMV.Core.Framework.WPF.Helpers.ExportFileHelper.ExportPrintScreemToPng(nomeArquivoImg);
            #endregion

            Assert.IsTrue(emailService.SendEmail(HMV.PEP.Test.Properties.Resources.Template_Email_Erro, System.Configuration.ConfigurationManager.AppSettings["emailErro"], null, objHash, at.ToArray()));
            
        }*/


    }
}