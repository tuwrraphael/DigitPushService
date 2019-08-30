using System.Collections.Generic;

namespace Digit.DeviceSynchronization.Impl
{
    public interface ISyncRequest
    {
        IDictionary<string, string> GetChannelOptions();
        IEnumerable<string> GetSyncActions();

        bool HighPriority { get; }
    }
}
