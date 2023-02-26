using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.DAL
{
    public class DataContext : IdentityDbContext<DbUser>
    {
        public DbSet<DbMessage> Messages { get; set; }
        public DbSet<DbChat> Chats { get; set; }
        public DbSet<DbChatRole> ChatRoles { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole[]
            {
                new IdentityRole
                {
                    Name = Constants.IdentityAdminRoleName,
                    NormalizedName = Constants.IdentityAdminRoleName,
                },
                new IdentityRole
                {
                    Name = Constants.IdentityUserRoleName,
                    NormalizedName = Constants.IdentityUserRoleName,
                },
            });

            modelBuilder.Entity<DbChatRole>().HasData(new DbChatRole[]
            {
                new DbChatRole
                {
                    Id = 1,
                    Name = Constants.ChatOwnerRoleName,
                },
                new DbChatRole
                {
                    Id = 2,
                    Name = Constants.ChatAdminRoleName,
                },
                new DbChatRole
                {
                    Id = 3,
                    Name = Constants.ChatUserRoleName,
                },
            });
        }
    }
}
