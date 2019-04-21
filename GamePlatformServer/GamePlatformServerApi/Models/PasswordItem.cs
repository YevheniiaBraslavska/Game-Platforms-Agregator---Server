using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Models {
    public class PasswordItem {
        [Key]
        public long PasswordId { get; set; }
        public string Password { get; set; }
        [ForeignKey("UserItem")]
        public long UserId { get; set; }
        public bool Temporary { get; set; }
        public DateTime Time { get; set; }
    }
}
