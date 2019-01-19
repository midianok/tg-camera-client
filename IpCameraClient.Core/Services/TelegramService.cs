using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IpCameraClient.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace IpCameraClient.Core
{
    public class TelegramService : ITelegramService
    {
        private const string StartCommand = "/start";
        
        private readonly TelegramBotClient _client;
        private readonly IGetRecordService _getRecordService;
        private readonly IList<string> _telegramUsersWhiteList;
        
        public TelegramService(IGetRecordService getRecordService, 
            IList<string> telegramUsersWhiteList, 
            TelegramBotClient client)
        {
            _getRecordService = getRecordService;
            _telegramUsersWhiteList = telegramUsersWhiteList;
            _client = client;
        }

        public async void ProcessMessageAsync(Message message)
        {
            if (!_telegramUsersWhiteList.Contains(message.Chat.Username) || message.Type != MessageType.Text)
                return;
            
            switch (message.Text)
            {
                case StartCommand:
                    await ShowCameraKeyboardByUserIdAsync(message.Chat.Id);
                    break;
                case var btnTxt when btnTxt.Contains(Emoji.Photo):
                    await SendPhotoByUserIdAsync(message.Chat.Id);
                    break;
            }
        }
        
        private async Task SendPhotoByUserIdAsync(long userId)
        {
            var image = await _getRecordService.GetImage();

            using (var file = new MemoryStream(image))
            {
                await _client.SendPhotoAsync(userId, new InputOnlineFile(file));
            }
        }

        private Task ShowCameraKeyboardByUserIdAsync(long userId)
        {
            var cameraListKm = new TelegramKeyboardMarkup();

            cameraListKm.Keyboard.Add(new List<TelegramKeyboardButton>
            {
                new TelegramKeyboardButton { Text = Emoji.Photo },
            });

            return _client.SendTextMessageAsync(userId, Emoji.Camera, replyMarkup: cameraListKm);
        }
    }
}