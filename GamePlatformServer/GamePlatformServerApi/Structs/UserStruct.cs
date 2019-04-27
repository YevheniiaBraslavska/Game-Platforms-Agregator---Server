﻿using GamePlatformServerApi.Models;
using System;
using System.Linq;

namespace GamePlatformServerApi.Structs {
    public class UserStruct {
        private long Id;
        public string Login;
        public PasswordStruct Password;
        public EmailStruct Email;

        public UserStruct() {
        }

        /// <summary>
        /// Fetch all valid data for user's login.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="login"></param>
        public UserStruct(Context context, string login) {
            Login = login;
            Id = (from user in context.Users
                  where user.Login == login
                  select user.UserId).ToList()[0];
            Password = new PasswordStruct(context, Id);
            Email = new EmailStruct(context, Id);
        }

        public void Register(Context context) {
            Id = SaveUser(context);
            Password.UserId = Id;
            Password.Save(context);
            Email.UserId = Id;
            Email.Save(context, DateTime.Now);
        }

        private long SaveUser(Context context) {
            context.Users.Add(new UserItem {
                Login = Login,
            });
            context.SaveChanges();
            return (from user in context.Users
                    where user.Login == Login
                    select user.UserId).ToList()[0];
        }

        public void SaveNewPassword(Context context, PasswordStruct password) {
            Password.UserId = Id;
            Password.Save(context);
        }

        public string GetVerifCode(Context context) {
            return (from user in context.Users
                    where user.Login == Login
                    join code in context.VerificationCodes on user.UserId equals code.UserId
                    orderby code.CodeId descending
                    select code.Code).ToList()[0];
        }
    }
}
