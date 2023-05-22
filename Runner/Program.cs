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

		static Demo demo = new();
		static DemoReader.DemoReader demoReader = new();

		static int t = 0;
		static int ct = 0;

        static void Main(string[] args)
        {
			demoReader.eventHandler.ScoreChange += EventHandler_ScoreChange;
			demoReader.eventHandler.PlayerDeath += EventHandler_PlayerDeath; ;

            int useDemoInfo = int.Parse(Console.ReadLine());
			Console.WriteLine($"Using demo {useDemoInfo}");

            Stopwatch sw = Stopwatch.StartNew();
			for (int i = 0; i < 1; i++)
			{
				if (useDemoInfo == 1)
				{
					demo.Analyze("Demos/demo1.dem");
				}
				else
				{
					demoReader.Analyze("Demos/demo1.dem");
				}
			}
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

		private static void EventHandler_PlayerDeath(ref Player player, ref Player attacker, ref Player assister)
		{
			//Console.WriteLine($"Player {player.Name} was killed by {attacker.Name}");
		}

		private static void EventHandler_ScoreChange(int oldScore, int newScore, DemoReader.Team team)
		{
			switch (team)
			{
				case DemoReader.Team.Terrorists:
					t = newScore;
					break;
				case DemoReader.Team.CounterTerrorists:
					ct = newScore;
					break;
				default:
					break;
			}

			Console.WriteLine($"CT: {ct}, T: {t}");
			Console.WriteLine($"	{demoReader.container.bombsites[1].Center}");
			Console.WriteLine($"	Player 0: {demoReader.container.players[0].Position}");
		}
	}
}