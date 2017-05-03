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
                ContentName = $"{Model}_{DateTime.Now:ddMMyyyy-H-mm-ss}.jpg",
                ContentType = ContentType.IMAGE,
                DateTime = DateTime.Now,
                Content = await GetPhotoFromCamera()
            };
        }

        public async Task<Record> GetGifAsync()
        {
            byte[] content;
            using (var images = new MagickImageCollection())
            {
                var photos = await GetPhotoFromCamera(25);

                foreach (var photo in photos)
                    images.Add(new MagickImage(photo));

                images.OptimizePlus();
                content = images.ToByteArray(MagickFormat.Gif);
            }
            
            return new Record
            {
                Camera = this,
                ContentName = $"{Model}_{DateTime.Now:ddMMyyyy-H-mm-ss}.gif",
                ContentType = ContentType.GIF,
                DateTime = DateTime.Now,
                Content = content
            };
        }

        private async Task<byte[]> GetPhotoFromCamera()
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

        private async Task<List<byte[]>> GetPhotoFromCamera(int imagesCount)
        {
            var images = new List<byte[]>();

            using (var httpClient = new HttpClient())
            {
                for (var i = 0; i < imagesCount; i++)
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(Auth));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    var image = await httpClient.GetByteArrayAsync(CameraUrl);
                    images.Add(image);
                }
            };

            return images;
        }

    }
}
