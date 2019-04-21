using GamePlatformServerApi.Models;
using GamePlatformServerApi.Structs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GamePlatformServerApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {
        private readonly Context context;

        public UserController(Context context) {
            this.context = context;

            if (this.context.Users.Count() == 0) {
                TestData.GetUsers(this.context);
            }
        }

        //GET /api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<UserStruct>>> GetUserItems(long id) {
            var query = from user in context.Users
                        where user.UserId == id
                        join password in context.Passwords on user.UserId equals password.UserId
                        select new UserStruct {
                            Login = user.Login,
                            Password = new PasswordStruct() {
                                Password = password.Password
                            }
                        };
            return await query.ToListAsync();
        }

        //GET /api/user/check/[login]
        [HttpGet("check/{login}")]
        public ActionResult<VerificationStruct> CheckLogin(string login) {
            return Verification.Login(context, login);
        }

        //POST /api/user/save/[login,password,email]
        [Route("save")]
        [HttpPost]
        public ActionResult<VerificationStruct> SaveNewUser(string login, string password, string email) {
            var logincheck = Verification.Login(context, login);
            if (!logincheck.Answer)
                return logincheck;
            var passwordcheck = Verification.Password(password);
            if (!passwordcheck.Answer)
                return passwordcheck;
            var user = new UserStruct() {
                Login = login,
                Password = new PasswordStruct() {
                    Password = password,
                    Temporary = false
                },
                Email = new EmailStruct() {
                    Email = email,
                    Verified = false
                }
            };
            user.Register(context);
            return new VerificationStruct() {
                Answer = true,
                Message = "User was registered."
            };
        }

        //GET /api/user/email/getverification/[login]
        [HttpGet("email/getverification/{login}")]
        public ActionResult<VerificationStruct> GetVerifyEmail(string login) {
            var lastemail = (from user in context.Users
                             where user.Login == login
                             join email in context.Emails on user.UserId equals email.UserId
                             orderby email.Time descending
                             select email).ToList()[0];
            if (lastemail.Verified) {
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Already verified."
                };
            }
            else {
                var emailtoverif = new EmailStruct(lastemail);
                emailtoverif.Verify(context);
                return new VerificationStruct() {
                    Answer = true,
                    Message = "Code was send on email."
                };
            }
        }

        //POST /api/user/email/setverification/[login,verifcode]
        [Route("email/setverification")]
        [HttpPost]
        public ActionResult<VerificationStruct> SetVerifyEmail(string login, string verifcode) {
            var dbcode = (from user in context.Users
                          where user.Login == login
                          join code in context.VerificationCodes on user.UserId equals code.UserId
                          orderby code.CodeId
                          select code.Code).ToList()[0];
            if (dbcode.Equals(verifcode))
                return new VerificationStruct() {
                    Answer = true,
                    Message = "Code is right."
                };
            else
                return new VerificationStruct() {
                    Answer = false,
                    Message = "Code is invalid"
                };
        }
    }
}
