using Digit.DeviceSynchronization.Impl;
using System.Threading.Tasks;

namespace DigitPushService.Client
{
    public interface IDigitSyncApi
    {
        Task Location(LocationSyncRequest locationSyncRequest);
        Task Device(DeviceSyncRequest deviceSyncRequest);
    }
}