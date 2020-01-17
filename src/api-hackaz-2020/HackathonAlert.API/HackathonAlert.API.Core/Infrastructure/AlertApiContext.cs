using HackathonAlert.API.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace HackathonAlert.API.Core.Infrastructure
{
    public class AlertApiContext : DbContext
    {
        public AlertApiContext(DbContextOptions<AlertApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Alert>().HasIndex(a => a.TimePosted);
        }

        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Position> Positions { get; set; }
    }
}
