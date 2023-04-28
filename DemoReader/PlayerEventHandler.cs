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

		Guid IS_PLAYER_ID;

		Guid PLAYER_HEALTH_ID;
		Guid PLAYER_ARMOR_ID;
		Guid PLAYER_ACCOUNT_ID;
		Guid PLAYER_CURRENT_EQUIPMENT_VALUE_ID;
		Guid PLAYER_ROUND_START_EQUIPMENT_VALUE_ID;
		Guid PLAYER_FREEZE_END_EQUIPMENT_VALUE_ID;
		Guid PLAYER_HAS_DEFUSER_ID;
		Guid PLAYER_HAS_HELMET_ID;
		Guid PLAYER_DUCKING_ID;
		Guid PLAYER_IN_BUY_ZONE_ID;
		Guid PLAYER_TEAM_NUM_ID;

		Guid PLAYER_VEC_ORIGIN_ID;
		Guid PLAYER_VEC_ORIGIN_Z_ID;
		Guid PLAYER_EYE_ANGLE_X_ID;
		Guid PLAYER_EYE_ANGLE_Y_ID;
		Guid PLAYER_FLASH_DURATION_ID;
		Guid PLAYER_VELOCITY_X_ID;
		Guid PLAYER_VELOCITY_Y_ID;
		Guid PLAYER_VELOCITY_Z_ID;

		public PlayerEventHandler(DemoEventHandler eventHandler, ScoreEventHandler scoreEventHandler)
		{
			this.eventHandler = eventHandler;
			this.scoreEventHandler = scoreEventHandler;
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			IS_PLAYER_ID = serverClasses.FindServerClass("CCSPlayer").id;

			PLAYER_HEALTH_ID = serverClasses.FindProperty("CCSPlayer", "m_iHealth").id;
			PLAYER_ARMOR_ID = serverClasses.FindProperty("CCSPlayer", "m_ArmorValue").id;
			PLAYER_ACCOUNT_ID = serverClasses.FindProperty("CCSPlayer", "m_iAccount").id;
			PLAYER_CURRENT_EQUIPMENT_VALUE_ID = serverClasses.FindProperty("CCSPlayer", "m_unCurrentEquipmentValue").id;
			PLAYER_ROUND_START_EQUIPMENT_VALUE_ID = serverClasses.FindProperty("CCSPlayer", "m_unRoundStartEquipmentValue").id;
			PLAYER_FREEZE_END_EQUIPMENT_VALUE_ID = serverClasses.FindProperty("CCSPlayer", "m_unFreezetimeEndEquipmentValue").id;
			PLAYER_HAS_DEFUSER_ID = serverClasses.FindProperty("CCSPlayer", "m_bHasDefuser").id;
			PLAYER_HAS_HELMET_ID = serverClasses.FindProperty("CCSPlayer", "m_bHasHelmet").id;
			PLAYER_DUCKING_ID = serverClasses.FindProperty("CCSPlayer", "m_bDucking").id;
			PLAYER_IN_BUY_ZONE_ID = serverClasses.FindProperty("CCSPlayer", "m_bInBuyZone").id;
			PLAYER_TEAM_NUM_ID = serverClasses.FindProperty("CCSPlayer", "m_iTeamNum").id;

			PLAYER_VEC_ORIGIN_ID = serverClasses.FindProperty("CCSPlayer", "m_vecOrigin").id;
			PLAYER_VEC_ORIGIN_Z_ID = serverClasses.FindProperty("CCSPlayer", "m_vecOrigin[2]").id;
			PLAYER_EYE_ANGLE_X_ID = serverClasses.FindProperty("CCSPlayer", "m_angEyeAngles[1]").id;
			PLAYER_EYE_ANGLE_Y_ID = serverClasses.FindProperty("CCSPlayer", "m_angEyeAngles[0]").id;
			PLAYER_FLASH_DURATION_ID = serverClasses.FindProperty("CCSPlayer", "m_flFlashDuration").id;
			PLAYER_VELOCITY_X_ID = serverClasses.FindProperty("CCSPlayer", "m_vecVelocity[0]").id;
			PLAYER_VELOCITY_Y_ID = serverClasses.FindProperty("CCSPlayer", "m_vecVelocity[1]").id;
			PLAYER_VELOCITY_Z_ID = serverClasses.FindProperty("CCSPlayer", "m_vecVelocity[2]").id;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, int v)
		{
			if (serverClass.id == IS_PLAYER_ID)
			{
				// Avoid bounds checking
				ref Player player = ref Unsafe.Add(ref eventHandler.players[0], entity.id - 1);

				if (property.id == PLAYER_HEALTH_ID)
				{
					player.Health = v;
				}
				else if (property.id == PLAYER_ARMOR_ID)
				{
					player.Armor = v;
				}
				else if (property.id == PLAYER_ACCOUNT_ID)
				{
					player.Money = v;
				}
				else if (property.id == PLAYER_CURRENT_EQUIPMENT_VALUE_ID)
				{
					player.CurrentEquipmentValue = v;
				}
				else if (property.id == PLAYER_ROUND_START_EQUIPMENT_VALUE_ID)
				{
					player.RoundStartEquipmentValue = v;
				}
				else if (property.id == PLAYER_FREEZE_END_EQUIPMENT_VALUE_ID)
				{
					player.FreezetimeEndEquipmentValue = v;
				}
				else if (property.id == PLAYER_HAS_DEFUSER_ID)
				{
					player.HasDefuser = v == 1;
				}
				else if (property.id == PLAYER_HAS_HELMET_ID)
				{
					player.HasDefuser = v == 1;
				}
				else if (property.id == PLAYER_DUCKING_ID)
				{
					player.IsDucking = v == 1;
				}
				else if (property.id == PLAYER_IN_BUY_ZONE_ID)
				{
					player.IsDucking = v == 1;
				}
				else if (property.id == PLAYER_TEAM_NUM_ID)
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
			if (serverClass.id == IS_PLAYER_ID)
			{
				// Avoid bounds checking
				ref Player player = ref Unsafe.Add(ref eventHandler.players[0], entity.id - 1);

				if (property.id == PLAYER_VEC_ORIGIN_Z_ID)
				{
					player.Position.Z = v;
				}
				else if (property.id == PLAYER_EYE_ANGLE_X_ID)
				{
					player.ViewDirection.X = v;
				}
				else if (property.id == PLAYER_EYE_ANGLE_Y_ID)
				{
					player.ViewDirection.Y = v;
				}
				else if (property.id == PLAYER_FLASH_DURATION_ID)
				{
					player.FlashDuration = v;
				}
				else if (property.id == PLAYER_VELOCITY_X_ID)
				{
					player.Velocity.X = v;
				}
				else if (property.id == PLAYER_VELOCITY_Y_ID)
				{
					player.Velocity.Y = v;
				}
				else if (property.id == PLAYER_VELOCITY_Z_ID)
				{
					player.Velocity.Z = v;
				}
			}
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, Vector3 v)
		{
			if (serverClass.id == IS_PLAYER_ID)
			{
				if (property.id == PLAYER_VEC_ORIGIN_ID)
				{
					// Avoid bounds checking
					ref Player player = ref Unsafe.Add(ref eventHandler.players[0], entity.id - 1);

					player.Position.X = v.X;
					player.Position.Y = v.Y;
				}
			}
		}
	}
}
