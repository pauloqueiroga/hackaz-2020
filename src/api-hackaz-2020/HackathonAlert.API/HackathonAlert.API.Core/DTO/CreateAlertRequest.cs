﻿namespace HackathonAlert.API.Core.DTO
{
    public class CreateAlertRequest
    {
        public string AlarmId { get; set; }
        public string StreamId { get; set; }
        public string SourceId { get; set; }
        public WarningTypeMessage Type { get; set; }
        public AlarmRegionMessage Region { get; set; }

        public PositionMessage Position { get; set; }
        public PositionMessage Target1Position { get; set; }
        public PositionMessage Target2Position { get; set; }
        public PositionMessage Target3Position { get; set; }
    }
}
