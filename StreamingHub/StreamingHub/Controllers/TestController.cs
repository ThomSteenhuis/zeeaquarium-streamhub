namespace StreamingHub.Controllers
{
    using StreamingHub.Models;
    using StreamingHub.Notifications;
    using Microsoft.Azure.NotificationHubs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Net;

    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {
        [HttpGet]
        [Authorize(Roles = Role.User)]
        public String GetSuccess()
        {
            return "Success";
        }
    }
}
