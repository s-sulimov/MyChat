using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.DAL
{
    public class DataContext : IdentityDbContext<DbUser>
    {
        public DbSet<DbMessage> Messages { get; set; }
        public DbSet<DbChat> Chats { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole[]
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "User",
                },
            });
        }
    }
}
