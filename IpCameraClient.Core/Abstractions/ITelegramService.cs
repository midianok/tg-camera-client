using Telegram.Bot.Types;

namespace IpCameraClient.Core
{
    public interface ITelegramService
    {
        void ProcessMessageAsync(Message message);
    }
}