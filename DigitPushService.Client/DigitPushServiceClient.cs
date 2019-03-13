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

        public IPushCollection Push => new PushCollection(ClientFactory);

        public IPushChannelsCollection PushChannels => new PushChannelsCollection(ClientFactory);

        private class PushChannelsCollection : IPushChannelsCollection
        {
            private readonly Func<Task<HttpClient>> clientFactory;

            public PushChannelsCollection(Func<Task<HttpClient>> clientFactory)
            {
                this.clientFactory = clientFactory;
            }

            public IPushChannelsApi this[string userId] => new PushChannelsApi(clientFactory, userId);
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

            public IPushChannelApi this[string channelId] => throw new NotImplementedException();

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

        private class PushCollection : IPushCollection
        {
            private readonly Func<Task<HttpClient>> clientFactory;

            public PushCollection(Func<Task<HttpClient>> clientFactory)
            {
                this.clientFactory = clientFactory;
            }

            public IPushApi this[string userId] => new PushApi(clientFactory, userId);
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
    }
}
