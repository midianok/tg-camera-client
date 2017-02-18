using IpCameraClient.Abstractions;
using System;
using System.IO;

namespace IpCameraClient.Model
{
    public class FileSystemDataProvider : IDataProvider
    {
        public byte[] GetData(string path)
            => File.ReadAllBytes(path);

        public void WriteData(string destintation, byte[] data) =>
            File.WriteAllBytes(destintation, data);
    }
}
