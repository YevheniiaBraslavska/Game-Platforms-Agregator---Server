using GamePlatformServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Structs {
    public class PasswordStruct {
        public string Password { get; set; }
        public long UserId { get; set; }
        public bool Temporary { get; set; }

        public bool Save(Context context) {
            if (UserId == 0)
                return false;
            context.Passwords.Add(new PasswordItem {
                Password = Password,
                Temporary = false,
                Time = DateTime.Now,
                UserId = UserId
            });
            context.SaveChanges();
            return true;
        }
    }
}
