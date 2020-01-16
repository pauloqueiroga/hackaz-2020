using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace Position4All.DemoSubscriberApp
{
    public class Program
    {
        private static string ServerUrl;
        private static string StreamName;

        static int Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var logger = loggerFactory.CreateLogger<Program>();
                                    
            if (!ParseArgs(args))
            {
                logger.LogError("Aborting");
                return -1;
            }

            var cancellationSource = new CancellationTokenSource();

            logger.LogDebug("Creating Subscriber");
            var subscriber = new Subscriber(logger, ServerUrl, StreamName);
            var longRunningTask = subscriber.RunAsync(cancellationSource.Token);
            logger.LogInformation("Running until user input is detected...");

            Console.ReadLine();
            logger.LogInformation("User input detected, cancelling log running task");
            cancellationSource.Cancel();
            longRunningTask.Wait(500);
            logger.LogInformation("Goodbye.");
            return 0;
        }
        
        private static bool ParseArgs(string[] args)
        {
            if (args == null
                || args.Length < 2)
            {
                PrintUsage();
                return false;
            }

            ServerUrl = args[0];
            StreamName = args[1];
            return true;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("USAGE: example-subscriber-app <nats-server-url> <stream-subject>");
            Console.WriteLine("WHERE:");
            Console.WriteLine("   <nats-server-url> - URL for the NATS server");
            Console.WriteLine("   <stream-subject>  - Subject for the stream to subscribe to");
        }

    }
}
