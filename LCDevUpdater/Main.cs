using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace LCDevUpdater
{
    public class Main
    {
        internal string commit;

        public Main(string commit)
        {
            this.commit = commit;
            Console.WriteLine(String.IsNullOrEmpty(commit) ? "Force update is on due to versioning error or first run." : "Our exe is from commit " + commit);
            init();
        }
        public async void init()
        {
            string json = null;
            try
            {
                Console.WriteLine("Downloading version info");
                json = await DownloadStringAsync("https://ci.appveyor.com/api/projects/EddyV/legendaryclient");
            }
            catch
            {
                Console.WriteLine("Error downloading version info. Program will exit.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Parsing version info");
            JObject o = JObject.Parse(json);
            string commitId = (string)o["build"]["commitId"];
            if(commit == commitId)
            {
                Console.WriteLine("You have the latest build.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Checking build status");
            string status = (string)o["build"]["status"];
            if(status != "success")
            {
                Console.WriteLine("Latest build failed. Program will exit.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Getting job id");
                string jobid = (string)  o["build"]["jobs"][0]["jobId"];
                Console.WriteLine("Downloading update");
                bool updated = await DownloadFileAsync("https://ci.appveyor.com/api/buildjobs/" + jobid +"/artifacts/LegendaryClient.exe","LegendaryClientTmp.exe");
                if(updated)
                {
                    Console.WriteLine("Downloaded update. Overwriting old exe");
                    File.Delete("LegendaryClient.exe");
                    File.Move("LegendaryClientTmp.exe", "LegendaryClient.exe");
                    Console.WriteLine("Saving version");
                    File.WriteAllText("version", commitId);
                    Console.WriteLine("Done! Press any key to exit.");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    Console.WriteLine("Error downloading file. Program will exit;");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return;
                }
            }
        }

        private async Task<string> DownloadStringAsync(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(url);
            }
        }

        private async Task<bool> DownloadFileAsync(string url,string filename)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadFile(url,filename);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
