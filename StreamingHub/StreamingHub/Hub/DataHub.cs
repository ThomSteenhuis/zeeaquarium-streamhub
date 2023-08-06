using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace StreamingHub.Hub
{
    using StreamingHub.Models;
    using StreamingHub.Notifications;

    [Authorize]
    public class DataHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private const string WATCHERS_GROUP = "watchers";

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Others.SendAsync("joinedGroup", Context.ConnectionId, groupName);
        }
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Others.SendAsync("leftGroup", Context.ConnectionId, groupName);
        }

        [Authorize(Roles = Role.Streamer)]
        public Task BroadcastMeasurement(string name, string measurement)
            => Clients.Group(WATCHERS_GROUP).SendAsync("broadcastMeasurement", name, measurement);

        [Authorize(Roles = Role.User)]
        public Task SwitchDevice(string name, string on)
            => Clients.Others.SendAsync("switchDevice", name, on);
        
        public Task RequestDeviceStatus(string name)
            => Clients.Others.SendAsync("requestDeviceStatus", name);

        [Authorize(Roles = Role.Streamer)]
        public Task DeviceStatus(string name, string on)
            => Clients.Group(WATCHERS_GROUP).SendAsync("deviceStatus", name, on);

        [Authorize(Roles = Role.User)]
        public Task SetSetting(string setting, string value)
            => Clients.Others.SendAsync("setSetting", setting, value);

        public Task RequestSetting(string setting)
            => Clients.Others.SendAsync("requestSetting", setting);

        [Authorize(Roles = Role.Streamer)]
        public Task Setting(string setting, string value)
            => Clients.Group(WATCHERS_GROUP).SendAsync("setting", setting, value);

        [Authorize(Roles = Role.User)]
        public Task Command(string command)
            => Clients.Others.SendAsync("command", command);

        public Task Ping(string message)
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ping", message);
        }
    }
}