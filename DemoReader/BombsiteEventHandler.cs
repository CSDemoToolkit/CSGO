using System.Numerics;

namespace DemoReader
{
	public class BombsiteEventHandler
	{
		DemoEventHandler eventHandler;

		public BombsiteEventHandler(DemoEventHandler eventHandler)
		{
			this.eventHandler = eventHandler;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, Vector3 v)
		{
			if (serverClass.name == "CCSPlayerResource")
			{
				if (property.varName == "m_bombsiteCenterA")
				{
					ref Bombsite bombsite = ref eventHandler.bombsites[0];
					bombsite.BombsiteEnt = entity.id;
					bombsite.Center.X = v.X;
					bombsite.Center.Y = v.Y;
				}
				else if(property.varName == "m_bombsiteCenterB")
				{
					ref Bombsite bombsite = ref eventHandler.bombsites[1];
					bombsite.BombsiteEnt = entity.id;
					bombsite.Center.X = v.X;
					bombsite.Center.Y = v.Y;
				}
			}
			else if(serverClass.name == "CBaseTrigger")
			{
				int bombsiteIdx = 0;
				for (int i = 0; i < eventHandler.bombsites.Length; i++)
				{
					if (eventHandler.bombsites[i].BombsiteEnt == entity.id)
					{
						bombsiteIdx = i;
						break;
					}
				}

				ref Bombsite bombsite = ref eventHandler.bombsites[bombsiteIdx];
				if (property.varName == "m_vecMins")
				{
					bombsite.BoundingBox.X = v.X;
					bombsite.BoundingBox.Y = v.Y;
				}
				else if (property.varName == "m_vecMaxs")
				{
					bombsite.BoundingBox.Z = v.X;
					bombsite.BoundingBox.W = v.Y;
				}
			}
		}
	}
}
