using Digit.DeviceSynchronization.Impl;
using DigitPushService.Models;
using Newtonsoft.Json;
using OAuthApiClient.Abstractions;
using PushServer.PushConfiguration.Abstractions.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DigitPushService.Client
{
    public class DigitPushServiceClient : IDigitPushServiceClient
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly HttpClient httpClient;

        private async Task<HttpClient> ClientFactory()
        {
            await authenticationProvider.AuthenticateClient(httpClient);
            return httpClient;
        }

        public DigitPushServiceClient(IAuthenticationProvider authenticationProvider,
            HttpClient httpClient)
        {
            this.authenticationProvider = authenticationProvider;
            this.httpClient = httpClient;
        }

        public IUserApi this[string userId] => new UserApi(ClientFactory, userId);

        private class UserApi : IUserApi
        {
            private readonly Func<Task<HttpClient>> clientFactory;
            private readonly string userId;

            public UserApi(Func<Task<HttpClient>> clientFactory, string userId)
            {
                this.clientFactory = clientFactory;
                this.userId = userId;
            }

            public IPushChannelsApi this[string userId] => new PushChannelsApi(clientFactory, userId);

            public IPushApi Push => new PushApi(clientFactory, userId);

            public IPushChannelsApi PushChannels => new PushChannelsApi(clientFactory, userId);

            public IDigitSyncApi DigitSync => new DigitSyncApi(clientFactory, userId);
        }

        private class PushChannelsApi : IPushChannelsApi
        {
            private readonly Func<Task<HttpClient>> clientFactory;
            private string userId;

            public PushChannelsApi(Func<Task<HttpClient>> clientFactory, string userId)
            {
                this.clientFactory = clientFactory;
                this.userId = userId;
            }

            public IPushChannelApi this[string channelId] => new PushChannelApi(clientFactory, userId, channelId);

            public async Task<PushChannelConfiguration[]> GetAllAsync()
            {
                var client = await clientFactory();
                var res = await client.GetAsync($"api/{userId}/pushchannels");
                if (!res.IsSuccessStatusCode)
                {
                    throw new DigitPushServiceException($"Push creation request resulted in {res.StatusCode}.");
                }
                var content = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PushChannelConfiguration[]>(content);
            }
        }

        private class PushChannelApi : IPushChannelApi, IChannelOptions
        {
            private readonly Func<Task<HttpClient>> clientFactory;
            private readonly string userId;
            private readonly string channelId;

            public PushChannelApi(Func<Task<HttpClient>> clientFactory, string userId, string channelId)
            {
                this.clientFactory = clientFactory;
                this.userId = userId;
                this.channelId = channelId;
            }

            public IChannelOptions Options => this;

            public async Task PutAsync(PushChannelOptions options)
            {
                var client = await clientFactory();
                var json = JsonConvert.SerializeObject(options);
                var res = await client.PutAsync($"api/{userId}/pushchannels/{channelId}/options",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (!res.IsSuccessStatusCode)
                {
                    throw new DigitPushServiceException($"Update channel options resulted in {res.StatusCode}.");
                }
            }
        }

        private class PushApi : IPushApi
        {
            private readonly Func<Task<HttpClient>> clientFactory;
            private readonly string userId;

            public PushApi(Func<Task<HttpClient>> clientFactory, string userId)
            {
                this.clientFactory = clientFactory;
                this.userId = userId;
            }

            public async Task Create(PushRequest pushRequest)
            {
                var client = await clientFactory();
                var json = JsonConvert.SerializeObject(pushRequest);
                var res = await client.PostAsync($"api/{userId}/push",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (!res.IsSuccessStatusCode)
                {
                    throw new DigitPushServiceException($"Push creation request resulted in {res.StatusCode}.");
                }
            }
        }

        private class DigitSyncApi : IDigitSyncApi
        {
            private readonly Func<Task<HttpClient>> clientFactory;
            private readonly string userId;

            public DigitSyncApi(Func<Task<HttpClient>> clientFactory, string userId)
            {
                this.clientFactory = clientFactory;
                this.userId = userId;
            }

            public async Task Device(DeviceSyncRequest deviceSyncRequest)
            {
                var client = await clientFactory();
                var json = JsonConvert.SerializeObject(deviceSyncRequest);
                var res = await client.PostAsync($"api/{userId}/digit-sync/device",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (!res.IsSuccessStatusCode)
                {
                    throw new DigitPushServiceException($"Push creation request resulted in {res.StatusCode}.");
                }
            }

            public async Task Location(LocationSyncRequest locationSyncRequest)
            {
                var client = await clientFactory();
                var json = JsonConvert.SerializeObject(locationSyncRequest);
                var res = await client.PostAsync($"api/{userId}/digit-sync/location",
                    new StringContent(json, Encoding.UTF8, "application/json"));
                if (!res.IsSuccessStatusCode)
                {
                    throw new DigitPushServiceException($"Push creation request resulted in {res.StatusCode}.");
                }
            }
        }
    }
}
