namespace HackathonAlert.API.Core.DTO
{
    public class PositionMessage
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Heading { get; set; }
        public double Velocity { get; set; }
    }
}
