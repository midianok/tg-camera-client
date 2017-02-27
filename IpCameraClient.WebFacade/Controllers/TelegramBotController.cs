using IpCameraClient.Abstractions;
using IpCameraClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using SysFile = System.IO.File;
using System.IO;
using System;
using Telegram.Bot.Types.Enums;

namespace IpCameraClient.WebFacade.Controllers
{
    [Route("api/[controller]")]
    public class TelegramBotController : Controller
    {
        private const string GET_PHOTO_COMMAND = "Photo";
        private const string GET_GIF_COMMAND = "Gif";
        private const string START_COMMAND = "/start";

        private readonly IRepository<Camera> _cameras;
        private readonly IRepository<Record> _records;
        private readonly IRepository<TelegramUser> _users;
        private readonly IDataProvider _dataProvider;
        private readonly Settings _settings;
        public string _ipWhiteList { get; set; }

        public TelegramBotController(
            IRepository<Camera> cameras,
            IRepository<Record> records,
            IRepository<TelegramUser> users,
            IDataProvider dataProvider,
            IOptions<Settings> settings)
        {
            _cameras = cameras;
            _records = records;
            _users = users;
            _dataProvider = dataProvider;
            _settings = settings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var accessedUserNames = _users.Entities.Select(x => x.TelegramUserName);
            if (!accessedUserNames.Contains(update.Message.Chat.Username) &&
                update.Message.Type != MessageType.TextMessage) return NotFound();

            switch (update.Message.Text)
            {
                case START_COMMAND:
                    await ShowCamerasKeyboardAsync(update);
                    break;
                case string replyStr when replyStr.Contains(Emoji.Camera):
                    await ShowCameraControlKeyboardAsync(update);
                    break;
                case GET_PHOTO_COMMAND:
                    await SendPhotoAsync(update);
                    break;
                case GET_GIF_COMMAND:
                    await SendGifAsync(update);
                    break;
                default:
                    break;
            }

            return Ok();
        }

        private async Task SendGifAsync(Update update)
        {
            var cameraId = int.Parse(update.Message.Text.Split(' ')[1]);
            var camera = _cameras.Entities.Single(x => x.CameraId == cameraId);

            var record = await camera.GetGifAsync();
            _records.Add(record);

            _dataProvider.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            using (var file = new MemoryStream(record.Content))
            {
                await Bot.Api.SendDocumentAsync(update.Message.Chat.Id, new FileToSend(record.ContentName, file));
            }

            await ShowCamerasKeyboardAsync(update);
        }

        private async Task SendPhotoAsync(Update update)
        {
            var cameraId = int.Parse(update.Message.Text.Split(' ')[1]);
            var camera = _cameras.Entities.Single(x => x.CameraId == cameraId);

            var record = await camera.GetPhotoAsync();
            _records.Add(record);

            _dataProvider.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            using (var file = new MemoryStream(record.Content))
            {
                await Bot.Api.SendPhotoAsync(update.Message.Chat.Id, new FileToSend(record.ContentName, file));
            }

            await ShowCamerasKeyboardAsync(update);
        }

        private Task ShowCamerasKeyboardAsync(Update update)
        {
            var cameraListKM = new TelegramKeyboardMarkup();
            foreach (var camera in _cameras.Entities)
                cameraListKM.Keyboard.Add(new List<TelegramKeyboardButton> { new TelegramKeyboardButton { Text = $"{Emoji.Camera} {camera.CameraId} {camera.Model}" } });

            return Bot.Api.SendTextMessageAsync(update.Message.Chat.Id, "Choose Camera", replyMarkup: cameraListKM);
        }

        private Task ShowCameraControlKeyboardAsync(Update update)
        {
            var cameraActions = new TelegramKeyboardMarkup();
            cameraActions.Keyboard.Add(new List<TelegramKeyboardButton> {
                new TelegramKeyboardButton { Text = $"Photo" },
                new TelegramKeyboardButton { Text = $"Gif" }
            });
            return Bot.Api.SendTextMessageAsync(update.Message.Chat.Id, "What to do", replyMarkup: cameraActions);
        }
    }

    
}
