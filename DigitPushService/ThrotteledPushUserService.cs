using Microsoft.Extensions.Logging;
using PushServer.Abstractions.Services;
using PushServer.PushConfiguration.Abstractions.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Digit.DeviceSynchronization.Impl
{
    public class ThrotteledPushService : IThrotteledPushService
    {
        private static readonly ConcurrentDictionary<string, ThrotteledPushUserService> throtteledPushUserServices = new ConcurrentDictionary<string, ThrotteledPushUserService>();
        private readonly ILogger<ThrotteledPushService> logger;
        private readonly IServiceProvider serviceProvider;

        public ThrotteledPushService(ILogger<ThrotteledPushService> logger, IServiceProvider serviceProvider)

        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        public async Task PushThrotteled(string userId, ISyncRequest pushRequest)
        {
            var throtteledPushUserService = throtteledPushUserServices.GetOrAdd(userId,
                new ThrotteledPushUserService(userId, logger, serviceProvider));
            await throtteledPushUserService.EnqueuePush(pushRequest);
        }

        private class ThrotteledPushUserService
        {
            private static readonly List<ISyncRequest> pushRequests = new List<ISyncRequest>();
            private static readonly SemaphoreSlim sem = new SemaphoreSlim(1);

            private readonly string userId;
            private readonly ILogger<ThrotteledPushService> logger;
            private readonly IServiceProvider serviceProvider;
            private Timer _timer;
            private static readonly TimeSpan ThrottleTime = TimeSpan.FromSeconds(5);

            public ThrotteledPushUserService(string userId,
                ILogger<ThrotteledPushService> logger, IServiceProvider serviceProvider)
            {
                this.userId = userId;
                this.logger = logger;
                this.serviceProvider = serviceProvider;
            }

            public async Task EnqueuePush(ISyncRequest req)
            {
                await sem.WaitAsync();
                if (null == _timer)
                {
                    _timer = new Timer(async state =>
                    {
                        await sem.WaitAsync();
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var pushConfigurationStore = scope.ServiceProvider.GetService<IPushConfigurationStore>();
                            var pushService = scope.ServiceProvider.GetService<IPushService>();
                            try
                            {
                                var reqs = pushRequests.ToArray();
                                if (reqs.Length > 0)
                                {
                                    var grouped = (await Task.WhenAll(reqs.Select(async request =>
                                    {
                                        var channels = await pushConfigurationStore.GetForOptionsAsync(userId, request.GetChannelOptions());
                                        return new { request, channels };
                                    })))
                                    .Where(r => null != r.channels && r.channels.Any())
                                    .GroupBy(r => r.channels.First().Id, v => v.request);
                                    foreach (var group in grouped)
                                    {
                                        var collection = new PushRequestCollection(group);
                                        try
                                        {
                                            await pushService.Push(group.Key, collection.GetPayload(), collection.GetOptions());
                                        }
                                        catch (Exception e)
                                        {
                                            logger.LogError(e, $"Could not push {collection.GetPayload()} to {group.Key}");
                                        }
                                    }
                                    pushRequests.Clear();
                                }
                            }
                            finally
                            {
                                _timer = null;
                                sem.Release();
                            }
                        }
                    }, null, (int)ThrottleTime.TotalMilliseconds, Timeout.Infinite);
                }
                pushRequests.Add(req);
                sem.Release();
            }
        }
    }
}
