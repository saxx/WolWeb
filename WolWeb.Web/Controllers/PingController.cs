using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web.Http;

namespace WolWeb.Controllers {
    public class PingController : ApiController {

        [HttpGet]
        public string Index(string id) {
            var pinger = new Ping();
            try {
                var result = pinger.Send(id);

                if (result.Status == IPStatus.Success)
                    return "Up and running (" + result.RoundtripTime + "ms)";

                return result.Status.ToString();
            }
            catch (Exception ex) {
                if (ex.InnerException != null)
                    return ex.InnerException.Message;
                return ex.Message;
            }
        }

    }
}
