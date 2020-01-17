using System;
using System.Collections.Generic;
using HackathonAlert.API.Core.DTO;

namespace HackathonAlert.API.Core.Domain
{
    public class Alert
    {
        public long Id { get; set; }
        public string AlarmId { get; set; }
        public string StreamId { get; set; }
        public string SourceName { get; set; }
        public WarningType Type { get; set; }
        public AlarmRegion Region { get; set; }

        public DateTime TimePosted { get; set; }

        public virtual List<Position> Positions { get; set; } = new List<Position>();

        public AlertMessage ToAlertMessage()
        {
            var alertMessage = new AlertMessage
            {
                AlarmId = AlarmId,
                StreamId = StreamId,
                SourceId = SourceName,
                TimePosted = TimePosted,
                Type = (WarningTypeMessage) Type,
                Region = (AlarmRegionMessage) Region
            };

            foreach (var position in Positions)
            {
                switch (position.Type)
                {
                    case PositionType.Target1:
                        alertMessage.Target1Position = position.ToPositionMessage();
                        break;
                    case PositionType.Target2:
                        alertMessage.Target2Position = position.ToPositionMessage();
                        break;
                    case PositionType.Target3:
                        alertMessage.Target3Position = position.ToPositionMessage();
                        break;
                    default:
                        alertMessage.Position = position.ToPositionMessage();
                        break;
                }
            }

            return alertMessage;
        }

        public static Alert FromCreateRequest(CreateAlertRequest createAlertRequest)
        {
            var alert = new Alert
            {
                AlarmId = createAlertRequest.AlarmId,
                StreamId = createAlertRequest.StreamId,
                SourceName = createAlertRequest.SourceId,
                Type = (WarningType)createAlertRequest.Type,
                Region = (AlarmRegion)createAlertRequest.Region
            };

            var ownPosition = Position.FromPositionMessage(createAlertRequest.Position);
            ownPosition.Type = PositionType.Own;
            alert.Positions.Add(ownPosition);

            var target1Position = Position.FromPositionMessage(createAlertRequest.Target1Position);
            target1Position.Type = PositionType.Target1;
            alert.Positions.Add(target1Position);

            var target2Position = Position.FromPositionMessage(createAlertRequest.Target2Position);
            target2Position.Type = PositionType.Target2;
            alert.Positions.Add(target2Position);

            var target3Position = Position.FromPositionMessage(createAlertRequest.Target3Position);
            target3Position.Type = PositionType.Target3;
            alert.Positions.Add(target3Position);

            return alert;
        }
    }
}
