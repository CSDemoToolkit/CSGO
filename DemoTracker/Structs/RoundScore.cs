using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTracker.Structs
{
	public struct RoundScore
	{
		public int TScore;
		public int CTScore;

		public RoundScore(int tScore, int ctScore)
		{
			this.TScore = tScore;
			this.CTScore = ctScore;
		}
	}
}
