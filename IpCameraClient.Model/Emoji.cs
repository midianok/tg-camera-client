
using System.Text;

namespace IpCameraClient.Model
{
    public static class Emoji
    {
        static readonly byte[] _camera = { 240, 159, 147, 185 };
        public static string Camera { get => Encoding.UTF8.GetString(_camera); } 
    }
}
