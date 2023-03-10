using DemoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTracker.Structs
{
	public struct TickSummary
	{
		public RoundScore roundScore;

		public TickSummary()
		{
			roundScore = new RoundScore();
			roundScore.TScore = 0;
			roundScore.CTScore = 0;
		}
	}
}
