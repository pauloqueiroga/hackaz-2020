using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HackathonAlert.API.Core.Infrastructure
{
    public class InMemoryApiContextFactory : IAlertContextFactory
    {
        public InMemoryApiContextFactory(string databaseName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// Create a new EquipmentContext using an in-memory database provider.
        /// </summary>
        /// <returns>new EquipmentContext using an in-memory database provider</returns>
        public AlertApiContext AlertContext()
        {
            var options = new DbContextOptionsBuilder<AlertApiContext>()
                .UseInMemoryDatabase(DatabaseName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .EnableSensitiveDataLogging()
                .Options;

            return new AlertApiContext(options);
        }

        private string DatabaseName { get; }
    }
}
