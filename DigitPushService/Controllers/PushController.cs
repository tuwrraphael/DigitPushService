using System.Net;
using System.Threading.Tasks;
using DigitPushService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PushServer.Abstractions;
using PushServer.Abstractions.Services;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushController : ControllerBase
    {
        private readonly IPushService pushService;
        private readonly ILogger<PushController> logger;
        private readonly IPushConfigurationManager pushConfigurationManager;

        public PushController(IPushService pushService,
            ILogger<PushController> logger, IPushConfigurationManager pushConfigurationManager)
        {
            this.pushService = pushService;
            this.logger = logger;
            this.pushConfigurationManager = pushConfigurationManager;
        }
        [Authorize("User")]
        [HttpPost("me/push")]
        public async Task<IActionResult> Push([FromBody]PushRequest request)
        {
            return await ExecutePush(User.GetId(), request);
        }
        [Authorize("Service")]
        [HttpPost("{userId}/push")]
        public async Task<IActionResult> Push(string userId, [FromBody]PushRequest request)
        {
            return await ExecutePush(userId, request);
        }

        private async Task HandleFailures(string userId, PushFailedException ex)
        {
            foreach (var failed in ex.Failures)
            {
                logger.LogError($"Push to {failed.Configuration.Id} failed.", failed.Exception);
                var response = (failed.Exception as PushException)?.ResponseMessage;
                if (HttpStatusCode.Gone == response?.StatusCode)
                {
                    await pushConfigurationManager.DeleteAsync(userId, failed.Configuration.Id);
                }
            }
        }

        private async Task<IActionResult> ExecutePush(string userId, PushRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                if (!string.IsNullOrEmpty(request.ChannelId))
                {
                    await pushService.Push(request.ChannelId, request.Payload, null);
                }
                else if (null != request.ChannelOptions)
                {
                    await pushService.Push(userId, request.ChannelOptions, request.Payload, null);
                }
                return StatusCode(201);
            }
            catch (PushConfigurationNotFoundException)
            {
                return NotFound();
            }
            catch (PushPartiallyFailedException ex)
            {
                await HandleFailures(userId, ex);
                return StatusCode(201);
            }
            catch (PushFailedException ex)
            {
                await HandleFailures(userId, ex);
                throw;
            }
        }
    }
}
