using System;
using System.Collections.Generic;
using System.Text;

namespace IpCameraClient.Db
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string ImgLocation { get; set; }
    }
}
