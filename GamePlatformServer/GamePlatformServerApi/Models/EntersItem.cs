using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Models {
    public class EntersItem {
        [Key]
        public int EnterId { get; set; }
        [ForeignKey("UserItem")]
        public long UserId { get; set; }
        public DateTime EnterTime { get; set; }
        public bool Success { get; set; }
    }
}
