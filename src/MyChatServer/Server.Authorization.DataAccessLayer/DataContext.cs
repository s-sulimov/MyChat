namespace Sulimov.MyChat.Server.Authorization.DataAccessLayer;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.Core;

public class DataContext : IdentityDbContext<DbUser>
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder?.Entity<IdentityRole>().HasData(new IdentityRole[]
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
    }
}
