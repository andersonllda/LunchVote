using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using HMV.Core.Framework.Exception;
using System.Text.RegularExpressions;

namespace HMV.Core.Framework.Helper
{
    public static class EmailHelper
    {
        public static void SendEmail(string emailto, string subjecttext, string bodyText)
        {
            SendEmail(emailto, subjecttext, bodyText, null);
        }
        
        public static void SendEmail(string emailto, string subjecttext, string bodyText, params string[] arquivos)
        {
            MailMessage mailmessage = new MailMessage();

            try
            {
                mailmessage.From = new MailAddress("no-reply@hmv.org.br", "Hospital Moinhos de Vento");
                string[] destinatarios = emailto.Split(';');

                if (destinatarios.Length > 0)
                {
                    foreach (string destino in destinatarios)
                    {
                        mailmessage.To.Add(destino);
                    }
                }
                else
                    mailmessage.To.Add(emailto);


                mailmessage.Subject = subjecttext;
                mailmessage.Body = bodyText;
                mailmessage.BodyEncoding = Encoding.UTF8;
                mailmessage.IsBodyHtml = true;

                if (arquivos != null )
                {
                    foreach (var item in arquivos)
                        mailmessage.Attachments.Add(new Attachment(item));
                }

                SmtpClient smtp = new SmtpClient();

                smtp.Send(mailmessage);
                
            }
            catch (System.Exception ex)
            {
                throw new EmailException(ex.Message);
            }
        }

        public static void SenderEmail(string rementente,string emailto, string subjecttext, string bodyText,params string[] arquivos)        
        {
            
            MailMessage mailmessage = new MailMessage();

            try
            {
                mailmessage.From = new MailAddress(rementente, rementente);
                string[] destinatarios = emailto.Split(';');

                if (destinatarios.Length > 0)
                {
                    foreach (string destino in destinatarios)
                    {
                        mailmessage.To.Add(destino);
                    }
                }
                else
                    mailmessage.To.Add(emailto);


                mailmessage.Subject = subjecttext;
                mailmessage.Body = bodyText;
                mailmessage.BodyEncoding = Encoding.UTF8;
                mailmessage.IsBodyHtml = true;

                if (arquivos != null)
                {
                    foreach (var item in arquivos)
                        mailmessage.Attachments.Add(new Attachment(item));
                }

                SmtpClient smtp = new SmtpClient();

                smtp.Send(mailmessage);

            }
            catch (System.Exception ex)
            {
                throw new EmailException(ex.Message);
            }
        
        }

        // Método que retorna verdadeiro se o e-mail for válido e falso caso não.
        public static Boolean ValidaEmail(string email)
        {

            // Expressão regular que vai validar os e-mails
            string emailRegex = @"^(([^<>()[\]\\.,;áàãâäéèêëíìîïóòõôöúùûüç:\s@\""]+"
            + @"(\.[^<>()[\]\\.,;áàãâäéèêëíìîïóòõôöúùûüç:\s@\""]+)*)|(\"".+\""))@"
            + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|"
            + @"(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

            // Instância da classe Regex, passando como 
            // argumento sua Expressão Regular 
            Regex rx = new Regex(emailRegex);

            // Método IsMatch da classe Regex que retorna
            // verdadeiro caso o e-mail passado estiver
            // dentro das regras da sua regex.
            return rx.IsMatch(email);
        }

    }
}
