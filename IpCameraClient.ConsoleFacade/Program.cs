using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IpCameraClient.Infrastructure.Repository;
using IpCameraClient.Model;
using IpCameraClient.Model.Telegram;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace IpCameraClient.ConsoleFacade
{
    class Program
    {
        private static TelegramBotClient _client;
        
        static async Task<int> Main(string[] args)
        {                       
            var telegramFacade = new TelegramFacade();
            telegramFacade.Run();
            
            return 1;
        }
        
        public class Settings
        {
            public string TelegramBotToken { get; set; }
            public string TelegramUsersAccess { get; set; }
            public string HostUrl { get; set; }
            public string DefaultCameraModelName { get; set; }
            public string DefaultCameraUrl { get; set; }
            public string DefaultCameraAuth { get; set; }
            public string ContentFolderName { get; set; }
            public bool Ngrok { get; set; }
        }
    }
}