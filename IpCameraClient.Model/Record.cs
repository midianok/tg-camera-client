using System;
using System.ComponentModel.DataAnnotations;

namespace IpCameraClient.Model
{
    public class Record
    {
        public int RecordId { get; set; }

        public DateTime DateTime { get; set; }
        public string ImgLocation { get; set; }

        public virtual Camera Camera { get; set; }
    }
}
