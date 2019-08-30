using System.Collections.Generic;

namespace Digit.DeviceSynchronization.Impl
{
    public class DeviceSyncRequest : ISyncRequest
    {
        public string DeviceId { get; set; }

        public bool HighPriority => true;

        public IDictionary<string, string> GetChannelOptions()
        {
            return new Dictionary<string, string>() { { $"digit.sync.{DeviceId}", null } };
        }

        public IEnumerable<string> GetSyncActions()
        {
            return new List<string>() { $"deviceSync.{DeviceId}" };
        }
    }
}
