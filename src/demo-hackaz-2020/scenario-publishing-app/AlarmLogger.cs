using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Position4All.DemoPublishingApp
{
    public class AlarmLogger
    {
        private readonly ILogger _logger;
        private readonly string _streamSubject;
        private readonly HttpHelper _httpHelper;

        public AlarmLogger(ILogger logger, string streamSubject, string alarmServerUrl)
        {
            _logger = logger;
            _streamSubject = streamSubject;
            _httpHelper = new HttpHelper(_logger, alarmServerUrl);
        }

        internal async void LogAlarm(ExpectedAlarm annotation)
        {
            await Task.Run(() => _logger.LogInformation($"stream_service,{_streamSubject},{annotation.ToCsv()}"));
            await _httpHelper.NotifyAlarm(annotation, _streamSubject);
        }
    }
}