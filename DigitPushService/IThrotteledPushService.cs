using System.Threading.Tasks;

namespace Digit.DeviceSynchronization.Impl
{
    public interface IThrotteledPushService
    {
        Task PushThrotteled(string userId, ISyncRequest pushRequest);
    }
}