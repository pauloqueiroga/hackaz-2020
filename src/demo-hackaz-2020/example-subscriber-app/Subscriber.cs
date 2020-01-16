using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Threading;
using System.Threading.Tasks;

namespace Position4All.DemoSubscriberApp
{
    internal class Subscriber
    {
        private ILogger<Program> _logger;
        private readonly string _url;
        private const int MaxMessages = 10000;
        private int _counter;

        public Subscriber(ILogger<Program> logger, string url)
        {
            _logger = logger;
            _url = url;
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

            using (var connection = connectionFactory.CreateConnection(options))
            {
                var subscription = connection.SubscribeAsync("test1", MessageReceived);
                _counter = 0;

                while (!cancellationToken.IsCancellationRequested && _counter < MaxMessages)
                {
                    Task.Delay(2000).Wait(cancellationToken);
                }

                _logger.LogInformation($"Received {_counter} messages. Cancelled {cancellationToken.IsCancellationRequested}.");
                return (_counter >= MaxMessages);
            }
        }

        public void MessageReceived(object sender, MsgHandlerEventArgs args)
        {
            _logger.LogInformation($"Received {args.Message.Data[0]}");
        }
    }
}