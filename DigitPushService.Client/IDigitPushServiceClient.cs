namespace DigitPushService.Client
{
    public interface IDigitPushServiceClient
    {
        IPushCollection Push { get;  }

        IPushChannelsCollection PushChannels { get; }
    }
}