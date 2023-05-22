using System.Numerics;

namespace DemoReader
{
	public class BombsitePacketHandler
	{
		DemoPacketHandler packetHandler;

		Guid PLAYER_RESOURCE_ID;
		Guid BASE_TRIGGER_ID;
		Guid BOMBSITE_CENTER_A_ID;
		Guid BOMBSITE_CENTER_B_ID;
		Guid VEC_MIN_ID;
		Guid VEC_MAX_ID;

		public BombsitePacketHandler(DemoPacketHandler packetHandler)
		{
			this.packetHandler = packetHandler;
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			PLAYER_RESOURCE_ID = serverClasses.FindServerClass("CCSPlayerResource").id;
			BOMBSITE_CENTER_A_ID = serverClasses.FindProperty("CCSPlayerResource", "m_bombsiteCenterA").id;
			BOMBSITE_CENTER_B_ID = serverClasses.FindProperty("CCSPlayerResource", "m_bombsiteCenterB").id;

			BASE_TRIGGER_ID = serverClasses.FindServerClass("CBaseTrigger").id;
			VEC_MIN_ID = serverClasses.FindProperty("CBaseTrigger", "m_vecMins").id;
			VEC_MAX_ID = serverClasses.FindProperty("CBaseTrigger", "m_vecMaxs").id;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, Vector3 v)
		{
			if (serverClass.id == PLAYER_RESOURCE_ID)
			{
				if (property.id == BOMBSITE_CENTER_A_ID)
				{
					ref BombsiteInfo bombsite = ref packetHandler.container.bombsites[0];
					bombsite.BombsiteGuid = property.id;
					bombsite.BombsiteId = entity.id;
					bombsite.Center.X = v.X;
					bombsite.Center.Y = v.Y;
				}
				else if(property.id == BOMBSITE_CENTER_B_ID)
				{
					ref BombsiteInfo bombsite = ref packetHandler.container.bombsites[1];
					bombsite.BombsiteGuid = property.id;
					bombsite.BombsiteId = entity.id;
					bombsite.Center.X = v.X;
					bombsite.Center.Y = v.Y;
				}
			}
			else if(serverClass.id == BASE_TRIGGER_ID)
			{
				int bombsiteIdx = 0;
				for (int i = 0; i < packetHandler.container.bombsites.Length; i++)
				{
					if (packetHandler.container.bombsites[i].BombsiteGuid == property.id)
					{
						bombsiteIdx = i;
						break;
					}
				}

				ref BombsiteInfo bombsite = ref packetHandler.container.bombsites[bombsiteIdx];
				if (property.id == VEC_MIN_ID)
				{
					bombsite.BoundingBox.X = v.X;
					bombsite.BoundingBox.Y = v.Y;
				}
				else if (property.id == VEC_MAX_ID)
				{
					bombsite.BoundingBox.Z = v.X;
					bombsite.BoundingBox.W = v.Y;
				}
			}
		}
	}
}
