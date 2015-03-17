using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LCDevUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "LegendaryClient Updater";
            Console.WriteLine("LegendaryClient Development-Build Updater");
            Console.WriteLine("Initializing...");
            string commit;
            try
            {
                commit = File.ReadAllText("version");
            }
            catch
            {
                Console.WriteLine("Can't read local latest commit. Assuming it's first run");
                commit = "";
                File.WriteAllText("version", commit, Encoding.UTF8);
            }
            new Main(commit);
        }
    }
}
