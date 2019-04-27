using GamePlatformServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Structs {
    public class EmailStruct {
        public string Email { get; set; }
        public bool Verified { get; set; }
        public long UserId { get; set; }
        private string post = @"gameplatformproject@gmail.com";
        private string password = @"G1a2m3e4P5l6a7t8f9o0r1m2";

        public EmailStruct() {
        }

        public EmailStruct(EmailItem item) {
            GetFromItem(item);
        }

        public EmailStruct(Context context, long userid) {
            var query = (from email in context.Emails
                         where email.UserId == userid
                         orderby email.Time descending
                         select email).ToList()[0];
            GetFromItem(query);
        }

        public void GetFromItem(EmailItem item) {
            Email = item.Email;
            Verified = item.Verified;
            UserId = item.UserId;
        }

        public bool Save(Context context, DateTime time) {
            if (UserId == 0)
                return false;
            context.Emails.Add(new EmailItem() {
                Email = Email,
                Verified = Verified,
                Time = time,
                UserId = UserId
            });
            context.SaveChanges();
            return true;
        }

        public void Verify(Context context) {
            var smtpClient = new SmtpClient(@"smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(post, password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            smtpClient.Timeout = 10000;

            var mail = new MailMessage() {
                From = new MailAddress(post, "GamePlatform")
            };
            mail.To.Add(new MailAddress(Email));
            var code = GenCode();
            mail.Body = "This is you code for email verification in GamePlatform: " + code;
            smtpClient.Send(mail);
            SaveCode(context, code);
        }

        private void SaveCode(Context context, string code) {
            context.VerificationCodes.Add(new VerificationCodeItem() {
                UserId = UserId,
                Code = code
            });
            context.SaveChanges();
        }

        private string GenCode() {
            Random ran = new Random();
            var min = 1000;
            var max = 10000;
            return ran.Next(min, max).ToString() + "-" + ran.Next(min, max).ToString() + "-" +
                ran.Next(min, max).ToString() + "-" + ran.Next(min, max).ToString();
        }

        public void SetVerified(Context context) {
            var query = (from email in context.Emails
                         where email.Email == Email && email.UserId == UserId
                         select email).ToList()[0];
            query.Verified = true;
            context.Emails.Update(query);
            context.SaveChanges();
        }
    }
}
