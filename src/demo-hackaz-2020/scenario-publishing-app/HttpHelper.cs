using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Position4All.DemoPublishingApp.Messages;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Position4All.DemoPublishingApp
{
    public class HttpHelper
    {
        private const string HardcodedSourceId = "5cf1009186ef473aa4a4c3109d0982bb";
        private readonly ILogger _logger;
        private readonly string _serverUrl;


        public HttpHelper(ILogger logger, string serverUrl)
        {
            _logger = logger;
            _serverUrl = serverUrl;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12
                | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls;
        }

        internal async Task<bool> NotifyAlarm(ExpectedAlarm alarm, string streamSubject)
        {
            var requestBody = ToAlarmRequest(alarm, streamSubject);
            var serializedBody = JsonConvert.SerializeObject(requestBody);
            var contentBody = new StringContent(serializedBody, Encoding.UTF8, "application/json");
            var success = await Post($"{_serverUrl}/alert", contentBody);
            return success;
        }

        private async Task<bool> Post(string url, StringContent body)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using var response = await httpClient.PostAsync(url, body);
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        _logger.LogWarning($"HTTP post returned {response.StatusCode}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Error connecting to HTTP server {e.Message}");
                return false;
            }
        }

        private CreateAlertRequest ToAlarmRequest(ExpectedAlarm alarm, string streamSubject)
        {
            var parsedType = Enum.TryParse<WarningTypeMessage>(alarm.AlarmType, out var warningType);

            if (!parsedType)
            {
                _logger.LogWarning($"Unable to parse Alarm Type {alarm.AlarmType}");
                warningType = WarningTypeMessage.Unspecified;
            }

            var parsedRegion = Enum.TryParse<AlarmRegionMessage>(alarm.AlarmRegion, out var alarmRegion);

            if (!parsedRegion)
            {
                _logger.LogWarning($"Unable to parse Alarm Region {alarm.AlarmRegion}");
                alarmRegion = AlarmRegionMessage.Unspecified;
            }

            var request = new CreateAlertRequest
            {
                SourceId = HardcodedSourceId,
                StreamId = streamSubject,
                Type = warningType,
                AlarmId = alarm.AlarmId,
                Position = ToServicePosition(alarm.CurrentState.OwnPosition),
                Target1Position = ToServicePosition(alarm.CurrentState.Target1Position),
                Target2Position = ToServicePosition(alarm.CurrentState.Target2Position),
                Target3Position = ToServicePosition(alarm.CurrentState.Target3Position),
                Region = alarmRegion,
            };

            return request;
        }

        private PositionMessage ToServicePosition(Position position)
        {
            var message = new PositionMessage
            {
                Latitude = (position?.Latitude).GetValueOrDefault(),
                Longitude = (position?.Longitude).GetValueOrDefault(),
                Altitude = (position?.Altitude).GetValueOrDefault(),
                Heading = (position?.Heading).GetValueOrDefault(),
                Velocity = (position?.Velocity).GetValueOrDefault(),
            };
            return message;
        }
    }
}