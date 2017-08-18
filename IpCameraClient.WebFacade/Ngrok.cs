using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace IpCameraClient.WebFacade
{
    public static class Ngrok
    {
        private const string ApiTunnelsUrl = @"http://127.0.0.1:4040/api/tunnels/command_line";
        public static string GetTunnelUrl()
        {
            using (var client = new HttpClient())
            {
                var responce = client.GetStringAsync(ApiTunnelsUrl).Result;
                var tunnel = JsonConvert.DeserializeObject<Tunnel>(responce);
                return tunnel.PublicUrl;
            }
        }

        public class Tunnel
        {
            [JsonProperty(PropertyName = "public_url")]
            public string PublicUrl { get; set; }
        }
    }
}
