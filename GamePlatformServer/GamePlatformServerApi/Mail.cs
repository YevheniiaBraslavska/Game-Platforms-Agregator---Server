using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GamePlatformServerApi {
    public static class Mail {
        private static readonly string Post = AppConfigurations.Email;
        private static readonly string Password = AppConfigurations.Password;

        public static void Send(string toemail, string body) {
            var smtpClient = new SmtpClient(AppConfigurations.SMTP, AppConfigurations.SMTPPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(Post, Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.Timeout = 10000;

            var mail = new MailMessage() {
                From = new MailAddress(Post, "GamePlatform")
            };
            mail.To.Add(new MailAddress(toemail));
            mail.Body = body;
            smtpClient.Send(mail);
        }
    }
}
