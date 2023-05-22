namespace DemoReader
{
	public enum RoundEndReason
	{
		/// <summary>
		/// Target Successfully Bombed!
		/// </summary>
		TargetBombed = 1,

		/// <summary>
		/// The VIP has escaped.
		/// </summary>
		VIPEscaped,

		/// <summary>
		/// VIP has been assassinated
		/// </summary>
		VIPKilled,

		/// <summary>
		/// The terrorists have escaped
		/// </summary>
		TerroristsEscaped,

		/// <summary>
		/// The CTs have prevented most of the terrorists from escaping!
		/// </summary>
		CTStoppedEscape,

		/// <summary>
		/// Escaping terrorists have all been neutralized
		/// </summary>
		TerroristsStopped,

		/// <summary>
		/// The bomb has been defused!
		/// </summary>
		BombDefused,

		/// <summary>
		/// Counter-Terrorists Win!
		/// </summary>
		CTWin,

		/// <summary>
		/// Terrorists Win! 
		/// </summary>
		TerroristWin,

		/// <summary>
		/// Round Draw!
		/// </summary>
		Draw,

		/// <summary>
		/// All Hostages have been rescued
		/// </summary>
		HostagesRescued,

		/// <summary>
		/// Target has been saved! 
		/// </summary>
		TargetSaved,

		/// <summary>
		/// Hostages have not been rescued!
		/// </summary>
		HostagesNotRescued,

		/// <summary>
		/// Terrorists have not escaped!
		/// </summary>
		TerroristsNotEscaped,

		/// <summary>
		/// VIP has not escaped! 
		/// </summary>
		VIPNotEscaped,

		/// <summary>
		/// Game Commencing! 
		/// </summary>
		GameStart,

		/// <summary>
		/// Terrorists Surrender
		/// </summary>
		TerroristsSurrender,

		/// <summary>
		/// CTs Surrender
		/// </summary>
		CTSurrender,
	};

	public enum RoundMVPReason
	{
		MostEliminations = 1,
		BombPlanted,
		BombDefused,
	};

	public enum Bombsite
	{
		A,
		B
	}

	public delegate void ScoreChange(int oldScore, int newScore, Team team);
	public delegate void GamePhaseChange(GamePhase phase);
	public delegate void RoundWinStatusChange(RoundWinStatus status);
	public delegate void RoundStart(int timeLimit, int fragLimit, string objective);
	public delegate void WinPanelMatch();
	public delegate void RoundAnnounceFinal();
	public delegate void LastRoundHalf();
	public delegate void RoundOfficiallyEnded();
	public delegate void BeginNewMatch();
	public delegate void RoundAnnounceMatchStart();
	public delegate void RoundFreezeEnd();
	public delegate void RoundEnd(RoundEndReason reason, Team team, string msg);
	public delegate void RoundMvp(RoundMVPReason reason, ref Player player);
	public delegate void BotTakeover(ref Player player);
	public delegate void PlayerDeath(ref Player player, ref Player attacker, ref Player assister);
	public delegate void PlayerHurt(ref Player player, ref Player attacker);
	public delegate void PlayerDisconnect(ref Player player);
	public delegate void PlayerTeam(ref Player player, Team newTeam, Team oldTeam);
	public delegate void BeginPlant(ref Player player, Bombsite bombsite);
	public delegate void AbortPlant(ref Player player, Bombsite bombsite);
	public delegate void BombPlantet(ref Player player, Bombsite bombsite);
	public delegate void BombDefused(ref Player player, Bombsite bombsite);
	public delegate void BombExploded(ref Player player, Bombsite bombsite);

	public class DemoEventHandler
	{
		public event ScoreChange ScoreChange;
		public event GamePhaseChange GamePhaseChange;
		public event RoundWinStatusChange RoundWinStatusChange;
		public event RoundStart RoundStart;
		public event WinPanelMatch WinPanelMatch;
		public event RoundAnnounceFinal RoundAnnounceFinal;
		public event LastRoundHalf LastRoundHalf;
		public event RoundOfficiallyEnded RoundOfficiallyEnded;
		public event BeginNewMatch BeginNewMatch;
		public event RoundAnnounceMatchStart RoundAnnounceMatchStart;
		public event RoundFreezeEnd RoundFreezeEnd;
		public event RoundEnd RoundEnd;
		public event RoundMvp RoundMvp;
		public event BotTakeover BotTakeover;
		public event PlayerDeath PlayerDeath;
		public event PlayerHurt PlayerHurt;
		public event PlayerDisconnect PlayerDisconnect;
		public event PlayerTeam PlayerTeam;
		public event BeginPlant BeginPlant;
		public event AbortPlant AbortPlant;
		public event BombPlantet BombPlantet;
		public event BombDefused BombDefused;
		public event BombExploded BombExploded;

		DemoPacketContainer container;

		PlayerInfo[] playerInfo;
		Dictionary<int, int> userIdMap = new Dictionary<int, int>();
		bool userMapInitialized;

		public DemoEventHandler(DemoPacketContainer container, PlayerInfo[] playerInfo)
		{
			this.container = container;
			this.playerInfo = playerInfo;

			userIdMap.Add(0, 0);
		}

		internal void Update(ref GameEvent gameEvent, ref EventDescriptor descriptor)
		{
			if (descriptor.name != "player_connect" && !userMapInitialized)
			{
				return;
			}

			var stream = new SpanStream<byte>(gameEvent.keys.Span);

			if (descriptor.name == "round_start")
			{
				var timeLimit = stream.ReadAs<int>();
				var fragLimit = stream.ReadAs<int>();
				var objective = stream.ReadCustomString();
				RoundStart?.Invoke(timeLimit, fragLimit, objective);
			}
			else if (descriptor.name == "cs_win_panel_match")
			{
				WinPanelMatch?.Invoke();
			}
			else if (descriptor.name == "round_announce_final")
			{
				RoundAnnounceFinal?.Invoke();
			}
			else if (descriptor.name == "round_announce_last_round_half")
			{
				LastRoundHalf?.Invoke();
			}
			else if (descriptor.name == "round_officially_ended")
			{
				RoundOfficiallyEnded?.Invoke();
			}
			else if (descriptor.name == "begin_new_match")
			{
				BeginNewMatch?.Invoke();
			}
			else if (descriptor.name == "round_announce_match_start")
			{
				RoundAnnounceMatchStart?.Invoke();
			}
			else if (descriptor.name == "round_freeze_end")
			{
				RoundFreezeEnd?.Invoke();
			}
			else if (descriptor.name == "round_end")
			{
				var winner = stream.ReadAs<Team>();
				var reason = stream.ReadAs<RoundEndReason>();
				var message = stream.ReadCustomString();

				RoundEnd?.Invoke(reason, winner, message);
			}
			else if (descriptor.name == "round_mvp")
			{
				var userId = stream.ReadAs<int>();
				var reason = stream.ReadAs<RoundMVPReason>();

				RoundMvp?.Invoke(reason, ref container.players[userIdMap[userId]]);
			}
			else if (descriptor.name == "bot_takeover")
			{
				var userId = stream.ReadAs<int>();
				BotTakeover?.Invoke(ref container.players[userIdMap[userId]]);
			}
			else if (descriptor.name == "player_death")
			{
				var userId = stream.ReadAs<int>();
				var attacker = stream.ReadAs<int>();
				var assister = stream.ReadAs<int>();
				var assistedFlash = stream.ReadAs<bool>();
				var weapon = stream.ReadCustomString();
				var weaponItemId = stream.ReadCustomString();
				var weaponFauxItemId = stream.ReadCustomString();
				var weaponOriginalOwnerXUId = stream.ReadCustomString();
				var headshot = stream.ReadAs<bool>();
				var dominated = stream.ReadAs<int>();
				var revenger = stream.ReadAs<int>();
				var wipe = stream.ReadAs<int>();
				var penetrated = stream.ReadAs<int>();
				var noreplay = stream.ReadAs<bool>();
				var noscope = stream.ReadAs<bool>();
				var thrusmoke = stream.ReadAs<bool>();
				var attackerblind = stream.ReadAs<bool>();
				var distance = stream.ReadAs<float>();

				PlayerDeath?.Invoke(ref container.players[userIdMap[userId]], ref container.players[userIdMap[attacker]], ref container.players[userIdMap[assister]]);
			}
			else if (descriptor.name == "player_hurt")
			{
				var userId = stream.ReadAs<int>();
				var attacker = stream.ReadAs<int>();
				var health = stream.ReadAs<int>();
				var armor = stream.ReadAs<int>();
				var weapon = stream.ReadCustomString();
				var dmgHealth = stream.ReadAs<int>();
				var dmgArmor = stream.ReadAs<int>();
				var hitgroup = stream.ReadAs<int>();

				PlayerHurt?.Invoke(ref container.players[userIdMap[userId]], ref container.players[userIdMap[attacker]]);
			}
			else if (descriptor.name == "player_connect")
			{
				for (int i = 0; i < playerInfo.Length; i++)
				{
					if (playerInfo[i].userID == 0)
						continue;

					userIdMap.Add(playerInfo[i].userID, i);
					container.players[i].Name = playerInfo[i].name;
				}

				userMapInitialized = true;
			}
			else if (descriptor.name == "player_disconnect")
			{
				var userId = stream.ReadAs<int>();
				PlayerDisconnect?.Invoke(ref container.players[userIdMap[userId]]);
			}
			else if (descriptor.name == "player_team")
			{
				var userId = stream.ReadAs<int>();
				var teamEnt = stream.ReadAs<int>();
				var oldTeamEnt = stream.ReadAs<int>();
				var disconnect = stream.ReadAs<bool>();
				var autoTeam = stream.ReadAs<bool>();
				var silent = stream.ReadAs<bool>();
				var isBot = stream.ReadAs<bool>();

				Team newTeam = Team.Spectator;
				if (teamEnt == container.ctEnt)
					newTeam = Team.CounterTerrorists;
				else if (teamEnt == container.tEnt)
					newTeam = Team.Terrorists;

				Team oldTeam = Team.Spectator;
				if (oldTeamEnt == container.ctEnt)
					oldTeam = Team.CounterTerrorists;
				else if (oldTeamEnt == container.tEnt)
					oldTeam = Team.Terrorists;

				PlayerTeam?.Invoke(ref container.players[userIdMap[userId]], newTeam, oldTeam);
			}
			else if (descriptor.name == "bomb_beginplant" || descriptor.name == "bomb_abortplant" || descriptor.name == "bomb_planted" || descriptor.name == "bomb_defused" || descriptor.name == "bomb_exploded")
			{
				var userId = stream.ReadAs<int>();
				var site = stream.ReadAs<int>();

				Bombsite bombsite = Bombsite.A;
				if (site == container.bombsites[1].BombsiteId)
				{
					bombsite = Bombsite.B;
				}

				switch (descriptor.name)
				{
					case "bomb_beginplant":
						BeginPlant?.Invoke(ref container.players[userIdMap[userId]], bombsite);
						break;
					case "bomb_abortplant":
						AbortPlant?.Invoke(ref container.players[userIdMap[userId]], bombsite);
						break;
					case "bomb_planted":
						BombPlantet?.Invoke(ref container.players[userIdMap[userId]], bombsite);
						break;
					case "bomb_defused":
						BombDefused?.Invoke(ref container.players[userIdMap[userId]], bombsite);
						break;
					case "bomb_exploded":
						BombExploded?.Invoke(ref container.players[userIdMap[userId]], bombsite);
						break;
					default:
						break;
				}
			}

			/*
				for (int i = 0; i < descriptor.keys.Count; i++)
				{
					switch (descriptor.keys[i].type)
					{
						case 1:
							Console.WriteLine($"'{descriptor.keys[i].name}, String: {stream.ReadCustomString()}'");
							break;
						case 2:
							Console.WriteLine($"'{descriptor.keys[i].name}, Float: {stream.ReadAs<float>()}'");
							break;
						case 3:
						case 4:
						case 5:
							Console.WriteLine($"'{descriptor.keys[i].name}, Int: {stream.ReadAs<int>()}'");
							break;
						case 6:
							Console.WriteLine($"'{descriptor.keys[i].name}, Bool: {stream.ReadAs<byte>()}'");
							break;
						default:
							break;
					}
				}
				Console.WriteLine();
			*/
		}

		internal void InvokeScoreChanged(int newScore, int oldScore, Team team)
		{
			ScoreChange?.Invoke(newScore, oldScore, team);
		}

		internal void InvokeGamePhaseChange(GamePhase phase)
		{
			GamePhaseChange?.Invoke(phase);
		}

		internal void InvokeRoundWinStatusChange(RoundWinStatus status)
		{
			RoundWinStatusChange?.Invoke(status);
		}
	}
}
