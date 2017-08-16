using IpCameraClient.Model;

namespace IpCameraClient.Infrastructure.Abstractions
{
    public interface IGetRecordService
    {
        Record GetPhoto(Camera camera);
        Record GetVideo(Camera camera);
    }
}