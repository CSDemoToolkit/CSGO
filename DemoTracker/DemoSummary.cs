using DemoInfo;
using DemoTracker.Structs;
using System.IO;

namespace DemoTracker
{
	public class DemoSummary
	{
		private DemoParser _parser;
		private PlayerTracker _playerTracker;
		private int _currentTick = 0;

		public DemoSummary(string path)
		{
			_parser = new(File.OpenRead(path));
			_playerTracker = new PlayerTracker(_parser);
		}

		public void Process()
		{
			_parser.ParseHeader();

			_parser.TickDone += Parser_TickDone;
			Console.WriteLine("Starting");

			_parser.ParseToEnd();
			Console.WriteLine("Done!");
			Console.WriteLine(_currentTick);
		}

		public void GetZantoxPositions()
		{
			List<Vector?> zantoxPositions = new List<Vector?>(23935);
			for (int tick = 0; tick < _currentTick; tick++)
			{
				PlayerTickSummary[] players = _playerTracker.GetTick(tick);
				foreach (PlayerTickSummary player in players)
				{
					if (tick == 0)
					{
						Console.WriteLine("GetZantoxPositions: " + player.Name);
					}
					if (player.Name == "Zantox")
					{
						zantoxPositions.Add(player.Position);
						break;
					}
				}
			}
			foreach (Vector? pos in zantoxPositions)
			{
				Console.WriteLine(pos);
			}
		}

		private void Parser_TickDone(object? sender, TickDoneEventArgs e)
		{
			_playerTracker.Process_TickDone(_currentTick);
			_currentTick++;
		}
	}
}