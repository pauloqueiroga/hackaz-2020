using Microsoft.EntityFrameworkCore;

namespace HackathonAlert.API.Core.Infrastructure
{
    public class MySqlAlertApiContextFactory : IAlertContextFactory
    {
        public MySqlAlertApiContextFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public virtual AlertApiContext AlertContext()
        {
            var options = new DbContextOptionsBuilder<AlertApiContext>()
                .UseMySql(ConnectionString)
                .Options;

            return new AlertApiContext(options);
        }

        protected string ConnectionString { get; }
    }
}
