using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Models {
    public class EmailItem {
        [Key]
        public int EmailId { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
        [ForeignKey("UserItem")]
        public long UserId { get; set; }
        public DateTime Time { get; set; }
    }
}
