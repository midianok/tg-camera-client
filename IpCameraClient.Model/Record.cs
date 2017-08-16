using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IpCameraClient.Model
{
    public class Record
    {
        public int RecordId { get; set; }

        public DateTime DateTime { get; set; }

        public ContentType ContentType { get; set; }

        public string ContentName { get; set; }

        public virtual Camera Camera { get; set; }

        [NotMapped]
        public byte[] Content { get; set; }
    }

    public enum ContentType
    {
        Image,

        Video,

        Audio,

        Gif
    }
}
