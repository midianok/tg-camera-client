using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IpCameraClient.Infrastructure.Abstractions;
using IpCameraClient.Model;

namespace IpCameraClient.Infrastructure.Services
{
    public class GetRecordService : IGetRecordService
    {
        public async Task<Record> GetImage(Camera camera)
        {
            var content = await GetImageFromCamera(camera);
            return new Record
            {
                Camera = camera,
                ContentName = $"{camera.Model}_{DateTime.Now:ddMMyyyy-H-mm-ss}.jpg",
                ContentType = ContentType.Image,
                DateTime = DateTime.Now,
                Content = content
            };
        }

        public async Task<Record> GetVideo(Camera camera)
        {
            return null;
        }
        
        private async Task<byte[]> GetImageFromCamera(Camera camera)
        {
            using (var httpClient = new HttpClient())
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(camera.Auth));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                return await httpClient.GetByteArrayAsync(camera.CameraUrl);
            };
        }
    }
}