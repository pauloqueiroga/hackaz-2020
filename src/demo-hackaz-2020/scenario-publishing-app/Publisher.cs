using Google.Protobuf;
using Microsoft.Extensions.Logging;
using NATS.Client;
using Position4All.DemoPublishingApp.Messages;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Position4All.DemoPublishingApp
{
    public class Publisher
    {
        private readonly ILogger _logger;
        private string _url;
        private readonly string _subject;
        private ConcurrentQueue<State> _stateQueue = new ConcurrentQueue<State>();

        public Publisher(ILogger logger, string url, string subject)
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
            var counter = 0L;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_url == null)
                    {
                        _url = "nats://localhost:4222";
                    }

                    _logger.LogInformation($"Using Server URL: {_url}");
                    _logger.LogInformation($"Using Stream Subject: {_subject}");

                    var connectionFactory = new ConnectionFactory();
                    var options = ConnectionFactory.GetDefaultOptions();
                    options.Url = _url;

                    using var connection = connectionFactory.CreateConnection(options);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        while (_stateQueue.IsEmpty)
                        {
                            Task.Delay(100).Wait(cancellationToken);
                        }

                        var gotIt = _stateQueue.TryDequeue(out var message);

                        if (!gotIt)
                        {
                            continue;
                        }

                        using (var stream = new MemoryStream())
                        {
                            counter++;
                            message.WriteTo(stream);
                            connection.Publish(_subject, stream.ToArray());
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    Task.Delay(1000).Wait(cancellationToken);
                }
            }

            _logger.LogInformation($"Published {counter} messages. Cancelled {cancellationToken.IsCancellationRequested}.");
            return (!cancellationToken.IsCancellationRequested);
        }

        internal void EnqueueMessage(State state)
        {
            _stateQueue.Enqueue(state);
        }
    }
}
