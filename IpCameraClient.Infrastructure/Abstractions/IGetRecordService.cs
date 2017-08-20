using System.Threading.Tasks;
using IpCameraClient.Model;

namespace IpCameraClient.Infrastructure.Abstractions
{
    public interface IGetRecordService
    {
        Task<Record> GetImage(Camera camera);
        Task<Record> GetVideo(Camera camera);
    }
}