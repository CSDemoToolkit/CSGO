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
	public unsafe struct Player
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

		public fixed int Weapons[5];
		public fixed int WeaponAmmo[32];
		public int ActiveWeapon;
	}

	public struct Bombsite
	{
		public Guid BombsiteEnt;

		public Vector2 Center;
		public Vector4 BoundingBox;
	}

	public struct Inferno
	{
		public int Owner;
	}

	public struct Equipment
	{

	}

	public delegate void ScoreChange(int oldScore, int newScore, Team team);
	public delegate void GamePhaseChange(GamePhase phase);
	public delegate void RoundWinStatusChange(RoundWinStatus status);

	public class DemoPacketHandler
	{
		public const int MAX_EDICT_BITS = 11;
		public const int INDEX_MASK = (1 << MAX_EDICT_BITS) - 1;
		public const int MAX_ENTITIES = 1 << MAX_EDICT_BITS;

		public event ScoreChange ScoreChange;
		public event GamePhaseChange GamePhaseChange;
		public event RoundWinStatusChange RoundWinStatusChange;

		ScorePacketHandler scorePacketHandler;
		PlayerPacketHandler playerPacketHandler;
		GamerRulePacketHandler gamePacketEventHandler;
		BombsitePacketHandler bombsitePacketHandler;
		InfernoPacketHandler infernoPacketHandler;
		WeaponPacketHandler weaponPacketHandler;

		public Player[] players = new Player[32];
		public int[,] PlayerEquipment = new int[32, 64];
		public Bombsite[] bombsites = new Bombsite[2];
		public Equipment[] equipment = new Equipment[MAX_ENTITIES];

		public Dictionary<int, Inferno> infernos = new Dictionary<int, Inferno>();

		public DemoPacketHandler()
		{
			scorePacketHandler = new ScorePacketHandler(this);
			playerPacketHandler = new PlayerPacketHandler(this, scorePacketHandler);
			gamePacketEventHandler = new GamerRulePacketHandler(this);
			bombsitePacketHandler = new BombsitePacketHandler(this);
			infernoPacketHandler = new InfernoPacketHandler(this);
			weaponPacketHandler = new WeaponPacketHandler(this);
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			scorePacketHandler.Init(serverClasses);
			playerPacketHandler.Init(serverClasses);
			gamePacketEventHandler.Init(serverClasses);
			bombsitePacketHandler.Init(serverClasses);
			infernoPacketHandler.Init(serverClasses);
			weaponPacketHandler.Init(serverClasses);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, int v)
		{
			scorePacketHandler.Execute(ref entity, ref property, v);
			playerPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
			gamePacketEventHandler.Execute(ref serverClass, ref entity, ref property, v);
			infernoPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
			weaponPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, float v)
		{
			playerPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, Vector3 v)
		{
			playerPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
			bombsitePacketHandler.Execute(ref serverClass, ref entity, ref property, v);
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, string v)
		{
			scorePacketHandler.Execute(ref entity, ref property, v);
		}

		public void Destroy(ref ServerClass serverClass, ref Entity entity)
		{
			infernoPacketHandler.Destroy(ref serverClass, ref entity);
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
