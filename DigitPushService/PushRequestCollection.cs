using PushServer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Digit.DeviceSynchronization.Impl
{
    internal class PushRequestCollection
    {
        private readonly IEnumerable<ISyncRequest> _requests;

        public PushRequestCollection(IEnumerable<ISyncRequest> requests)
        {
            _requests = requests;
        }

        public string GetPayload()
        {
            var actions = string.Join(',', _requests.SelectMany(v => v.GetSyncActions())
                .Distinct()
                .Select(v => $"'{v}'").ToArray());
            return $"{{'actions': [{actions}]}}";
        }

        public PushOptions GetOptions()
        {
            return new PushOptions()
            {
                Urgency = _requests.Any(v => v.HighPriority) ? PushUrgency.High : PushUrgency.Normal
            };
        }
    }
}
