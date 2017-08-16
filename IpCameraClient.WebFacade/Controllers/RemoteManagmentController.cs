using IpCameraClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Infrastructure.Services;
using IpCameraClient.Model.Telegram;

namespace IpCameraClient.WebFacade.Controllers
{
    public class RemoteManagmentController : Controller
    {
        private readonly IRepository<Camera> _cameras;
        private readonly IRepository<Record> _records;
        private readonly IRecordSaverService _recordSaverService;
        private readonly IGetRecordService _getRecordService;
        private readonly Settings _settings;

        public RemoteManagmentController(
            IRepository<Camera> cameras,
            IRepository<Record> records,
            IRepository<TelegramUser> users,
            IRecordSaverService recordSaverService,
            IOptions<Settings> settings)
        {
            _cameras = cameras;
            _records = records;
            _getRecordService = new GetRecordService();
            _recordSaverService = recordSaverService;
            _settings = settings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var camera = _cameras.GetById(id);
            var record = _getRecordService.GetPhoto(camera);

            _records.Add(record);
            _records.SaveChanges();
            _recordSaverService.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            return Ok();
        }
    }
}