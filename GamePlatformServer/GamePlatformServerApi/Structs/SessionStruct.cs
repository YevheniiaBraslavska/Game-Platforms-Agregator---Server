using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Structs {
    public class SessionStruct {
        public UserStruct User;
        public int SessionNo;

        public int GenSessionNo() {
            var ran = new Random();
            var min = 1000;
            var max = 10000;
            return ran.Next(min, max);
        }

        public UserStruct GetUser(List<SessionStruct> sessions) {
            foreach (var session in sessions) {
                if (session.SessionNo == SessionNo)
                    return session.User;
            }
            return new UserStruct();
        }

        public int GetSession(List<SessionStruct> sessions) {
            foreach (var session in sessions) {
                if (session.User.Id == User.Id)
                    return session.SessionNo;
            }
            return 0;
        }
    }
}
