﻿using GamePlatformServerApi.Models;
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
            if (!answer.Answer && answer.Message["Invalid"].Equals("password")) {
                var entersleft = user.GetLeftEnters(context);
                if (entersleft == 0) {
                    answer.Message["Message"] = answer.Message["Message"] + " User was permanently banned.";
                }
                answer.Message.Add("EntersLeft", entersleft.ToString());
                if (entersleft == 0) {
                    user.SetBan(context);
                }
            }
            return answer;
        }

        //POST /api/user/
    }
}
