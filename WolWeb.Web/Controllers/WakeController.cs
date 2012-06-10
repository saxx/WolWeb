using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Web.Http;

namespace WolWeb.Controllers {
    public class WakeController : ApiController {

        [HttpGet]
        public bool Index(string id) {
            try {
                // Convert MAC address to Hex bytes
                var value = long.Parse(id.Replace("-", "").Replace(":", ""), NumberStyles.HexNumber, CultureInfo.CurrentCulture.NumberFormat);
                var macBytes = BitConverter.GetBytes(value);
                Array.Reverse(macBytes);

                var macAddress = new byte[6];
                for (int i = 0; i <= 5; i++)
                    macAddress[i] = macBytes[i + 2];

                var packet = new List<byte>();

                //Trailer of 6 FF packets 
                for (int i = 0; i < 6; i++)
                    packet.Add(0xFF);

                //Repeat 16 time the MAC address (which is 6 bytes) 
                for (int i = 0; i < 16; i++)
                    packet.AddRange(macAddress);

                var client = new UdpClient();
                client.Connect("192.168.1.255", 7); //the port doesn't matter
                client.Send(packet.ToArray(), packet.Count);

                return true;
            }
            catch {
                return false;
            }
        }

    }
}
