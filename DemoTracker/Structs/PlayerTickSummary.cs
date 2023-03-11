using DemoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
		public Vector3? Position;
		public Vector2? ViewDirection;
		public float? FlashDuration;
		public int? HP;
		public int? Armor;
		public int? Money;
		public bool? IsDucking;
		public Team? Team;
	}
}
