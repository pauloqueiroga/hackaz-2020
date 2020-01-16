using Position4All.DemoPublishingApp.Messages;

namespace Position4All.DemoPublishingApp
{
    internal static class StateExtensions
    {
        public static string ToCsv(this State state)
        {
            if (state == null)
            {
                return ",,,";
            }

            return $"{state.OwnPosition.ToCsv()},{state.Target1Position.ToCsv()},{state.Target2Position.ToCsv()},{state.Target3Position.ToCsv()}";
        }

        public static string ToCsv(this Position position)
        {
            if (position == null)
            {
                return ",,,,";
            }

            return $"{position.Latitude},{position.Longitude},{position.Altitude},{position.Heading},{position.Velocity}";
        }
    }
}