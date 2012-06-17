using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace WolWeb.ViewModels.Home {
    public class IndexViewModel {

        public IndexViewModel(IIdentity identity, string pathToFile) {
            var hosts = new List<PreconfiguredHost>();

            foreach (var line in File.ReadAllLines(pathToFile).Select(x => x.Trim()).Where(x => !x.StartsWith("#"))) {
                var parts = line.Split('|');
                if (parts.Length == 3) {
                    hosts.Add(new PreconfiguredHost {
                        Name = parts[0].Trim(),
                        IpAddress = parts[1].Trim(),
                        MacAddress = parts[2].Trim()
                    });
                }
            }

            PreconfiguredHosts = hosts.OrderBy(x=>x.Name);

            CompilationTime = new FileInfo(typeof(WolWeb.WebApiApplication).Assembly.Location).LastWriteTimeUtc;
            UserName=identity.Name;
        }

        public IEnumerable<PreconfiguredHost> PreconfiguredHosts { get; set; }
        public DateTime CompilationTime { get; set; }
        public string UserName { get; set; }

        public class PreconfiguredHost {
            public string Name { get; set; }
            public string IpAddress { get; set; }
            public string MacAddress { get; set; }
        }

    }
}