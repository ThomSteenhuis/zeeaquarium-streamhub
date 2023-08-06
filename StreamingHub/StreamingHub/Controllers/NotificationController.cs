namespace StreamingHub.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.NotificationHubs;
    using StreamingHub.Models;
    using StreamingHub.Notifications;
    using System.Threading.Tasks;

    [Route("api/notifications")]
    [Authorize(Roles = Role.Streamer)]
    public class NotificationController : Controller
    {
        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> Send(string title, string body)
        {
            var message = "{ \"notification\" : { \"title\": \"" + title + "\", \"body\":\"" + body + "\"}}";
            var outcome = await Notifications.Instance.Hub.SendFcmNativeNotificationAsync(message);

            if (outcome != null)
            {
                if (!((outcome.State == NotificationOutcomeState.Abandoned) ||
                    (outcome.State == NotificationOutcomeState.Unknown)))
                {
                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}
