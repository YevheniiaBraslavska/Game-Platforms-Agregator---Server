using GamePlatformServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Structs {
    public class PasswordStruct {
        public string Password { get; set; }
        public long UserId { get; set; }
        public bool Temporary { get; set; }

        public PasswordStruct() {
        }

        public PasswordStruct(Context context, long userid) {
            GetLastPassword(context, userid);
        }

        public bool Save(Context context) {
            if (UserId == 0)
                return false;
            context.Passwords.Add(new PasswordItem {
                Password = Encrypt(),
                Temporary = false,
                Time = DateTime.Now,
                UserId = UserId
            });
            context.SaveChanges();
            return true;
        }

        public string Encrypt() {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(Password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashbytes = new byte[36];
            Array.Copy(salt, 0, hashbytes, 0, 16);
            Array.Copy(hash, 0, hashbytes, 16, 20);
            string passwordhash = Convert.ToBase64String(hashbytes);
            return passwordhash;
        }

        public void GetLastPassword(Context context, long id) {
            var query = (from password in context.Passwords
                         where password.UserId == id
                         orderby password.Time descending
                         select password).ToList()[0];
            GetFromItem(query);
        }

        public void GetFromItem(PasswordItem item) {
            Password = item.Password;
            UserId = item.UserId;
            Temporary = item.Temporary;
        }
    }
}
