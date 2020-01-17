using System.Linq.Expressions;
using HackathonAlert.API.Core.DTO;

namespace HackathonAlert.API.Core.Domain
{
    public class Position
    {
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Heading { get; set; }
        public double Velocity { get; set; }

        public PositionType Type { get; set; }

        public virtual Alert ParentAlert { get; set; }

        public PositionMessage ToPositionMessage()
        {
            return new PositionMessage
            {
                Latitude = Latitude,
                Longitude = Longitude,
                Altitude = Altitude,
                Heading = Heading,
                Velocity = Velocity
            };
        }

        public static Position FromPositionMessage(PositionMessage position)
        {
            return new Position
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Altitude = position.Altitude,
                Heading = position.Heading,
                Velocity = position.Velocity
            };
        }
    }
}
