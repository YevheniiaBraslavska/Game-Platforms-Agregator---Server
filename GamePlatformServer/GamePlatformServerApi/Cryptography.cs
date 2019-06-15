using GamePlatformServerApi.Models;
using GamePlatformServerApi.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public static string Encrypt(string password) {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashbytes = new byte[36];
            Array.Copy(salt, 0, hashbytes, 0, 16);
            Array.Copy(hash, 0, hashbytes, 16, 20);
            string passwordhash = Convert.ToBase64String(hashbytes);
            return passwordhash;
        }
    }
}
