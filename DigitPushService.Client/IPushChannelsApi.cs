using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace DigitPushService.Client
{
    public interface IPushChannelsApi
    {
        Task<PushChannelConfiguration[]> GetAllAsync();

        IPushChannelApi this[string channelId] { get; }
    }
}