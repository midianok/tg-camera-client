using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Model;

namespace IpCameraClient.Infrastructure.Services
{
    public class GetRecordService : IGetRecordService
    {
        public Record GetPhoto(Camera camera)
        {
            var content = GetPhotoFromCamera(camera).Result;
            return new Record
            {
                Camera = camera,
                ContentName = $"{camera.Model}_{DateTime.Now:ddMMyyyy-H-mm-ss}.jpg",
                ContentType = ContentType.Image,
                DateTime = DateTime.Now,
                Content = content
            };
        }

        public Record GetVideo(Camera camera)
        {
            byte[] content;
            using (var images = new MagickImageCollection())
            {
                var photos = GetPhotoFromCamera(camera, 25).Result;

                foreach (var photo in photos)
                    images.Add(new MagickImage(photo));

                images.OptimizePlus();
                content = images.ToByteArray(MagickFormat.Gif);
            }
            
            return new Record
            {
                Camera = camera,
                ContentName = $"{camera.Model}_{DateTime.Now:ddMMyyyy-H-mm-ss}.gif",
                ContentType = ContentType.Gif,
                DateTime = DateTime.Now,
                Content = content
            };
        }
        
        private async Task<byte[]> GetPhotoFromCamera(Camera camera)
        {
            byte[] photo;

            using (var httpClient = new HttpClient())
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(camera.Auth));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                photo = await httpClient.GetByteArrayAsync(camera.CameraUrl);
            };

            return photo;
        }

        private async Task<List<byte[]>> GetPhotoFromCamera(Camera camera, int imagesCount)
        {
            var images = new List<byte[]>();

            using (var httpClient = new HttpClient())
            {
                for (var i = 0; i < imagesCount; i++)
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(camera.Auth));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                    var image = await httpClient.GetByteArrayAsync(camera.CameraUrl);
                    images.Add(image);
                }
            };

            return images;
        }
    }
}