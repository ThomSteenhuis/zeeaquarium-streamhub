using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace ZeeaquariumAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class MeasurementsController : ControllerBase
    {
        [HttpGet("measurements/{measurementName}")]
        public async Task<string> GetMeasurement(string measurementName, CancellationToken ct)
        {
            var measurement = "";

            var connection = ControllerUtils.GetConnection();

            try
            {
                connection.On<string, string>("broadcastMeasurement", (name, value) =>
                {
                    if (name == measurementName)
                    {
                        measurement = value;
                    }
                });

                await connection.StartAsync(ct);

                while (measurement == "" && !ct.IsCancellationRequested)
                {
                    Thread.Sleep(100);
                }
            }
            catch (OperationCanceledException e)
            {
            }

            await connection.StopAsync();

            return measurement;
        }

        [HttpPost("measurements/{measurementName}/value/{value}")]
        public async Task<bool> PostMeasurement(string measurementName, string value, CancellationToken ct)
        {
            var connection = ControllerUtils.GetConnection();

            try
            {
                await connection.StartAsync(ct);

                await connection.InvokeAsync("broadcastMeasurement", measurementName, value, ct);
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
