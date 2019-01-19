using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IpCameraClient.Core;
using IpCameraClient.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using MihaZupan;
using Telegram.Bot;

namespace IpCameraClient.ConsoleFacade
{
    class Program
    {     
        static async Task<int> Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var settings = configuration.Get<Settings>();
            
            var getRecordService = new GetRecordService(settings.CameraImageUrl, settings.CameraAuth);
            var accessedUsers = settings.TelegramUsersAccess.Split(";").ToList();
            var telegramClient = string.IsNullOrWhiteSpace(settings.Proxy.Host) ?
                new TelegramBotClient(settings.TelegramBotToken) :
                new TelegramBotClient(
                    settings.TelegramBotToken, 
                    new HttpToSocks5Proxy(settings.Proxy.Host, settings.Proxy.Port, settings.Proxy.User, settings.Proxy.Password)
                );
            
            var telegramService = new TelegramService(getRecordService, accessedUsers, telegramClient);

            telegramClient.OnMessage += (sender, update) => telegramService.ProcessMessageAsync(update.Message);
            telegramClient.StartReceiving();
            Console.WriteLine("Started");
            
            Console.ReadLine();
            return 1;

        }
    }
}