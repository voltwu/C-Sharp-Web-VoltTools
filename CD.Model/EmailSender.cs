using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CD.Models
{
    public class EmailSender
    {
        public string username;
        public string password;
        public string host;
        public EmailSender()
        {
            this.username = "2965372064@qq.com";
            this.password = "cgelbmtqrwwbdeih";
            this.host = "smtp.qq.com";
        }
        public void send(Email email)
        {
            MailMessage myMail = new MailMessage();
            myMail.From = new MailAddress(username, email.sendername);
            foreach (var md in email.MailAddresses)
                myMail.To.Add(md);
            myMail.Subject = email.subject;
            myMail.SubjectEncoding = Encoding.UTF8;
            myMail.Body = email.body;
            myMail.BodyEncoding = Encoding.UTF8;
            myMail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = this.host;
            smtp.Credentials = new NetworkCredential(username, password);
            smtp.Send(myMail);
        }
    }
}
