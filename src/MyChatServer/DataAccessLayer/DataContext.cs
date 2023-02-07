using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<DbMessage> Messages { get; set; }
    }
}
