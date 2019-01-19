using System.Collections.Generic;
using System.Runtime.Serialization;
using Telegram.Bot.Types.ReplyMarkups;

namespace IpCameraClient.Core
{
    [DataContract]
    public class TelegramKeyboardMarkup : IReplyMarkup
    {
        [DataMember(Name = "keyboard")]
        public List<List<TelegramKeyboardButton>> Keyboard { get; set; }
        [DataMember(Name = "resize_keyboard")]
        public bool Resize { get; set; }
        [DataMember(Name = "one_time_keyboard")]
        public bool OneTime { get; set; }
        [DataMember(Name = "selective")]
        public bool Selective { get; set; }

        public TelegramKeyboardMarkup()
        {
            Keyboard = new List<List<TelegramKeyboardButton>>();
            Resize = true;
        }

    }

    [DataContract]
    public class TelegramKeyboardButton
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }
    }
}
