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
		public RoundScore? RoundScore;
		public PlayerTickSummary[] Players;
	}
}
