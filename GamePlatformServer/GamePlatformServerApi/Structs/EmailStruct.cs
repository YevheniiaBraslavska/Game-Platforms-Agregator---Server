﻿using GamePlatformServerApi.Models;
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
            Email = item.Email;
            Verified = item.Verified;
            UserId = item.UserId;
        }

        public bool Save(Context context) {
            if (UserId == 0)
                return false;
            context.Emails.Add(new EmailItem() {
                Email = Email,
                Verified = Verified,
                Time = DateTime.Now,
                UserId = UserId
            });
            context.SaveChanges();
            return true;
        }

        public void Verify(Context context) {
            var smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("post", "password");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            var mail = new MailMessage() {
                From = new MailAddress("post", "GamePlatform")
            };
            mail.To.Add(new MailAddress(Email));
            var code = GenCode();
            mail.Body = @"This is you code for email verification in GamePlatform: " + code;
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
    }
}