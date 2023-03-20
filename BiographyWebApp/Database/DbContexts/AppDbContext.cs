using BiographyWebApp.Abstractions;
using BiographyWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BiographyWebApp.Database.DbContexts
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ActivationCode> ActivationCodes { get; set; }
        public DbSet<ResetPasswordCode> ResetPasswordCodes { get; set; }    

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Navigation(u => u.ActivationCode).AutoInclude();
            modelBuilder.Entity<User>().Navigation(u => u.ResetPasswordCode).AutoInclude();
        }
    }
}
