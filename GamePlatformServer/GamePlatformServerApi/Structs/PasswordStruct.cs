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
            UserId = userid;
            GetLastPassword(context);
        }

        public bool Save(Context context) {
            if (UserId == 0)
                return false;
            context.Passwords.Add(new PasswordItem {
                Password = Cryptography.Encrypt(Password),
                Temporary = Temporary,
                Time = DateTime.Now,
                UserId = UserId
            });
            context.SaveChanges();
            return true;
        }

        public void GetLastPassword(Context context) {
            var query = (from password in context.Passwords
                         where password.UserId == UserId
                         orderby password.Time descending
                         select password).ToList()[0];
            GetFromItem(query);
        }

        public void GetLastPasswordNotTemporary(Context context) {
            var query = (from password in context.Passwords
                         where password.UserId == UserId && password.Temporary == false
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
