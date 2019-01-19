using IpCameraClient.WebFacade.Filters;
using Microsoft.AspNetCore.Mvc;
using IpCameraClient.Core;
using Telegram.Bot.Types;

namespace IpCameraClient.WebFacade.Controllers
{
    public class TelegramBotController : Controller
    {
        private readonly ITelegramService _telegramService;

        public TelegramBotController(ITelegramService telegramService)
        {
            _telegramService = telegramService;
        }

        [HttpPost]
        [IpWhitelist("149.154.167.197", "149.154.167.233")]
        public IActionResult Message([FromBody]Update update)
        {
             _telegramService.ProcessMessageAsync(update.Message);
            return Ok();
        }

        
    }

    
}
