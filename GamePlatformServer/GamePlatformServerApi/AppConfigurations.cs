using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi {
    public static class AppConfigurations {
        public static string Email;
        public static string Password;
        public static string SMTP;
        public static int SMTPPort;
        public static int PermanentBanErrors;
        public static int PermanentBanTimeHours;
        public static int PermanentBanTimeMinutes;
        public static int PermanentBanTimeSeconds;
        public static int ConstantBanErrors;
        public static int SessionTime;

        public static void Set(IConfiguration config) {
            Email = config["AppConfigurations:Email"];
            Password = config["AppConfigurations:Password"];
            SMTP = config["AppConfigurations:SMTP"];
            SMTPPort = int.Parse(config["AppConfigurations:SMTPPort"]);
            PermanentBanErrors = int.Parse(config["AppConfigurations:PermanentBanErrors"]);
            PermanentBanTimeHours = int.Parse(config["AppConfigurations:PermanentBanTimeHours"]);
            PermanentBanTimeMinutes = int.Parse(config["AppConfigurations:PermanentBanTimeMinutes"]);
            PermanentBanTimeSeconds = int.Parse(config["AppConfigurations:PermanentBanTimeSeconds"]);
            ConstantBanErrors = int.Parse(config["AppConfigurations:ConstantBanErrors"]);
            SessionTime = int.Parse(config["AppConfigurations:SessionTime"]);
        }
    }
}
