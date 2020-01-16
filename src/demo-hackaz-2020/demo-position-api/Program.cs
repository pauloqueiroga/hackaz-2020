using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Position4All.DemoPositionApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });
            return builder;
        }
    }
}
