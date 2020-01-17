using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Position4All.DemoSubscriberApp
{
    internal class Subscriber
    {
        private ILogger<Program> _logger;
        private readonly string _url;
        private readonly string _subject;
        private readonly Stopwatch _watch = new Stopwatch();
        private const int MaxMessages = 10000;
        private int _counter;

        public Subscriber(ILogger<Program> logger, string url, string subject)
        {
            _logger = logger;
            _url = url;
            _subject = subject;
        }

        internal async Task<bool> RunAsync(CancellationToken cancellationToken)
        {
            var completed = await Task.Run(() => Run(cancellationToken)).ConfigureAwait(false);
            return completed;
        }

        internal bool Run(CancellationToken cancellationToken)
        {
            var connectionFactory = new ConnectionFactory();
            var options = ConnectionFactory.GetDefaultOptions();
            options.Url = _url ?? "nats://localhost:4222";
            options.SetUserCredentials("./user.creds");

            using (var connection = connectionFactory.CreateConnection(options))
            {
                var subscription = connection.SubscribeAsync(_subject, MessageReceived);
                _counter = 0;
                _watch.Start();

                while (!cancellationToken.IsCancellationRequested && _counter < MaxMessages)
                {
                    _logger.LogInformation($"So far, {_counter} messages received");
                    Task.Delay(60000).Wait(cancellationToken);
                }

                _logger.LogInformation($"Received {_counter} messages. Cancelled {cancellationToken.IsCancellationRequested}.");
                return (_counter >= MaxMessages);
            }
        }

        public void MessageReceived(object sender, MsgHandlerEventArgs args)
        {
            _counter++;
            _logger.LogInformation($"Received {args.Message.Data.Length} bytes (Message #{_counter:00000} @{_watch.ElapsedMilliseconds} ms)");
        }
    }
}