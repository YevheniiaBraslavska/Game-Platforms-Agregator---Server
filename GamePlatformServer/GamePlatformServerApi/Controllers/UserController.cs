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
        private List<SessionStruct> Sessions;

        public UserController(Context context) {
            this.context = context;

            if (this.context.Users.Count() == 0) {
                TestData.GetUsers(this.context);
            }
        }

        //------------------------------------------
        // ... [TECH] Get user structure for user id
        //------------------------------------------
        //GET /api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<UserStruct>>> GetUserItems(long id) {
            var query = from user in context.Users
                        where user.UserId == id
                        join password in context.Passwords on user.UserId equals password.UserId
                        join email in context.Emails on user.UserId equals email.UserId
                        select new UserStruct {
                            Login = user.Login,
                            Password = new PasswordStruct() {
                                Password = password.Password,
                                Temporary = password.Temporary
                            },
                            Email = new EmailStruct() {
                                Email = email.Email,
                                Verified = email.Verified
                            }
                        };
            return await query.ToListAsync();
        }

        //-------------------------------------------------
        // ... Check if login is correct for internal rules
        //-------------------------------------------------
        //GET /api/user/check/login/[login]
        [HttpGet("check/login/{login}")]
        public ActionResult<VerificationStruct> CheckLogin(string login) {
            return Verification.Login(context, login);
        }

        //----------------------------------------------------
        // ... Check if password is correct for internal rules
        //----------------------------------------------------
        //GET /api/user/check/password/[password]
        [HttpGet("check/password/{password}")]
        public ActionResult<VerificationStruct> CheckPassword(string password) {
            return Verification.Password(password);
        }

        //------------------------------------------------
        // ... Register new user with login/password/email
        //------------------------------------------------
        //POST /api/user/register/[login,password,email]
        [Route("register")]
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
                Message = new Dictionary<string, string>() {
                    ["Message"] = "User was registered."
                }
            };
        }

        //-----------------------------------------------------------------------
        // ... Get verification code on email, if this email was not verified yet
        //-----------------------------------------------------------------------
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
                    Message = new Dictionary<string, string>() {
                        ["Message"] = "Already verified."
                    }
                };
            }
            else {
                var emailtoverif = new EmailStruct(lastemail);
                emailtoverif.Verify(context);
                return new VerificationStruct() {
                    Answer = true,
                    Message = new Dictionary<string, string>() {
                        ["Message"] = "Code was send on email."
                    }
                };
            }
        }

        //---------------------------------------------------
        // ... Set verification for email with temporary code
        //---------------------------------------------------
        //POST /api/user/email/setverification/[login,verifcode]
        [Route("email/setverification")]
        [HttpPost]
        public ActionResult<VerificationStruct> SetVerifyEmail(string login, string verifcode) {
            var user = new UserStruct(context, login);
            var dbcode = user.GetVerifCode(context);
            if (dbcode.Equals(verifcode)) {
                user.Email.SetVerified(context);
                return new VerificationStruct() {
                    Answer = true,
                    Message = new Dictionary<string, string>() {
                        ["Message"] = "Code is right."
                    }
                };
            }
            else
                return new VerificationStruct() {
                    Answer = false,
                    Message = new Dictionary<string, string>() {
                        ["Message"] = "Code is invalid"
                    }
                };
        }

        //----------------------------------------
        // ... Enter in system with login/password
        //----------------------------------------
        //POST /api/user/login/[login,password]
        [Route("login")]
        [HttpPost]
        public ActionResult<VerificationStruct> Login(string login, string password) {
            var user = new UserStruct() {
                Login = login,
                Password = new PasswordStruct() {
                    Password = password
                }
            };
            var answer = user.Enter(context);
            if (!answer.Answer && answer.Message["Invalid"].Equals("Password")) {
                var wrongenters = user.GetWrongEnters(context);
                var permanentersleft = AppConfigurations.PermanentBanErrors - wrongenters;
                var constentersleft = AppConfigurations.ConstantBanErrors - wrongenters;
                if (AppConfigurations.PermanentBanErrors - wrongenters == 0) {
                    answer.Message.Add("EntersLeft", permanentersleft.ToString());
                    answer.Message["Message"] = answer.Message["Message"] + " User was permanently banned.";
                    user.SetBan(context, true);
                    user.SendPermanentBanEmail(context);
                }
                else if (constentersleft <= 0) {
                    answer.Message.Add("EntersLeft", constentersleft.ToString());
                    answer.Message["Message"] = answer.Message["Message"] + " Password was reset.";
                    user.SetBan(context, true);
                    user.SendConstantBanEmail(context);
                }
            }
            if (!answer.Answer && answer.Message["Invalid"].Equals("Locked")) {
                user.SendPermanentBanEmail(context);
            }
            if (answer.Answer) {
                var session = new SessionStruct() {
                    User = user,
                };
                session.SessionNo = session.GetSession(Sessions);
                if (session.SessionNo != 0) {
                    Sessions.Remove(session);
                }
                session.SessionNo = session.GenSessionNo();
                Sessions.Add(session);
                answer.Message.Add("SessionNo", session.SessionNo.ToString());
            }
            return answer;
        }

        //--------------------------------------------------
        // ... Set new password(only if user is not locked)
        //--------------------------------------------------
        //POST /api/user/password/change/[session],[new password]
        [Route("password/change")]
        [HttpPost]
        public ActionResult<VerificationStruct> ChangePassword(int session, string newpassword) {
            var sessionstruct = new SessionStruct() {
                SessionNo = session
            };
            sessionstruct.User = sessionstruct.GetUser(Sessions);
            if (sessionstruct.User.Id != 0) {
                var password = new PasswordStruct() {
                    UserId = sessionstruct.User.Id
                };
                password.GetLastPasswordNotTemporary(context);
                if (password.Password == Cryptography.Encrypt(newpassword))
                    return new VerificationStruct() {
                        Answer = false,
                        Message = new Dictionary<string, string>() {
                            ["Message"] = "New password should not be equal to previous."
                        }
                    };
                password.Password = newpassword;
                password.Save(context);
            }
            return new VerificationStruct() {
                Answer = true,
                Message = new Dictionary<string, string>() {
                    ["Message"] = "Password was successfully changed."
                }
            };
        }
    }
}
