using System;
using System.Net.NetworkInformation;
using System.Web.Http;

namespace WolWeb.Controllers {
    [AuthorizeRemoteOnly]
    public class PingController : ApiController {

        [HttpGet]
        public PingResult Index(string id) {
            var pinger = new Ping();
            try {
                var result = pinger.Send(id);

                if (result.Status == IPStatus.Success)
                    return new PingResult("Ok (" + result.RoundtripTime + "ms)", true);
                return new PingResult("Failed (" + result.Status.ToString() + ")", false);
            }
            catch (Exception ex) {
                if (ex.InnerException != null)
                    return new PingResult("Failed (" + ex.InnerException.Message + ")", false);
                return new PingResult("Failed (" + ex.Message + ")", false);
            }
        }

        public class PingResult {
            public PingResult(string message, bool status) {
                Message = message;
                Status = status;
            }
            public string Message { get; set; }
            public bool Status { get; set; }
        }

    }
}
