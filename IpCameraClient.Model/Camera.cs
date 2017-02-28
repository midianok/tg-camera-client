using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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
            return new Record
            {
                Camera = this,
                ContentName = $"{Model}_{DateTime.Now.ToString("ddMMyyyy-H-mm-ss")}.jpg",
                ContentType = ContentType.IMAGE,
                DateTime = DateTime.Now,
                Content = await GetImageFromCamera()
            };
        }

        public async Task<Record> GetGifAsync()
        {
            byte[] content;
            using (var images = new MagickImageCollection())
            {
                for (int i = 0; i < 25; i++)
                {
                    images.Add(new MagickImage( await GetImageFromCamera()));
                    images[i].AnimationDelay = 15;
                }
                images.OptimizePlus();
                images.Write("temp.gif");
                content = File.ReadAllBytes("temp.gif");
                File.Delete("temp.gif");
            }
            
            return new Record
            {
                Camera = this,
                ContentName = $"{Model}_{DateTime.Now.ToString("ddMMyyyy-H-mm-ss")}.gif",
                ContentType = ContentType.GIF,
                DateTime = DateTime.Now,
                Content = content
            };
        }

        private async Task<byte[]> GetImageFromCamera()
        {
            byte[] photo;

            using (var httpClient = new HttpClient())
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Auth));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                photo = await httpClient.GetByteArrayAsync(CameraUrl);
            };

            return photo;
        }

    }
}
