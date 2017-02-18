using IpCameraClient.Abstractions;
using IpCameraClient.Model;
using IpCameraClient.WebFacade.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IpCameraClient.WebFacade.Controllers
{
    [Route("api/[controller]")]
    public class TelegramBotController : Controller
    {
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
        [IpWhitelist("149.154.167.197", "149.154.167.233")]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var accessedUserNames = _users.Entities.Select(x => x.TelegramUserName);
            if (!accessedUserNames.Contains(update.Message.Chat.Username) &&
                update.Message.Type != MessageType.TextMessage) return NotFound();

            var camera = _cameras.Entities.First();
            var record = await camera.GetPhotoAsync();
            _records.Add(record);

            _dataProvider.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            using (var file = new MemoryStream(record.Content))
            {
                await Bot.Api.SendPhotoAsync(update.Message.Chat.Id, new FileToSend(record.ContentName, file));
            }

            return Ok();
        }
    }
}
