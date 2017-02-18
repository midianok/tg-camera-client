using System;
using System.Collections.Generic;
using System.Text;

namespace IpCameraClient.Abstractions
{
    public interface IDataProvider
    {
        void WriteData(string destintation, byte[] data);
        byte[] GetData(string path);

    }
}
