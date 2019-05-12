using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamePlatformServerApi.Models {
    public class Context : DbContext {
        public Context(DbContextOptions<Context> options) : base(options) {
        }

        public DbSet<UserItem> Users { get; set; }
        public DbSet<PasswordItem> Passwords { get; set; }
        public DbSet<EmailItem> Emails { get; set; }
        public DbSet<VerificationCodeItem> VerificationCodes { get; set; }
        public DbSet<EntersItem> Enters { get; set; }
    }
}
