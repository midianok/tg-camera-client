using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using IpCameraClient.WebFacade.Filters;
using Microsoft.Extensions.Options;
using IpCameraClient.Abstractions;
using System.Linq;
using IpCameraClient.Model;

namespace IpCameraClient.WebFacade.Controllers
{
    [Route("api/[controller]")]
    public class TelegramBotController : Controller
    {
        private readonly IRepository<Camera> _cameras;
        private readonly IRepository<Record> _records;

        public TelegramBotController(IOptions<AppSettings> optionsAccessor, IRepository<Camera> cameras, IRepository<Record> records)
        {
            _cameras = cameras;
            _records = records;

        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Update update)
        {
            var camera = _cameras.GetAll().First();
            var record = await camera.GetRecordAsync();
            _records.Add(record);

            using(var file = System.IO.File.Open($"./CameraImages/{record.ContentLocation}", System.IO.FileMode.Open))
            {
                await Program.Bot.client.SendPhotoAsync(update.Message.Chat.Id, new FileToSend("Photo.jpg",file));
            }

            return Ok();
        }

        [IpWhitelist(new[] {""})]
        [HttpGet("{value}")]
        public IActionResult Get(string value)
        {

            return new JsonResult(value);
        }
    }
}
