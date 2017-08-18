using System.Collections.Generic;
using LiteDB;

namespace IpCameraClient.Model
{
    public class Camera
    {
        public int Id { get; set; }

        public string Model { get; set; }

        public string CameraUrl { get; set; }

        public string Auth { get; set; }

        public List<Record> Records { get; set; }

    }
}
