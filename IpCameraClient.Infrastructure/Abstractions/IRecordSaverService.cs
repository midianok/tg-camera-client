namespace IpCameraClient.Infrastructure.Abstractions
{
    public interface IRecordSaverService
    {
        void WriteData(string destintation, byte[] data);
        byte[] GetData(string path);

    }
}
