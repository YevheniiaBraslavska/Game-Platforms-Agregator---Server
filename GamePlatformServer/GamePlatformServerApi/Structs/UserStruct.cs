using GamePlatformServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GamePlatformServerApi.Structs {
    public class UserStruct {
        private long Id;
        public string Login;
        public PasswordStruct Password;
        public EmailStruct Email;
        public bool Ban;

        public UserStruct() {
        }

        /// <summary>
        /// Fetch all valid data for user's login.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="login"></param>
        public UserStruct(Context context, string login) {
            Login = login;
            GetUserItem(context);
            Password = new PasswordStruct(context, Id);
            Email = new EmailStruct(context, Id);
        }

        private void GetUserItem(Context context) {
            var item = (from user in context.Users
                        where user.Login == Login
                        select user).ToList()[0];
            Id = item.UserId;
            Ban = item.Ban;
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
                Ban = false
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

        public VerificationStruct Enter(Context context) {
            var userexist = UserExist(context);
            if (!userexist) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = new Dictionary<string, string>() {
                        ["Invalid"] = "login",
                        ["Message"] = "Invalid login."
                    }
                };
            }
            else if (Ban) {
                var enter = new EnterStruct() {
                    UserId = Id,
                    EnterTime = DateTime.Now,
                    Success = false
                };
                enter.Save(context);
                return new VerificationStruct() {
                    Answer = false,
                    Message = new Dictionary<string, string>() {
                        ["Invalid"] = "login",
                        ["Message"] = "This login is permanently banned."
                    }
                };
            }
            else {
                var answer = VerifyPassword(context);
                if (Id != 0) {
                    var enter = new EnterStruct() {
                        UserId = Id,
                        EnterTime = DateTime.Now,
                        Success = answer.Answer
                    };
                    enter.Save(context);
                }
                return answer;
            }
        }

        private VerificationStruct VerifyPassword(Context context) {
            var savedpasswords = (from user in context.Users
                                  where user.Login == Login
                                  join pass in context.Passwords on user.UserId equals pass.UserId
                                  orderby pass.Time descending
                                  select pass.Password).ToList();
            var savedpasswordhash = savedpasswords[0];
            byte[] hashbytes = Convert.FromBase64String(savedpasswordhash);
            byte[] salt = new byte[16];
            Array.Copy(hashbytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(Password.Password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashbytes[i + 16] != hash[i])
                    return new VerificationStruct() {
                        Answer = false,
                        Message = new Dictionary<string, string>() {
                            ["Invalid"] = "password",
                            ["Message"] = "Invalid password."
                        }
                    };
            return new VerificationStruct() {
                Answer = true,
                Message = new Dictionary<string, string>() {
                    ["Message"] = "Password is right."
                }
            };
        }

        public int GetLeftEnters(Context context) {
            var dayago = 24 * 60;
            var lastenters = (from enter in context.Enters
                              where enter.UserId == Id
                              where enter.EnterTime >= DateTime.Now.AddMinutes(-dayago)
                              select enter).ToList();
            var count = 0;
            for (var i = 0; i < lastenters.Count(); i++) {
                if (!lastenters[i].Success)
                    count++;
                else {
                    count = 0;
                    break;
                }
            }
            return AppConfigurations.PermanentBanErrors - count;
        }

        public void SetBan(Context context) {
            var query = (from user in context.Users
                         where user.UserId == Id
                         select user).ToList()[0];
            query.Ban = true;
            context.Users.Update(query);
            context.SaveChanges();
        }

        public bool UserExist(Context context) {
            GetUserItem(context);
            return Id != 0 ? true : false;
        }
    }
}
