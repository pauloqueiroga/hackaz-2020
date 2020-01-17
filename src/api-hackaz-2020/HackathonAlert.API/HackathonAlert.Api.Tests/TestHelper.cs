using System;
using System.Collections.Generic;
using System.Linq;
using HackathonAlert.API.Core.DTO;
using HackathonAlert.API.Core.Infrastructure;
using HackathonAlert.API.Core.Services;
using HackathonAlert.API.Core.Settings;

namespace HackathonAlert.Api.Tests
{
    public static class TestHelper
    {
        public static AlertService GetTestAlertService(IAlertContextFactory alertContextFactory, SourceList sourceList)
        {
            return new AlertService(alertContextFactory, sourceList);
        }

        public static TestInMemoryApiContextFactory GetInMemoryTestContextFactory()
        {
            return new TestInMemoryApiContextFactory(Guid.NewGuid().ToString());
        }

        public static SourceList GetTestSourceList()
        {
            var sourceList = new SourceList()
            {
                Sources = new List<Source>
                {
                    new Source {Guid = "SampleGuid1", TeamName = "SampleTeam1" },
                    new Source {Guid = "SampleGuid2", TeamName = "SampleTeam2" },
                    new Source {Guid = "SampleGuid3", TeamName = "SampleTeam3" }
                }
            };

            return sourceList;
        }

        public static CreateAlertRequest CreateRandomValidCreateAlertRequest()
        {
            var random = new Random();

            var createAlertRequest = new CreateAlertRequest
            {
                AlarmId = Guid.NewGuid().ToString(),
                SourceId = Guid.NewGuid().ToString(),
                StreamId = Guid.NewGuid().ToString(),
                Type = (WarningTypeMessage)random.Next(1, 6),
                Region = (AlarmRegionMessage)random.Next(1, 5),
                Position = CreateRandomValidPositionMessage(random),
                Target1Position = CreateRandomValidPositionMessage(random),
                Target2Position = CreateRandomValidPositionMessage(random),
                Target3Position = CreateRandomValidPositionMessage(random)
            };

            return createAlertRequest;
        }

        public static PositionMessage CreateRandomValidPositionMessage()
        {
            var random = new Random();
            return CreateRandomValidPositionMessage(random);
        }

        public static PositionMessage CreateRandomValidPositionMessage(Random random)
        {
            var positionMessage = new PositionMessage
            {
                Latitude = random.Next(-89, 90),
                Longitude = random.Next(-179, 180),
                Altitude = random.Next(10000),
                Heading = random.Next(0, 360),
                Velocity = random.Next(0, 500)
            };

            return positionMessage;
        }

        public static int GetNumberOfAlertsInDb(TestInMemoryApiContextFactory contextFactory)
        {
            using var context = contextFactory.AlertContext();
            return context.Alerts.Count();
        }
    }
}
