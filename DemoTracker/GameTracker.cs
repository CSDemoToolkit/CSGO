using DemoInfo;
using DemoTracker.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker;

namespace DemoTracker
{
	internal class GameTracker
	{
		private DemoParser _demoParser;
		private VariableTracker<RoundScore> _scoreTracker;
		private TickSummary _tickSummary;

		public GameTracker(DemoParser demoParser)		{
			_demoParser = demoParser;
			_scoreTracker = new VariableTracker<RoundScore>();
			_tickSummary = new TickSummary();
		}

		public TickSummary GetTick(int tick)
		{
			_tickSummary.RoundScore = _scoreTracker[tick];
			return _tickSummary;
		}

		public void Process_TickDone(int tick)
		{
			_scoreTracker.Add(tick, new RoundScore(_demoParser.TScore, _demoParser.CTScore));
		}
	}
}
