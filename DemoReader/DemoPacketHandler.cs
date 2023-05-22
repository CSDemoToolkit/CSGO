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
		public string Name;

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

	public struct BombsiteInfo
	{
		public Guid BombsiteGuid;
		public int BombsiteId;

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

	public class DemoPacketContainer
	{
		public Player[] players = new Player[32];
		public int[,] PlayerEquipment = new int[32, 64];
		public BombsiteInfo[] bombsites = new BombsiteInfo[2];
		public Equipment[] equipment = new Equipment[DemoPacketHandler.MAX_ENTITIES];

		public Dictionary<int, Inferno> infernos = new Dictionary<int, Inferno>();

		internal int tEnt = 0; // Yuck
		internal int ctEnt = 0; // Yuck
	}

	public class DemoPacketHandler
	{
		public const int MAX_EDICT_BITS = 11;
		public const int INDEX_MASK = (1 << MAX_EDICT_BITS) - 1;
		public const int MAX_ENTITIES = 1 << MAX_EDICT_BITS;

		internal DemoPacketContainer container;

		ScorePacketHandler scorePacketHandler;
		PlayerPacketHandler playerPacketHandler;
		GamerRulePacketHandler gamePacketEventHandler;
		BombsitePacketHandler bombsitePacketHandler;
		InfernoPacketHandler infernoPacketHandler;
		WeaponPacketHandler weaponPacketHandler;

		public DemoPacketHandler(DemoPacketContainer container, DemoEventHandler eventHandler)
		{
			this.container = container;

			scorePacketHandler = new ScorePacketHandler(container, eventHandler);
			playerPacketHandler = new PlayerPacketHandler(this, scorePacketHandler);
			gamePacketEventHandler = new GamerRulePacketHandler(eventHandler);
			bombsitePacketHandler = new BombsitePacketHandler(this);
			infernoPacketHandler = new InfernoPacketHandler(this);
			weaponPacketHandler = new WeaponPacketHandler(this);
		}

		internal void Init(Span<ServerClass> serverClasses)
		{
			scorePacketHandler.Init(serverClasses);
			playerPacketHandler.Init(serverClasses);
			gamePacketEventHandler.Init(serverClasses);
			bombsitePacketHandler.Init(serverClasses);
			infernoPacketHandler.Init(serverClasses);
			weaponPacketHandler.Init(serverClasses);
		}

		internal void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, int v)
		{
			scorePacketHandler.Execute(ref entity, ref property, v);
			playerPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
			gamePacketEventHandler.Execute(ref serverClass, ref entity, ref property, v);
			infernoPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
			weaponPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
		}

		internal void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, float v)
		{
			playerPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
		}

		internal void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, Vector3 v)
		{
			playerPacketHandler.Execute(ref serverClass, ref entity, ref property, v);
			bombsitePacketHandler.Execute(ref serverClass, ref entity, ref property, v);
		}

		internal void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, string v)
		{
			scorePacketHandler.Execute(ref entity, ref property, v);
		}

		internal void Destroy(ref ServerClass serverClass, ref Entity entity)
		{
			infernoPacketHandler.Destroy(ref serverClass, ref entity);
		}
	}
}
