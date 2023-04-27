using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace DemoReader
{
	public struct Player
	{
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 ViewDirection;

		public int Health;
		public int Armor;
		public int Money;
		public int CurrentEquipmentValue;
		public int RoundStartEquipmentValue;
		public int FreezetimeEndEquipmentValue;

		public float FlashDuration;

		public Team Team;

		public bool HasDefuser;
		public bool HasHelmet;
		public bool IsDucking;
		public bool IsInBuyZone;
	}

	public struct Bombsite
	{
		public int BombsiteEnt;

		public Vector2 Center;
		public Vector4 BoundingBox;
	}

	public struct Inferno
	{
		public int Owner;
	}

	public delegate void ScoreChange(int oldScore, int newScore, Team team);
	public delegate void GamePhaseChange(GamePhase phase);
	public delegate void RoundWinStatusChange(RoundWinStatus status);

	public class DemoEventHandler
	{
		public event ScoreChange ScoreChange;
		public event GamePhaseChange GamePhaseChange;
		public event RoundWinStatusChange RoundWinStatusChange;

		ScoreEventHandler scoreEventHandler;
		PlayerEventHandler playerEventHandler;
		GameRulesEventHandler gameRulesEventHandler;
		BombsiteEventHandler bombsiteEventHandler;
		InfernoEventHandler infernoEventHandler;

		public Player[] players = new Player[32];
		public int[,] PlayerEquipment = new int[32, 64];
		public Bombsite[] bombsites = new Bombsite[2];

		public Dictionary<int, Inferno> infernos = new Dictionary<int, Inferno>();

		public DemoEventHandler()
		{
			scoreEventHandler = new ScoreEventHandler(this);
			playerEventHandler = new PlayerEventHandler(this, scoreEventHandler);
			gameRulesEventHandler = new GameRulesEventHandler(this);
			bombsiteEventHandler = new BombsiteEventHandler(this);
			infernoEventHandler = new InfernoEventHandler(this);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, int v)
		{
			scoreEventHandler.Execute(ref entity, property, v);
			playerEventHandler.Execute(ref serverClass, ref entity, property, v);
			gameRulesEventHandler.Execute(ref serverClass, ref entity, property, v);
			infernoEventHandler.Execute(ref serverClass, ref entity, property, v);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, float v)
		{
			playerEventHandler.Execute(ref serverClass, ref entity, property, v);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, Vector3 v)
		{
			playerEventHandler.Execute(ref serverClass, ref entity, property, v);
			bombsiteEventHandler.Execute(ref serverClass, ref entity, property, v);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, string v)
		{
			scoreEventHandler.Execute(ref entity, property, v);
		}

		public void Destroy(ref ServerClass serverClass, ref Entity entity)
		{
			infernoEventHandler.Destroy(ref serverClass, ref entity);
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
