using System.Collections.Generic;

namespace Digit.DeviceSynchronization.Impl
{
    public class LocationSyncRequest : ISyncRequest
    {
        public bool HighPriority => false;

        public IDictionary<string, string> GetChannelOptions()
        {
            return new Dictionary<string, string>() { { "digitLocationRequest", null } };
        }

        public IEnumerable<string> GetSyncActions()
        {
            return new List<string>() { "locationSync" };
        }
    }
}
