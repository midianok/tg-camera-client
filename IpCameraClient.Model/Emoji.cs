using System.Text;

namespace IpCameraClient.Model
{
    public static class Emoji
    {
        static readonly byte[] _camera = { 240, 159, 147, 185 };
        public static string Camera => Encoding.UTF8.GetString(_camera);
        
        static readonly byte[] _photo = { 240, 159, 147, 183 };
        public static string Photo => Encoding.UTF8.GetString(_photo);

        static readonly byte[] _gif = { 240, 159, 142, 172 };
        public static string Gif => Encoding.UTF8.GetString(_gif);

    }
}
