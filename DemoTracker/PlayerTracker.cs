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
	public class PlayerTracker
	{
		private DemoParser _demoParser;

		private class Player
		{
			public string Name { get; private set; }
			public long SteamID { get; private set; }
			public string SteamID32 { get; private set; }
			private ContinuousVariableTracker<Vector> Position;
			private ContinuousVariableTracker<float?> ViewDirectionX;
			private ContinuousVariableTracker<float?> ViewDirectionY;
			private ContinuousVariableTracker<float?> FlashDuration;
			private VariableTracker<int?> HP;
			private VariableTracker<int?> Armor;
			private VariableTracker<int?> Money;
			private VariableTracker<bool?> IsDucking;

			public Player(string name, long steamId, string steamId32)
			{
				Name = name;
				SteamID = steamId;
				SteamID32 = steamId32;
				Position = new ContinuousVariableTracker<Vector>();
				ViewDirectionX = new ContinuousVariableTracker<float?>();
				ViewDirectionY = new ContinuousVariableTracker<float?>();
				FlashDuration = new ContinuousVariableTracker<float?>();
				HP = new VariableTracker<int?>();
				Armor = new VariableTracker<int?>();
				Money = new VariableTracker<int?>();
				IsDucking = new VariableTracker<bool?>();
			}

			public void AddTick(int tick, DemoInfo.Player playerInfo)
			{
				Position.Add(tick, playerInfo.Position.Copy());
				ViewDirectionX.Add(tick, playerInfo.ViewDirectionX);
				ViewDirectionY.Add(tick, playerInfo.ViewDirectionY);
				FlashDuration.Add(tick, playerInfo.FlashDuration);
				HP.Add(tick, playerInfo.HP);
				Armor.Add(tick, playerInfo.Armor);
				Money.Add(tick, playerInfo.Money);
				IsDucking.Add(tick, playerInfo.IsDucking);
			}

			public void AddEmptyTick(int tick)
			{
				Position.Add(tick, null);
				ViewDirectionX.Add(tick, null);
				ViewDirectionY.Add(tick, null);
				FlashDuration.Add(tick, null);
				HP.Add(tick, null);
				Armor.Add(tick, null);
				Money.Add(tick, null);
				IsDucking.Add(tick, null);
			}

			public PlayerTickSummary GetTick(int tick)
			{
				if (tick == 0)
				{
					Console.WriteLine("GetTick: " + Name);
				}

				PlayerTickSummary summary = new PlayerTickSummary();
				summary.Name = Name;
				summary.SteamID = SteamID;
				summary.SteamID32 = SteamID32;
				summary.Position = Position[tick];
				summary.ViewDirectionX = ViewDirectionX[tick];
				summary.ViewDirectionY = ViewDirectionY[tick];
				summary.FlashDuration = FlashDuration[tick];
				summary.HP = HP[tick];
				summary.Armor = Armor[tick];
				summary.Money = Money[tick];
				summary.IsDucking = IsDucking[tick];
				return summary;
			}
		}

		private Dictionary<long, Player> _playersBySteamId;

		public PlayerTracker(DemoParser demoParser)
		{
			_demoParser = demoParser;
			_playersBySteamId = new Dictionary<long, Player>();
		}

		public PlayerTickSummary[] GetTick(int tick)
		{
			PlayerTickSummary[] array = new PlayerTickSummary[_playersBySteamId.Count];
			foreach (Player player in _playersBySteamId.Values)
			{
				array.Append(player.GetTick(tick));
			}
			return array;
		}

		public void Process_TickDone(int tick)
		{
			List<long> playersAccountedFor = new List<long>();
			foreach (DemoInfo.Player player in _demoParser.PlayingParticipants)
			{
				Player? playerTracker = _playersBySteamId.GetValueOrDefault(player.SteamID, null);
				if (playerTracker == null)
				{
					_playersBySteamId.Add(player.SteamID, new Player(player.Name, player.SteamID, player.SteamID32));
					playerTracker = _playersBySteamId[player.SteamID];
				}

				playerTracker.AddTick(tick, player);
				playersAccountedFor.Add(player.SteamID);
			}

			foreach (long player in _playersBySteamId.Keys)
			{
				if (!playersAccountedFor.Contains(player))
				{
					Player playerTracker = _playersBySteamId[player];
					playerTracker.AddEmptyTick(tick);
				}
			}
		}
	}
}
