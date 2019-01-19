using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IpCameraClient.Core
{
    public class GetRecordService : IGetRecordService
    {
        private readonly string _cameraImageUrl;
        private readonly string _auth;

        public GetRecordService(string cameraImageUrl,  string auth)
        {
            _cameraImageUrl = cameraImageUrl;
            _auth = auth;
        }
        public async Task<byte[]> GetImage()
        {
            using (var httpClient = new HttpClient())
            {
                var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(_auth));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                return await httpClient.GetByteArrayAsync(_cameraImageUrl);
            };
        }
    }
}