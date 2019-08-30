namespace DigitPushService.Client
{
    public interface IUserApi
    {
        IPushApi Push { get; }
        IPushChannelsApi PushChannels { get; }
        IDigitSyncApi DigitSync { get; }
    }
}