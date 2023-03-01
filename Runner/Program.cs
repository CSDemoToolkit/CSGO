using CSGO;
using CSGO.API.Faceit;
using CSGO.API.Gamer;
using DemoReader;
using System.Diagnostics;

namespace Runner
{
    internal class Program
    {
        static HttpClient faceitClient = Faceit.GetClient();
        static HttpClient gamerClient = Gamer.GetClient();

        static void Main(string[] args)
        {
            Demo demo = new();
            DemoReader.DemoReader demoReader = new();
            Stopwatch sw = Stopwatch.StartNew();

            int useDemoInfo = 1;

            if (useDemoInfo == 1)
            {
                demo.Analyze("Demos/demo1.dem");
            }
            else
            {
                demoReader.Analyze("Demos/demo1.dem");
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}