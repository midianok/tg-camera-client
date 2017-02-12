using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Telegram.Bot;

namespace IpCameraClient.WebFacade
{
    public class Program
    {
        public static class Bot
        {
            public static readonly TelegramBotClient client = new TelegramBotClient("");
        }

        public static void Main(string[] args)
        {
            Bot.client.SetWebhookAsync("https://edf47d7b.ngrok.io/api/TelegramBot").Wait();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://*:5050")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();

            Bot.client.SetWebhookAsync().Wait();

        }
    }
}
