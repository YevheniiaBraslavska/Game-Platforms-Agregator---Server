using GamePlatformServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Structs {
    public class EnterStruct {
        public DateTime EnterTime;
        public long UserId;
        public bool Success;

        public void Save(Context context) {
            context.Enters.Add(new EntersItem {
                EnterTime = EnterTime,
                UserId = UserId,
                Success = Success
            });
            context.SaveChanges();
        }
    }
}
