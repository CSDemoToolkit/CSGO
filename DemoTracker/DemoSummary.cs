using DemoInfo;
using DemoTracker.Structs;
using System.IO;
using System.Numerics;

namespace DemoTracker
{
	public class DemoSummary
	{
		private DemoParser _parser;
		private GameTracker _gameTracker;
		private PlayerTracker _playerTracker;

		private int _currentTick = 0;

		public DemoSummary(string path)
		{
			_parser = new(File.OpenRead(path));
			_gameTracker = new GameTracker(_parser);
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

		public TickSummary GetTickSummary(int tick)
		{
			TickSummary _tickSummary = _gameTracker.GetTick(tick);
			_tickSummary.Players = _playerTracker.GetTick(tick);
			return _tickSummary;
		}

		private void Parser_TickDone(object? sender, TickDoneEventArgs e)
		{
			_playerTracker.Process_TickDone(_currentTick);
			_gameTracker.Process_TickDone(_currentTick);
			_currentTick++;
		}
	}
}