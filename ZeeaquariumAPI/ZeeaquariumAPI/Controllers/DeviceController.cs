using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace ZeeaquariumAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class DeviceController : Controller
    {
        [HttpGet("devices/{deviceName}")]
        public async Task<string> GetDeviceSwitchStatus(string deviceName, CancellationToken ct)
        {
            var deviceSwitchStatus = "";

            var connection = ControllerUtils.GetConnection();

            try
            {
                connection.On<string, string>("switchDevice", (name, status) =>
                {
                    if (name == deviceName)
                    {
                        deviceSwitchStatus = status;
                    }
                });

                await connection.StartAsync(ct);

                while (deviceSwitchStatus == "" && !ct.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
            }
            catch (OperationCanceledException e)
            {
            }

            await connection.StopAsync();
            

            return deviceSwitchStatus;
        }

        [HttpPost("devices/{deviceName}/on/{on}")]
        public async Task<bool> SwitchDevice(string deviceName, string on, CancellationToken ct)
        {
            var connection = ControllerUtils.GetConnection();

            try
            {
                await connection.StartAsync(ct);

                await connection.InvokeAsync("switchDevice", deviceName, on, ct);
            }
            catch (OperationCanceledException e)
            {
                await connection.StopAsync();

                return false;
            }

            await connection.StopAsync();

            return true;
        }
    }
}
