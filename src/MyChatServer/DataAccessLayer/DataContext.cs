using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.DAL
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole[]
            {
                new IdentityRole
                {
                    Name = "Administrator",
                    NormalizedName = "Administrator",
                    Id = "Administrator",
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "User",
                    Id = "User",
                },
            });
        }

        public DbSet<DbMessage> Messages { get; set; }
    }
}
