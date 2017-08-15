using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class EmailBusiness
    {
        public static string EnviarEmail(string emailTo, string emailFrom, string nomeFrom, string assunto, string mensagem)
        {
            return EnviarEmail(emailTo, emailFrom, nomeFrom, assunto, mensagem, false);
        }

        public static string EnviarEmail(string emailTo, string emailFrom, string nomeFrom, string assunto, string mensagem, bool copiarContatoIPEFAE)
        {
            return EnviarEmail(emailTo, emailFrom, nomeFrom, assunto, mensagem, copiarContatoIPEFAE, new List<string>());
        }

        public static string EnviarEmail(string emailTo, string emailFrom, string nomeFrom, string assunto, string mensagem, bool copiarContatoIPEFAE, List<string> anexos)
        {
            return EnviarEmail(emailTo, emailFrom, nomeFrom, assunto, mensagem, copiarContatoIPEFAE, false, anexos);
        }

        public static string EnviarEmail(string emailTo, string emailFrom, string nomeFrom, string assunto, string mensagem, bool copiarContatoIPEFAE, bool copiarFrom, List<string> anexos)
        {
            return EnviarEmail(emailTo, emailFrom, nomeFrom, String.Empty, assunto, mensagem, copiarContatoIPEFAE, copiarFrom, anexos);
        }

        public static string EnviarEmail(string emailTo, string emailFrom, string nomeFrom, string emailReplyTo, string assunto, string mensagem, bool copiarContatoIPEFAE, bool copiarFrom, List<string> anexos)
        {
            if (BaseBusiness.IsDebug)
                emailTo = BaseBusiness.EmailNaoRespondaIPEFAE;

            string bcc = BaseBusiness.EmailIPEFAE;

            MailMessage msg = new MailMessage();
            msg.To.Add(emailTo);

            if (String.IsNullOrEmpty(emailReplyTo))
                emailReplyTo = emailFrom;

            if (copiarFrom && !String.IsNullOrEmpty(emailFrom))
                msg.CC.Add(emailFrom);

            if (copiarContatoIPEFAE)
            {
                string[] emailsBcc = bcc.Split(';');

                foreach (string eBcc in emailsBcc)
                {
                    msg.Bcc.Add(eBcc);
                }
            }

            foreach (string pathAnexo in anexos)
            {
                Attachment attachment = new Attachment(pathAnexo, MediaTypeNames.Application.Pdf);
                ContentDisposition disposition = attachment.ContentDisposition;
                disposition.CreationDate = System.IO.File.GetCreationTime(pathAnexo);
                disposition.ModificationDate = System.IO.File.GetLastWriteTime(pathAnexo);
                disposition.ReadDate = System.IO.File.GetLastAccessTime(pathAnexo);
                disposition.FileName = System.IO.Path.GetFileName(pathAnexo);
                disposition.Size = new System.IO.FileInfo(pathAnexo).Length;
                disposition.DispositionType = DispositionTypeNames.Attachment;
                msg.Attachments.Add(attachment);
            }

            msg.From = new MailAddress(emailFrom, nomeFrom);
            msg.ReplyToList.Add(new MailAddress(emailReplyTo));
            msg.Subject = assunto;
            msg.Body = mensagem;
            msg.IsBodyHtml = true;

            SmtpClient objSmtp = new SmtpClient();

            try
            {
                // Grava na tabela de e-mails
                tb_ema_email ema = new tb_ema_email();
                ema.ema_bit_anexo = anexos.Count > 0;
                ema.ema_dat_email = BaseBusiness.DataAgora;
                ema.ema_des_assunto = assunto;
                ema.ema_des_bcc = bcc;
                ema.ema_des_cc = emailFrom;
                ema.ema_des_corpo = mensagem;
                ema.ema_des_from = emailReplyTo;
                ema.ema_des_to = emailTo;
                EmailData.Salvar(ema);

                objSmtp.Timeout = int.MaxValue;
                //objSmtp.EnableSsl = true;
                objSmtp.Send(msg);
                return "sucesso";
            }
            catch (Exception ex)
            {
                LogErroBusiness.Salvar(ex, null);
                return ex.Message;
            }
        }
    }
}
