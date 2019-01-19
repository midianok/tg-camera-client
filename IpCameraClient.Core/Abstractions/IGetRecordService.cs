using System.Threading.Tasks;

namespace IpCameraClient.Core
{
    public interface IGetRecordService
    {
        Task<byte[]> GetImage();
    }
}