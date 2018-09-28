namespace DigitPushService.Client
{
    public interface IPushChannelsCollection
    {
        IPushChannelsApi this[string userId] { get; }
    }
}