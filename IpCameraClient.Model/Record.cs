using System;
using System.ComponentModel.DataAnnotations;

namespace IpCameraClient.Model
{
    public class Record
    {
        public int RecordId { get; private set; }

        public DateTime DateTime { get; set; }

        public ContentType ContentType { get; set; }

        public string ContentLocation { get; set; }

        public virtual Camera Camera { get; set; }
    }

    public enum ContentType
    {
        IMAGE,

        VIDEO,

        AUDIO
    }
}
