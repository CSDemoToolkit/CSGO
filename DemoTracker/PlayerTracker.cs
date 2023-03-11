using DemoInfo;
using DemoTracker.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tracker;

namespace DemoTracker
{
	internal class PlayerTracker
	{
		private DemoParser _demoParser;

		private class Player
		{
			public string Name { get; private set; }
			public long SteamID { get; private set; }
			public string SteamID32 { get; private set; }
			private ContinuousVariableTracker<Vector3> Position;
			private ContinuousVariableTracker<Vector2> ViewDirection;
			private ContinuousVariableTracker<float> FlashDuration;
			private VariableTracker<int> HP;
			private VariableTracker<int> Armor;
			private VariableTracker<int> Money;
			private VariableTracker<bool> IsDucking;
			private VariableTracker<Team> Team;

			private PlayerTickSummary _summary;

			public Player(string name, long steamId, string steamId32)
			{
				Name = name;
				SteamID = steamId;
				SteamID32 = steamId32;
				Position = new ContinuousVariableTracker<Vector3>();
				ViewDirection = new ContinuousVariableTracker<Vector2>();
				FlashDuration = new ContinuousVariableTracker<float>();
				HP = new VariableTracker<int>();
				Armor = new VariableTracker<int>();
				Money = new VariableTracker<int>();
				IsDucking = new VariableTracker<bool>();
				Team = new VariableTracker<Team>();

				_summary = new PlayerTickSummary();
				_summary.Name = Name;
				_summary.SteamID = SteamID;
				_summary.SteamID32 = SteamID32;
			}

			public void AddTick(int tick, DemoInfo.Player playerInfo)
			{
				if (playerInfo.IsAlive)
				{
					Position.Add(tick, playerInfo.Position.ToNumericVector());
					ViewDirection.Add(tick, new Vector2(playerInfo.ViewDirectionX, playerInfo.ViewDirectionY));
					FlashDuration.Add(tick, playerInfo.FlashDuration);
				}
				else
				{
					Position.Add(tick, null);
					ViewDirection.Add(tick, null);
					FlashDuration.Add(tick, null);
				}
				HP.Add(tick, playerInfo.HP);
				Armor.Add(tick, playerInfo.Armor);
				Money.Add(tick, playerInfo.Money);
				IsDucking.Add(tick, playerInfo.IsDucking);
				Team.Add(tick, playerInfo.Team);
			}

			public void AddEmptyTick(int tick)
			{
				Position.Add(tick, null);
				ViewDirection.Add(tick, null);
				FlashDuration.Add(tick, null);
				HP.Add(tick, null);
				Armor.Add(tick, null);
				Money.Add(tick, null);
				IsDucking.Add(tick, null);
				Team.Add(tick, null);
			}

			public PlayerTickSummary GetTick(int tick)
			{
				_summary.Position = Position[tick];
				_summary.ViewDirection = ViewDirection[tick];
				_summary.FlashDuration = FlashDuration[tick];
				_summary.HP = HP[tick];
				_summary.Armor = Armor[tick];
				_summary.Money = Money[tick];
				_summary.IsDucking = IsDucking[tick];
				_summary.Team = Team[tick];
				return _summary;
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
			for (int playerIndex = 0; playerIndex < _playersBySteamId.Count; playerIndex++)
			{
				array[playerIndex] = _playersBySteamId.Values.ElementAt(playerIndex).GetTick(tick);
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
