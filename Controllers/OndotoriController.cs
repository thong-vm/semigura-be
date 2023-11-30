using Microsoft.AspNetCore.Mvc;

namespace semigura.Controllers
{
    public class OndotoriController : ControllerBase
    {

        // Create API return Devices infomation
        [Route("v1/devices/current")]
        [HttpPost]
        public Result1 PostDeviceInfo()
        {
            return CrtData();
        }
        // Create Device Table with Data
        public Result1 CrtData()
        {




            ChanelData chanelData11 = new ChanelData()
            {
                num = "1",
                name = "Ch.1",
                value = "92.6",
                unit = "C",
            };
            ChanelData chanelData12 = new ChanelData()
            {
                num = "2",
                name = "Ch.2",
                value = "42.1",
                unit = "C",
            };
            ChanelData chanelData21 = new ChanelData()
            {
                num = "1",
                name = "Ch.1",
                value = "21.1",
                unit = "C",
            };
            ChanelData chanelData22 = new ChanelData()
            {
                num = "2",
                name = "Ch.2",
                value = "20.8",
                unit = "C",
            };
            ChanelData chanelData31 = new ChanelData()
            {
                num = "1",
                name = "Ch.1",
                value = "90.2",
                unit = "C",
            };
            ChanelData chanelData32 = new ChanelData()
            {
                num = "2",
                name = "Ch.2",
                value = "29.0",
                unit = "C",
            };
            ChanelData chanelData41 = new ChanelData()
            {
                num = "1",
                name = "Ch.1",
                value = "41.1",
                unit = "C",
            };
            ChanelData chanelData42 = new ChanelData()
            {
                num = "2",
                name = "Ch.2",
                value = "41.2",
                unit = "C",
            };
            ChanelData chanelData51 = new ChanelData()
            {
                num = "1",
                name = "Ch.1",
                value = "35.1",
                unit = "C",
            };
            ChanelData chanelData52 = new ChanelData()
            {
                num = "2",
                name = "Ch.2",
                value = "40",
                unit = "%",
            };
            ChanelData chanelData6 = new ChanelData()
            {
                num = "1",
                name = "",
                value = "90.2",
                unit = "C",
            };
            ChanelData chanelData7 = new ChanelData()
            {
                num = "1",
                name = "",
                value = "9.9",
                unit = "C",
            };

            List<ChanelData> datachanel1 = new List<ChanelData>();
            datachanel1.Add(chanelData11);
            datachanel1.Add(chanelData12);
            List<ChanelData> datachanel2 = new List<ChanelData>();
            datachanel2.Add(chanelData21);
            datachanel2.Add(chanelData22);
            List<ChanelData> datachanel3 = new List<ChanelData>();
            datachanel3.Add(chanelData31);
            datachanel3.Add(chanelData32);
            List<ChanelData> datachanel4 = new List<ChanelData>();
            datachanel4.Add(chanelData41);
            datachanel4.Add(chanelData42);
            List<ChanelData> datachanel5 = new List<ChanelData>();
            datachanel5.Add(chanelData51);
            datachanel5.Add(chanelData52);
            List<ChanelData> datachanel6 = new List<ChanelData>();
            datachanel6.Add(chanelData6);
            List<ChanelData> datachanel7 = new List<ChanelData>();
            datachanel7.Add(chanelData7);
            Device newDevice1 = new Device()
            {
                num = "1",
                serial = "523435CC",
                model = "TR-71wb",
                name = "TR-71wb_523435CC",
                battery = "5",
                rssi = "",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "60",
                unixtime = "1684200324",
                channel = datachanel1
            };
            Device newDevice2 = new Device()
            {
                num = "1",
                serial = "52343A86",
                model = "TR-71wb",
                name = "TR-71wb",
                battery = "5",
                rssi = "",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "0",
                unixtime = "1682053683",
                channel = datachanel2
            };
            Device newDevice3 = new Device()
            {
                num = "1",
                serial = "52343A87",
                model = "TR-71wb",
                name = "TR-71wb_52343A87",
                battery = "5",
                rssi = "",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "0",
                unixtime = "1684116218",
                channel = datachanel3
            };
            Device newDevice4 = new Device()
            {
                num = "1",
                serial = "52343A88",
                model = "TR-71wb",
                name = "TR-71wb-2",
                battery = "5",
                rssi = "",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "0",
                unixtime = "1684200587",
                channel = datachanel4
            };
            Device newDevice5 = new Device()
            {
                num = "1",
                serial = "52369008",
                model = "TR-72wb",
                name = "TR-72wb",
                battery = "5",
                rssi = "",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "0",
                unixtime = "1679716172",
                channel = datachanel5
            };
            Device newDevice6 = new Device()
            {
                num = "1",
                serial = "52825BDC",
                model = "RTR502B",
                name = "Unit01",
                battery = "5",
                rssi = "5",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "0",
                unixtime = "1684471793",
                channel = datachanel6
            };
            Device newDevice7 = new Device()
            {
                num = "2",
                serial = "528261F0",
                model = "RTR502B",
                name = "Unit02",
                battery = "5",
                rssi = "",
                time_diff = "540",
                std_bias = "0",
                dst_bias = "0",
                unixtime = "1684471779",
                channel = datachanel7
            };
            List<Device> datadevice = new List<Device>();
            datadevice.Add(newDevice1);
            datadevice.Add(newDevice2);
            datadevice.Add(newDevice3);
            datadevice.Add(newDevice4);
            datadevice.Add(newDevice5);
            datadevice.Add(newDevice6);
            datadevice.Add(newDevice7);
            Result1 a = new Result1();
            a.devices = datadevice;
            return a;
        }

        // create Divice Class
        public class Device
        {
            public string? num { get; set; }
            public string? serial { get; set; }
            public string? model { get; set; }
            public string? name { get; set; }
            public string? battery { get; set; }
            public string? rssi { get; set; }
            public string? time_diff { get; set; }
            public string? std_bias { get; set; }
            public string? dst_bias { get; set; }
            public string? unixtime { get; set; }
            public List<ChanelData>? channel { get; set; }
        }
        public class ChanelData
        {

            public string? num { get; set; }
            public string? name { get; set; }
            public string? value { get; set; }
            public string? unit { get; set; }
        }
        public class Result1
        {
            public List<Device>? devices { get; set; }
        }
    }

}