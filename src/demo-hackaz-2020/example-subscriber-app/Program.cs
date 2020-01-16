using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Position4All.DemoSubscriberApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var logger = loggerFactory.CreateLogger<Program>();
            var cancellationSource = new CancellationTokenSource();

            logger.LogDebug("Creating Subscriber");
            var subscriber = new Subscriber(logger, args[0]);
            var longRunningTask = subscriber.RunAsync(cancellationSource.Token);
            logger.LogInformation("Running until user input is detected...");

            Console.ReadLine();
            logger.LogInformation("User input detected, cancelling log running task");
            cancellationSource.Cancel();
            longRunningTask.Wait(500);
            logger.LogInformation("Goodbye.");
        }
    }
}
