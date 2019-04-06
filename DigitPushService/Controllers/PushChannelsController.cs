using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PushServer.Abstractions.Services;
using PushServer.AzureNotificationHub;
using PushServer.Firebase;
using PushServer.PushConfiguration.Abstractions.Models;
using PushServer.WebPush;

namespace DigitPushService.Controllers
{
    [Route("api")]
    public class PushChannelsController : ControllerBase
    {
        private readonly IPushConfigurationManager pushConfigurationManager;

        public PushChannelsController(IPushConfigurationManager pushConfigurationManager)
        {
            this.pushConfigurationManager = pushConfigurationManager;
        }

        [Authorize("User")]
        [HttpPost("me/pushchannels")]
        [Consumes("application/vnd+pushserver.firebase+json")]
        public async Task<IActionResult> Register([FromBody]FirebasePushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var config = await pushConfigurationManager.RegisterAsync(User.GetId(), registration);
            return Ok(config);
        }

        [Authorize("User")]
        [HttpPost("me/pushchannels")]
        [Consumes("application/vnd+pushserver.webpush+json")]
        public async Task<IActionResult> Register([FromBody]WebPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var config = await pushConfigurationManager.RegisterAsync(User.GetId(), registration);
            return Ok(config);
        }

        [Authorize("User")]
        [HttpPost("me/pushchannels")]
        [Consumes("application/vnd+pushserver.azurenotificationhub+json")]
        public async Task<IActionResult> Register([FromBody]AzureNotificationHubPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var config = await pushConfigurationManager.RegisterAsync(User.GetId(), registration);
            return Ok(config);
        }

        [Authorize("User")]
        [HttpGet("me/pushchannels")]
        public async Task<IActionResult> Get()
        {
            return Ok(await pushConfigurationManager.GetAllAsync(User.GetId()));
        }

        [Authorize("Service")]
        [HttpGet("{userId}/pushchannels")]
        public async Task<IActionResult> Get(string userId)
        {
            return Ok(await pushConfigurationManager.GetAllAsync(userId));
        }

        [Authorize("User")]
        [HttpDelete("me/pushchannels/{configurationId}")]
        public async Task<IActionResult> Delete(string configurationId)
        {
            var success = await pushConfigurationManager.DeleteAsync(User.GetId(), configurationId);
            if (success)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize("User")]
        [HttpPut("me/pushchannels/{configurationid}")]
        [Consumes("application/vnd+pushserver.azurenotificationhub+json")]
        public async Task<IActionResult> Update(string configurationid, [FromBody]AzureNotificationHubPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpPut("me/pushchannels/{configurationid}")]
        [Consumes("application/vnd+pushserver.webpush+json")]
        public async Task<IActionResult> Update(string configurationid, [FromBody]WebPushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }

        [Authorize("User")]
        [HttpPut("me/pushchannels/{configurationid}")]
        [Consumes("application/vnd+pushserver.firebase+json")]
        public async Task<IActionResult> Update(string configurationid, [FromBody]FirebasePushChannelRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await pushConfigurationManager.UpdateAsync(User.GetId(), configurationid, registration);
            return Ok();
        }

        [Authorize("Service")]
        [HttpPut("{userId}/pushchannels/{configurationid}/options")]
        public async Task<IActionResult> Update(string userId, string configurationid, [FromBody]PushChannelOptions options)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!(await pushConfigurationManager.GetAllAsync(userId)).Any(v => v.Id == configurationid))
            {
                return NotFound();
            }
            await pushConfigurationManager.UpdateOptionsAsync(userId, configurationid, options);
            return Ok();
        }
    }
}
