using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HackathonPerformanceTest
{
    public class Program
    {
        private const string Guid = "5938f081625c4d70ad4b7c8040ac6bfe" +
                                    ",cf195bf67f9741218ed554dc42743935" +
                                    ",20c067e529874f56a57d0022f64cc74e" +
                                    ",eaa8449bf0d64ea6944142fe3882a5b4" +
                                    ",b5398326b88741d89b2488f8306efdad" +
                                    ",89db5b423f144656b7a3dac0889044cd" +
                                    ",c277f0280e064022aec778d956a488da" +
                                    ",67a1a9304c7142bf9124289617be9868" +
                                    ",ba0cd88d71cd4dcea540782b17b364a3" +
                                    ",bd076091f4954423a7f10fcd245a22d2";

        private const string HackAzUrl = "https://hackaz.modularminingcloud.com/api/alert/";

        public static void Main(string[] args)
        {
            var listOfGuids = Guid.Split(',');

            var allTasks = new List<Task>();

            var threadNum = 0;

            using var cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine("Starting the Tasks Now....");

            foreach (var guid in listOfGuids)
            {
                var num = threadNum;
                var postTask = new Task(() => MessageWorker(guid, num, cancellationTokenSource.Token));
                var getTask = new Task(() => GetWorker(guid, num, cancellationTokenSource.Token));

                allTasks.Add(postTask);
                allTasks.Add(getTask);

                postTask.Start();
                getTask.Start();

                threadNum++;
            }

            Console.ReadLine();

            cancellationTokenSource.Cancel();

            Task.WaitAll(allTasks.ToArray());
        }

        private static async void MessageWorker(string guid, int threadNum, CancellationToken cancelToken)
        {
            using var httpClient = new HttpClient();
            var waitEvent = new AutoResetEvent(false);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var postCount = 0;
            var unavailableCount = 0;

            while (!cancelToken.IsCancellationRequested)
            {
                var newAlertRequest = TestHelper.CreateRandomValidCreateAlertRequest();
                newAlertRequest.SourceId = guid;

                var alertRequestJson = JsonConvert.SerializeObject(newAlertRequest);

                var httpContent = new StringContent(alertRequestJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response;

                try
                {
                    response = await httpClient.PostAsync(HackAzUrl, httpContent, cancelToken);
                }
                catch (Exception ex)
                {
                    // Waiting here as well to make sure we aren't super messing something up
                    Console.WriteLine($"Exception Occured: {ex.Message}");
                    waitEvent.WaitOne(100);
                    continue;
                }
                
                if (response.StatusCode!= HttpStatusCode.Created)
                {
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        unavailableCount++;
                        if (unavailableCount % 50 == 0)
                        {
                            Console.WriteLine($"ThreadNum: {threadNum} --- Service Unavailable: {unavailableCount}");
                        }
                    }
                    else
                    {
                        var reason = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Request went bad...{response.StatusCode} : {reason}");
                    }
                }

                if (postCount % 100 == 0)
                {
                   Console.WriteLine($"ThreadNum: {threadNum} --- Posted: {postCount}  --- TimeRunning: {stopWatch.ElapsedMilliseconds/1000} sec"); 
                }

                postCount++;
                waitEvent.WaitOne(100);
            }
        }

        private static async void GetWorker(string guid, int threadNum, CancellationToken cancelToken)
        {
            using var httpClient = new HttpClient();
            var waitEvent = new AutoResetEvent(false);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var getCount = 0;
            var unavailableCount = 0;

            var fixedUrl = HackAzUrl + guid + "?minutesToSearch=5";

            while (!cancelToken.IsCancellationRequested)
            {
                HttpResponseMessage response;

                try
                {
                    response = await httpClient.GetAsync(fixedUrl, cancelToken);
                }
                catch (Exception ex)
                {
                    // Waiting here as well to make sure we aren't super messing something up
                    Console.WriteLine($"Exception Occured: {ex.Message}");
                    waitEvent.WaitOne(1000);
                    continue;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        unavailableCount++;
                        if (unavailableCount % 50 == 0)
                        {
                            Console.WriteLine($"ThreadNum: {threadNum} --- Service Unavailable: {unavailableCount}");
                        }
                    }
                    else
                    {
                        var reason = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Request went bad...{response.StatusCode} : {reason}");
                    }
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Just doing this to make sure we get it
                    var content = await response.Content.ReadAsStringAsync();
                }

                if (getCount % 100 == 0)
                {
                    Console.WriteLine($"ThreadNum: {threadNum} --- GetCount: {getCount}  --- TimeRunning: {stopWatch.ElapsedMilliseconds / 1000} sec");
                }

                getCount++;
                waitEvent.WaitOne(1000);
            }
        }
    }
}
