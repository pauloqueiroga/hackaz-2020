using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HackathonAlert.API.Core.Infrastructure
{
    public class AlertApiContextMigrationFactory : IDesignTimeDbContextFactory<AlertApiContext>
    {
        public AlertApiContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AlertApiContext>()
                .UseMySql("server = localhost; user id = root; password = 1234; persistsecurityinfo = True; database = dbtest")
                .Options;

            return new AlertApiContext(options);
        }
    }
}
