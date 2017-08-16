using IpCameraClient.Model;
using IpCameraClient.WebFacade.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Infrastructure.Services;
using IpCameraClient.Model.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IpCameraClient.WebFacade.Controllers
{
    public class TelegramBotController : Controller
    {
        private const string StartCommand = "/start";

        private readonly IRepository<Camera> _cameras;
        private readonly IRepository<Record> _records;
        private readonly IRepository<TelegramUser> _users;
        private readonly IRecordSaverService _recordSaverService;
        private readonly IGetRecordService _getRecordService;
        private readonly Settings _settings;

        public TelegramBotController(
            IRepository<Camera> cameras,
            IRepository<Record> records,
            IRepository<TelegramUser> users,
            IRecordSaverService recordSaverService,
            IOptions<Settings> settings)
        {
            _cameras = cameras;
            _records = records;
            _getRecordService = new GetRecordService();
            _users = users;
            _recordSaverService = recordSaverService;
            _settings = settings.Value;
        }

        [HttpPost]
        [IpWhitelist("149.154.167.197", "149.154.167.233")]
        public async Task<IActionResult> Message([FromBody]Update update)
        {
            var accessedUserNames = _users.GetAll().Select(x => x.TelegramUserName);
            if (!accessedUserNames.Contains(update.Message.Chat.Username) &&
                update.Message.Type != MessageType.TextMessage) return NotFound();

            switch (update.Message.Text)
            {
                case StartCommand:
                    await ShowCamerasKeyboardAsync(update);
                    break;
                case var btnTxt when btnTxt.Contains(Emoji.Photo):
                    await SendPhotoAsync(update);
                    break;
                case var btnTxt when btnTxt.Contains(Emoji.Gif):
                    await SendGifAsync(update);
                    break;
            }

            return Ok();
        }

        private async Task SendGifAsync(Update update)
        {
            var cameraId = int.Parse(update.Message.Text.Split().Last());
            var camera = _cameras.GetById(cameraId);
            var record = _getRecordService.GetVideo(camera);

            _records.Add(record);
            _records.SaveChanges();

            _recordSaverService.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            using (var file = new MemoryStream(record.Content))
            {
                await Bot.Api.SendDocumentAsync(update.Message.Chat.Id, new FileToSend(record.ContentName, file));
            }
        }

        private async Task SendPhotoAsync(Update update)
        {
            var cameraId = int.Parse(update.Message.Text.Split().Last());
            var camera = _cameras.GetById(cameraId);
            var record = _getRecordService.GetVideo(camera);

            _records.Add(record);
            _records.SaveChanges();

            _recordSaverService.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            using (var file = new MemoryStream(record.Content))
            {
                await Bot.Api.SendPhotoAsync(update.Message.Chat.Id, new FileToSend(record.ContentName, file));
            }
        }

        private Task ShowCamerasKeyboardAsync(Update update)
        {
            var cameraListKm = new TelegramKeyboardMarkup();

            foreach (var camera in _cameras.GetAll())
                cameraListKm.Keyboard.Add(new List<TelegramKeyboardButton>
                {
                    new TelegramKeyboardButton { Text = $"{Emoji.Photo} {camera.Model} {camera.CameraId}" },
                    new TelegramKeyboardButton { Text = $"{Emoji.Gif} {camera.Model} {camera.CameraId}" },
                });

            return Bot.Api.SendTextMessageAsync(update.Message.Chat.Id, Emoji.Camera, replyMarkup: cameraListKm);
        }
    }

    
}
