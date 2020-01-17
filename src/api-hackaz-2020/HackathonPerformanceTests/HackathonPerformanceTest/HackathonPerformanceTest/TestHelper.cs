using System;
using HackathonAlert.API.Core.DTO;

namespace HackathonPerformanceTest
{
    public static class TestHelper
    {
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
    }
}
