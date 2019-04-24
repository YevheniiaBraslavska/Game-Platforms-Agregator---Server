using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GamePlatformServerApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase {
        //GET /api/help
        [HttpGet]
        public ContentResult Help() {
            var content = "<html><body>" +
                "<h1>Help</h1>" +

                "<h4>GET /api/user/[id]</h4>" +
                "<p>Get user Login/Password for id.</p>" +
                "<p>Example: https://localhost:1234/api/user/3</p>" +

                "<h4>GET /api/user/check/[login]</h4>" +
                "<p>Check if this login can be taken for registrarion (not exists yet).</p>" +
                "<p>Example: https://localhst:1234/api/user/check/Ann</p>" +

                "<h4>GET /api/user/check/[password]</h4>" +
                "<p>Check password for rules: only latin and numbers, small and Capital characters, numbers.</p>" +
                "<p>Example: https://localhost:1234/api/user/check/AnnPass1</p>" +

                "<h4>POST /api/user/register/[login,password,email]</h4>" +
                "<p>Register new user. Note, login and password will be checked again.</p>" +
                "<p>Example: https://localhost:1234/api/user/register/?login=Max&password=MaxPas5&email=Max@email.com</p>" +

                "<h4>GET /api/user/email/getverification/[login]</h4>" +
                "<p>Ask for verification of email. If last email is not registred, server will send the email with code.</p>" +
                "<p>Example: https://localhost:1234/api/user/email/getverification/Ann</p>" +

                "<h4>POST /api/user/email/setverification/[login,verifcode]</h4>" +
                "<p>Check code for last email verification.</p>" +
                "<p>Example: </p>" +

                "<h4>POST /api/user/login/[login,password]</h4>\n" +
                "<p>Log in with login/password. Rollback if invalid login or password.</p>" +
                "<p>Example: https://localhost:1234/api/user/login/?login=Joe&password=JoePass2</p>" +
                "</body></html>";

            return new ContentResult {
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
                Content = content
            };
        }
    }
}