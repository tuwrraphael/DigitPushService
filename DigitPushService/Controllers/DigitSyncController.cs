using System.Threading.Tasks;
using Digit.DeviceSynchronization.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class DigitSyncController : ControllerBase
    {
        private readonly IThrotteledPushService throtteledPushService;

        public DigitSyncController(IThrotteledPushService throtteledPushService)
        {
            this.throtteledPushService = throtteledPushService;
        }

        [Authorize("User")]
        [HttpPost("me/digit-sync/location")]
        public async Task<IActionResult> Location([FromBody]LocationSyncRequest request)
        {
            string userId = User.GetId();
            await throtteledPushService.PushThrotteled(userId, request);
            return StatusCode(201);
        }

        [Authorize("Service")]
        [HttpPost("{userId}/digit-sync/location")]
        public async Task<IActionResult> Location(string userId, [FromBody]LocationSyncRequest request)
        {
            await throtteledPushService.PushThrotteled(userId, request);
            return StatusCode(201);
        }

        [Authorize("User")]
        [HttpPost("me/digit-sync/device")]
        public async Task<IActionResult> Device([FromBody]DeviceSyncRequest request)
        {
            string userId = User.GetId();
            await throtteledPushService.PushThrotteled(userId, request);
            return StatusCode(201);
        }

        [Authorize("Service")]
        [HttpPost("{userId}/digit-sync/Device")]
        public async Task<IActionResult> Push(string userId, [FromBody]DeviceSyncRequest request)
        {
            await throtteledPushService.PushThrotteled(userId, request);
            return StatusCode(201);
        }
    }
}
