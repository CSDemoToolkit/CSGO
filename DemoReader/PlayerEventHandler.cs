using System.Numerics;
using System.Runtime.CompilerServices;

namespace DemoReader
{
	public class PlayerEventHandler
	{
		const int MAX_EDICT_BITS = 11;
		const int INDEX_MASK = (1 << MAX_EDICT_BITS) - 1;

		DemoEventHandler eventHandler;
		ScoreEventHandler scoreEventHandler;

		public PlayerEventHandler(DemoEventHandler eventHandler, ScoreEventHandler scoreEventHandler)
		{
			this.eventHandler=eventHandler;
			this.scoreEventHandler=scoreEventHandler;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, int v)
		{
			if (serverClass.name == "CCSPlayer")
			{
				// Avoid bounds checking
				ref Player player = ref Unsafe.Add(ref eventHandler.players[0], entity.id - 1);

				if (property.varName == "m_iHealth")
				{
					player.Health = v;
				}
				else if (property.varName == "m_ArmorValue")
				{
					player.Armor = v;
				}
				else if (property.varName == "m_iAccount")
				{
					player.Money = v;
				}
				else if (property.varName == "m_unCurrentEquipmentValue")
				{
					player.CurrentEquipmentValue = v;
				}
				else if (property.varName == "m_unRoundStartEquipmentValue")
				{
					player.RoundStartEquipmentValue = v;
				}
				else if (property.varName == "m_unFreezetimeEndEquipmentValue")
				{
					player.FreezetimeEndEquipmentValue = v;
				}
				else if (property.varName == "m_bHasDefuser")
				{
					player.HasDefuser = v == 1;
				}
				else if (property.varName == "m_bHasHelmet")
				{
					player.HasDefuser = v == 1;
				}
				else if (property.varName == "m_bDucking")
				{
					player.IsDucking = v == 1;
				}
				else if (property.varName == "m_bInBuyZone")
				{
					player.IsDucking = v == 1;
				}
				else if (property.varName == "m_iTeamNum")
				{
					if (v == scoreEventHandler.GetTID())
					{
						player.Team = Team.Terrorists;
					}
					else if (v == scoreEventHandler.GetCTID())
					{
						player.Team = Team.CounterTerrorists;
					}
					else
					{
						player.Team = Team.Spectator;
					}
				}
			}
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, float v)
		{
			if (serverClass.name == "CCSPlayer")
			{
				// Avoid bounds checking
				ref Player player = ref Unsafe.Add(ref eventHandler.players[0], entity.id - 1);

				if (property.varName == "m_vecOrigin[2]")
				{
					player.Position.Z = v;
				}
				else if (property.varName == "m_angEyeAngles[1]")
				{
					player.ViewDirection.X = v;
				}
				else if (property.varName == "m_angEyeAngles[0]")
				{
					player.ViewDirection.Y = v;
				}
				else if (property.varName == "m_flFlashDuration")
				{
					player.FlashDuration = v;
				}
				else if (property.varName == "m_vecVelocity[0]")
				{
					player.Velocity.X = v;
				}
				else if (property.varName == "m_vecVelocity[1]")
				{
					player.Velocity.Y = v;
				}
				else if (property.varName == "m_vecVelocity[2]")
				{
					player.Velocity.Z = v;
				}
			}
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, Vector3 v)
		{
			if (serverClass.name == "CCSPlayer")
			{
				// Avoid bounds checking
				ref Player player = ref Unsafe.Add(ref eventHandler.players[0], entity.id - 1);

				if (property.varName == "m_vecOrigin")
				{
					player.Position.X = v.X;
					player.Position.Y = v.Y;
				}
			}
		}
	}
}
