using System;
using System.Diagnostics;
using System.IO;
using System.Web.Http;

namespace WolWeb.Controllers {
    public class ShutdownController : ApiController {

        [HttpGet]
        public string Index(string id) {
            try {

                var process = new Process();
                process.StartInfo.FileName = "shutdown.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = " /s /t 0 /m " + id;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.Start();

                var message = "";
                using (var outReader = new StreamReader(process.StandardOutput.BaseStream)) {
                    using (var errorReader = new StreamReader(process.StandardError.BaseStream)) {
                        process.WaitForExit();

                        message = outReader.ReadToEnd() + errorReader.ReadToEnd();
                    }
                }

                if (message == "")
                    message = "Success";
                return message;
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }

    }
}
