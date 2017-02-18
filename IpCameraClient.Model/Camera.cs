using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IpCameraClient.Model
{
    public class Camera
    {

        public int CameraId { get; private set; }

        public string Model { get; set; }

        public string CameraUrl { get; set; }

        public string Auth { get; set; }

        public List<Record> Records { get; set; }

        public async Task<Record> GetPhotoAsync()
        {
            byte[] photo;
            using (var httpClient = new HttpClient())
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Auth));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                photo = await httpClient.GetByteArrayAsync(CameraUrl);
            };

            return new Record
            {
                Camera = this,
                ContentName = $"{Model}_{DateTime.Now.ToString("ddMMyyyy-H-mm")}.jpg",
                ContentType = ContentType.IMAGE,
                DateTime = DateTime.Now,
                Content = photo
            };
        }

    }
}
