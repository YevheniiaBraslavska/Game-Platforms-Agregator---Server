using GamePlatformServerApi.Models;
using GamePlatformServerApi.Structs;
using System;

namespace GamePlatformServerApi {
    public static class TestData {
        public static void GetUsers(Context context) {
            var user = new UserStruct() {
                Login = "Ann",
                Password = new PasswordStruct() {
                    Password = "AnnPass1",
                    Temporary = false
                },
                Email = new EmailStruct() {
                    Email = "AnnEmail",
                    Verified = true
                }
            };
            user.Register(context);

            user = new UserStruct() {
                Login = "Joe",
                Password = new PasswordStruct() {
                    Password = "JoePass2",
                    Temporary = false
                },
                Email = new EmailStruct() {
                    Email = "JoeEmail",
                    Verified = true
                }
            };
            user.Register(context);
            user.SaveNewPassword(context, new PasswordStruct() {
                Password = "JoePass22",
                Temporary = false
            });

            user = new UserStruct() {
                Login = "Liz",
                Password = new PasswordStruct() {
                    Password = "LizPass3",
                    Temporary = false
                },
                Email = new EmailStruct() {
                    Email = "LizEmail",
                    Verified = true
                }
            };
            user.Register(context);
        }
    }
}
