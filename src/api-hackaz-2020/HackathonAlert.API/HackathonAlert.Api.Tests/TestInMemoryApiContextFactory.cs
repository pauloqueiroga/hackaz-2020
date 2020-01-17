using System;
using System.Collections.Generic;
using HackathonAlert.API.Core.Domain;
using HackathonAlert.API.Core.Infrastructure;

namespace HackathonAlert.Api.Tests
{
    public class TestInMemoryApiContextFactory : InMemoryApiContextFactory
    {
        public List<long> AlertIds { get; } = new List<long>();
        public TestInMemoryApiContextFactory(string databaseName) : base(databaseName)
        {
            Seed();
        }

        private void Seed()
        {
            using (var context = AlertContext())
            {
                var firstAlert = Alert.FromCreateRequest(TestHelper.CreateRandomValidCreateAlertRequest());
                firstAlert.SourceName = "SampleTeam3";
                firstAlert.TimePosted = DateTime.UtcNow.Subtract(new TimeSpan(24, 0, 0));

                var secondAlert = Alert.FromCreateRequest(TestHelper.CreateRandomValidCreateAlertRequest());
                var thirdAlert = Alert.FromCreateRequest(TestHelper.CreateRandomValidCreateAlertRequest());

                context.Alerts.Add(firstAlert);
                context.Alerts.Add(secondAlert);
                context.Alerts.Add(thirdAlert);

                AlertIds.Add(firstAlert.Id);
                AlertIds.Add(secondAlert.Id);
                AlertIds.Add(thirdAlert.Id);

                context.SaveChanges();
            }
        }
    }
}
