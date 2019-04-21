using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Models {
    public class VerificationCodeItem {
        [Key]
        public int CodeId { get; set; }
        [ForeignKey("UserItem")]
        public long UserId { get; set; }
        public string Code { get; set; }
    }
}
