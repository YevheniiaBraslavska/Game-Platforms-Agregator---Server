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
            var code = new Cryptography();
            var body = "This is you code for email verification in GamePlatform: " + code.Key;
            Mail.Send(Email, body);
            code.SaveToVerification(context, UserId);
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
