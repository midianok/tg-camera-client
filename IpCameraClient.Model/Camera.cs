using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IpCameraClient.Model
{
    public class Camera
    {
        //public Camera() => Records = new List<Record>();

        public int CameraId { get; set; }

        public string Model { get; set; }
        public List<Record> Records { get; set; }
    }
}
