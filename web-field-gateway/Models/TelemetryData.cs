using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web_field_gateway.Models
{
    //流水號,timestamp,類別,主機號,UID,DC/AC,ADSL/3G,Msg
    public class TelemetryData
    {
        static Random random = new System.Random();
        public string SeqNo { get; set; }
        public DateTime Timestamp { get; set; }
        public TelemetryTypes Type { get; set; }
        public string DeviceId { get; set; }
        public string UID { get; set; }
        public DCAC DCorAC { get; set; }
        public string ADSLor3G { get; set; }
        public string Message { get; set; }
        public static TelemetryData Random(string deviceID,string seqNo, string msg)
        {
            var ret = new TelemetryData()
            {
                SeqNo = seqNo,
                Timestamp = DateTime.UtcNow,
                Type = (TelemetryTypes)random.Next(0, 2),
                DeviceId = deviceID,
                UID = "UID-" + Guid.NewGuid().ToString(),
                DCorAC = (DCAC)random.Next(0, 1),
                ADSLor3G = random.Next(100) >= 50 ? "ADSL" : "3G",
                Message = msg
            };
            return ret;
        }
    }
    public enum TelemetryTypes
    {
        A= 0,B= 1,C = 2
    }
    public enum DCAC
    {
        DC = 0,AC = 1
    }
}
