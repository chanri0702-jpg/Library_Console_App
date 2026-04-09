using System;
using System.Net;
using System.Net.Mail;

namespace PRG281_Milestone2
{
    public class ManualEmail : IEmail
    {
        public delegate void EmailAlertHandler();
        public event EmailAlertHandler OnEmailAlert;

        public ManualEmail()
        {
            OnEmailAlert = new EmailAlert().Alert;
        }

        public void SendEmail(string email, string subject, string message)
        {
            string fromEmail = "roccocalzone7@gmail.com";
            string toEmail = email;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = message;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new NetworkCredential("roccocalzone7@gmail.com", "pnuwlqpwhnimedhu");
            smtpClient.EnableSsl = true;
            smtpClient.Send(mail);

            OnEmailAlert?.Invoke();
        }
    }
}
