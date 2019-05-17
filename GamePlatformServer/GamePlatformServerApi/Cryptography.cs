using GamePlatformServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi {
    public class Cryptography {
        public string Key;

        public Cryptography() {
            Key = Gen();
        }

        public string Gen() {
            Random ran = new Random();
            var min = 1000;
            var max = 10000;
            return ran.Next(min, max).ToString() + "-" + ran.Next(min, max).ToString() + "-" +
                ran.Next(min, max).ToString() + "-" + ran.Next(min, max).ToString();
        }

        public void SaveToVerification(Context context, long UserId) {
            context.VerificationCodes.Add(new VerificationCodeItem() {
                UserId = UserId,
                Code = Key
            });
            context.SaveChanges();
        }

        public void SaveToPassword(Context context, long UserId) {
            context.Passwords.Add(new PasswordItem() {
                UserId = UserId,
                Password = Key,
                Temporary = true,
                Time = DateTime.Now
            });
            context.SaveChanges();
        }
    }
}
