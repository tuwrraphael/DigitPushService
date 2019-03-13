using PushServer.PushConfiguration.Abstractions.Models;
using System.Threading.Tasks;

namespace DigitPushService.Client
{
    public interface IChannelOptions
    {
        Task PutAsync(PushChannelOptions options);
    }
}