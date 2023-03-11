using CSGO;
using CSGO.API.Faceit;
using CSGO.API.Gamer;
using DemoReader;
using DemoTracker;
using DemoTracker.Structs;
using System.Diagnostics;
using Tracker;

namespace Runner
{
    internal class Program
    {
        static HttpClient faceitClient = Faceit.GetClient();
        static HttpClient gamerClient = Gamer.GetClient();

        static void Main(string[] args)
        {
			string demoPath = "Demos/demo1.dem";

			Stopwatch sw = Stopwatch.StartNew();

            int useDemoInfo = 2;
            if (useDemoInfo == 1)
            {
				Demo demo = new();
                demo.Analyze(demoPath);
            }
			else if (useDemoInfo == 2)
			{
				DemoSummary demo = new DemoSummary(demoPath);
				demo.Process();
				TickSummary tick = demo.GetTickSummary(10000);
			}
            else
            {
				DemoReader.DemoReader demoReader = new();
                demoReader.Analyze(demoPath);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}