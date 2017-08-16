using System.IO;
using IpCameraClient.Infrastructure.Abstractions;

namespace IpCameraClient.Infrastructure.Services
{
    public class FileSystemRecordSaverService : IRecordSaverService
    {
        public byte[] GetData(string path) => 
            File.ReadAllBytes(path);

        public void WriteData(string destintation, byte[] data) =>
            File.WriteAllBytes(destintation, data);
    }
}
