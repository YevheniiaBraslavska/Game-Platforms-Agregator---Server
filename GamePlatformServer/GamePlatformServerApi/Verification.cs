using GamePlatformServerApi.Models;
using GamePlatformServerApi.Structs;
using System.Linq;
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
    }
}
