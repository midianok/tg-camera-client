using IpCameraClient.Abstractions;
using IpCameraClient.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace IpCameraClient.WebFacade.Controllers
{
    public class RemoteManagmentController : Controller
    {
        private readonly IRepository<Camera> _cameras;
        private readonly IRepository<Record> _records;
        private readonly IDataProvider _dataProvider;
        private readonly Settings _settings;
        public string _ipWhiteList { get; set; }

        public RemoteManagmentController(
            IRepository<Camera> cameras,
            IRepository<Record> records,
            IRepository<TelegramUser> users,
            IDataProvider dataProvider,
            IOptions<Settings> settings)
        {
            _cameras = cameras;
            _records = records;
            _dataProvider = dataProvider;
            _settings = settings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var record = await _cameras.Entities
                .Single(x => x.CameraId == id)
                .GetPhotoAsync();

            _records.Add(record);
            _dataProvider.WriteData($"{_settings.ContentFolderName}/{record.ContentName}", record.Content);

            return Ok();
        }
    }
}