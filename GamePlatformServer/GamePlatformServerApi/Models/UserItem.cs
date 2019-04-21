using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GamePlatformServerApi.Models {
    public class UserItem {
        [Key]
        public long UserId { get; set; }
        [StringLength(50)]
        public string Login { get; set; }
    }
}
