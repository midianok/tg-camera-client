using System;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Model;
using IpCameraClient.Model.Telegram;
using MihaZupan;
using Telegram.Bot;
// ReSharper disable StringLiteralTypo

namespace IpCameraClient.ConsoleFacade
{
    public class TelegramFacade
    {
        private readonly IRepository<Camera> _cameras;
        private readonly IRepository<Record> _records;
        private readonly IRepository<TelegramUser> _users;
        private readonly IRecordSaverService _recordSaverService;
        private readonly IGetRecordService _getRecordService;
        private readonly TelegramBotClient _client;
        
        private readonly Settings _settings;
        
        public TelegramFacade()
        {
            _client = new TelegramBotClient("353481546:AAE7MAG1FNRUiGbL7ZrJY9RjKh-0PmJGMns", new HttpToSocks5Proxy("quarasique.xyz", 1080, "proxyuser", "x2saLqSFpd2v9Aqs"));
        }
        
        public void Run()
        {
            _client.OnMessage += Bot_OnMessage;
            _client.StartReceiving();
            Console.ReadLine();
        }
    }
}