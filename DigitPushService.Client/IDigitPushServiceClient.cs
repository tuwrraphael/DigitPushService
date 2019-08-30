namespace DigitPushService.Client
{
    public interface IDigitPushServiceClient
    {
        IUserApi this[string userId] { get; }
    }
}