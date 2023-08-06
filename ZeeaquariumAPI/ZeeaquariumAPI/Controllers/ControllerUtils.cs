using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR.Client;

namespace ZeeaquariumAPI.Controllers
{
    public class ControllerUtils
    {
        public static HubConnection GetConnection()
        {
            return new HubConnectionBuilder()
                .WithUrl("https://zeeaquarium-streamingapp.azurewebsites.net/data")
                .Build();
        }
    }
}
