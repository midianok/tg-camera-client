using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IpCameraClient.Model
{
    public class Camera
    {
        public int CameraId { get; private set; }

        public string Model { get; set; }

        public List<Record> Records { get; set; }
    }
}
