using GamePlatformServerApi.Models;
using GamePlatformServerApi.Structs;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace GamePlatformServerApi {
    public static class Verification {
        public static VerificationStruct Login(Context context, string login) {
            var query = from user in context.Users
                        where user.Login == login
                        select user.Login;
            if (query.ToList().Count() != 0)
                return new VerificationStruct() {
                    Answer = false,
                    Message = "User with this login is already exist."
                };
            if (login.Count() > 50)
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Login must be shorter than 50 symbols."
                };
            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9]+$")) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Login must contain only latin letters and numbers."
                };
            }
            return new VerificationStruct() {
                Answer = true,
                Message = "Login is verified."
            };
        }

        public static VerificationStruct Password(string password) {
            if (!password.Any(char.IsUpper) && !password.Any(char.IsLower)) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Password must contain small and CAPITAL letters."
                };
            }
            if (!password.Any(char.IsNumber)) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Password must contain numbers."
                };
            }
            if (password.Count() > 60) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Password must be shorted than 60 symbols."
                };
            }
            if (password.Count() < 6) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Password must be longer than 6 symbols."
                };
            }
            return new VerificationStruct() {
                Answer = true,
                Message = "Password is verified."
            };
        }

        public static VerificationStruct User(Context context, string login, string password) {
            var savedpasswords = (from user in context.Users
                                  where user.Login == login
                                  join pass in context.Passwords on user.UserId equals pass.UserId
                                  orderby pass.Time descending
                                  select pass.Password).ToList();
            if (savedpasswords.Count() == 0)
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Invalid login."
                };
            var savedpasswordhash = savedpasswords[0];
            byte[] hashbytes = Convert.FromBase64String(savedpasswordhash);
            byte[] salt = new byte[16];
            Array.Copy(hashbytes, 0, salt, 0, 16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            for (int i = 0; i < 20; i++)
                if (hashbytes[i + 16] != hash[i])
                    return new VerificationStruct() {
                        Answer = false,
                        Message = "Invalid password."
                    };
            return new VerificationStruct() {
                Answer = true,
                Message = "Password is right."
            };
        }
    }
}
