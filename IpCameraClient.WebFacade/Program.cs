using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace IpCameraClient.WebFacade
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseIISIntegration()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();
            Bot.Api.DeleteWebhookAsync().Wait();
        }
    }
}
