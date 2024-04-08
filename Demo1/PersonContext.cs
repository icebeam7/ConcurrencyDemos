using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Demo1
{
    public class PersonContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Requires NuGet package Microsoft.EntityFrameworkCore.SqlServer
            optionsBuilder.UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=Demo1Concurrency;Trusted_Connection=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine, LogLevel.Information);
        }
    }
}
