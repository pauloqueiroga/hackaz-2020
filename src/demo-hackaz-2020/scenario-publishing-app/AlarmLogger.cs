using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Position4All.DemoPublishingApp
{
    public class AlarmLogger
    {
        private ILogger _logger;
        private string _streamSubject;

        public AlarmLogger(ILogger logger, string streamSubject)
        {
            _logger = logger;
            _streamSubject = streamSubject;
        }

        internal async void LogAlarm(ExpectedAlarm annotation)
        {
            await Task.Run(() => _logger.LogInformation($"stream_service,{_streamSubject},{annotation.ToCsv()}"));
        }
    }
}