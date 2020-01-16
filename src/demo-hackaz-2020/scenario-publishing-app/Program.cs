using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Position4All.DemoPublishingApp
{
    public class Program
    {
        private static ILogger Logger;
        private static string ServerUrl;
        private static string StreamSubject;
        private static string ScenariosPath = "./scenarios/";

        static int Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            Logger = loggerFactory.CreateLogger<Program>();
            
            if (!ParseArgs(args))
            {
                Logger.LogError("Aborting");
                return -1;
            }

            var cancellationSource = new CancellationTokenSource();

            Logger.LogDebug("Creating Publisher");
            var publisher = new Publisher(Logger, ServerUrl, StreamSubject);
            var longRunningPublisherTask = publisher.RunAsync(cancellationSource.Token);

            Logger.LogDebug("Creating Publisher");
            var alarmLogger = new AlarmLogger(Logger, StreamSubject);
            
            Logger.LogDebug("Creating Scenario Reader");
            var reader = new ScenarioReader(Logger, ScenariosPath, publisher, alarmLogger);
            var longRunningReaderTask = reader.RunAsync(cancellationSource.Token);

            Logger.LogInformation("Running until user input is detected...");
            
            Console.ReadLine();
            Logger.LogInformation("User input detected, cancelling long running tasks");
            cancellationSource.Cancel();
            Task.Delay(3000).Wait();
            Logger.LogInformation("Goodbye.");
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
            StreamSubject = args[1];

            if (args.Length >= 3)
            {
                ScenariosPath = args[2];
            }

            return true;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("USAGE: scenario-publishing-app <nats-server-url> <stream-subject> [scenarios-path]");
            Console.WriteLine("WHERE:");
            Console.WriteLine("   <nats-server-url> - URL for the NATS server");
            Console.WriteLine("   <stream-subject>  - Subject for the stream to be published");
            Console.WriteLine("   [scenarios-path]  - Path for finding scenario files");
            Console.WriteLine("                       (default if ommited: \"./scenarios/\")");
        }
    }
}
