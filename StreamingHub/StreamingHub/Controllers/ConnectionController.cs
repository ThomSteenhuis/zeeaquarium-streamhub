namespace StreamingHub.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using StreamingHub.Models;
    using StreamingHub.Notifications;

    [Route("api/[controller]")]
    [ApiController]
    public class ConnectionController : Controller
    {
        [HttpPost]
        [Authorize(Roles = Role.Streamer)]
        public void Connect()
        {
            ConnectionLostNotifications.RefreshNotificationsOnConnectionLost();
        }
    }
}
