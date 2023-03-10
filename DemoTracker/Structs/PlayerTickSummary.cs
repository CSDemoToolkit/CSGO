using DemoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker;

namespace DemoTracker.Structs
{
	public struct PlayerTickSummary
	{
		public string Name;
		public long SteamID;
		public string SteamID32;
		public Vector? Position;
		public float? ViewDirectionX;
		public float? ViewDirectionY;
		public float? FlashDuration;
		public int? HP;
		public int? Armor;
		public int? Money;
		public bool? IsDucking;
	}
}
