
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;
using System.Web;
using System.IO;
using web_field_gateway.Models;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;

namespace web_field_gateway.Controllers
{
    public class FieldGatewayController : ApiController
    {
        static string connectionString = "HostName=sks-demo-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=OH/eB28iElMTVY8I2MLucAReOQd+kDXgr12XY3srMqs=";
        static string iotHubUri = "sks-demo-iothub.azure-devices.net";
        private void SaveDeviceIdentity(string deviceId, string deviceKey)
        {
            var fn = Path.Combine(HttpContext.Current.Server.MapPath("~/APP_DATA"),
                                deviceId);
            File.WriteAllText(fn, deviceKey);
        }
        private string GetDeviceKey(string deviceId)
        {
            var fn = Path.Combine(HttpContext.Current.Server.MapPath("~/APP_DATA"),
                                deviceId);
            if (File.Exists(fn))
            {
                return File.ReadAllText(fn);
            }
            else
            {
                return string.Empty;
            }
        }
        private void DeleteDeviceKey(string deviceId)
        {
            var fn = Path.Combine(HttpContext.Current.Server.MapPath("~/APP_DATA"),
                                deviceId);
            if (File.Exists(fn))
            {
                File.Delete(fn);
            }
        }
        public async Task<string> Register(string deviceId)
        {
            Device device = null;
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            try
            {
                device = await registryManager.AddDeviceAsync(
                        new Device(deviceId)
                    );
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            if(device != null)
            {
                var deviceKey  =device.Authentication.SymmetricKey.PrimaryKey;
                SaveDeviceIdentity(deviceId, deviceKey);
                return deviceKey;
            }
            return string.Empty;
        }
        public async Task Unregister(string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            var device = await registryManager.GetDeviceAsync(deviceId);
            await registryManager.RemoveDeviceAsync(device);
        }
        public async Task SendTelemetry(TelemetryData telemetry)
        {
            DeviceClient dc = DeviceClient.Create(iotHubUri,
                new DeviceAuthenticationWithRegistrySymmetricKey(
                        telemetry.DeviceId,
                        GetDeviceKey(telemetry.DeviceId)
                    ));
            var text = JsonConvert.SerializeObject(telemetry);
            var buffer = Encoding.UTF8.GetBytes(text);
            await dc.SendEventAsync(new Microsoft.Azure.Devices.Client.Message(buffer));
            
        }
    }
}
