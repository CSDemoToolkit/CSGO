namespace DemoReader
{
	public static class Ext
	{
		public static ref ServerClass FindServerClass(this Span<ServerClass> span, string serverClass)
		{
			for (int i = 0; i < span.Length; i++)
			{
				if (span[i].name == serverClass)
				{
					return ref span[i];
				}
			}

			throw new Exception("Server class not found");
		}

		public static ref SendProperty FindProperty(this Span<ServerClass> span, string serverClass, string property)
		{
			ref ServerClass sc = ref span.FindServerClass(serverClass);
			for (int i = 0; i < sc.properties.Length; i++)
			{
				if (sc.properties[i].varName == property)
				{
					return ref sc.properties[i];
				}
			}

			throw new Exception("Property not found");
		}

		public static ref SendProperty FindProperty(this Span<ServerClass> span, string serverClass, string property, string parent)
		{
			ref ServerClass sc = ref span.FindServerClass(serverClass);
			for (int i = 0; i < sc.properties.Length; i++)
			{
				if (sc.properties[i].parentName == parent && sc.properties[i].varName == property)
				{
					return ref sc.properties[i];
				}
			}

			throw new Exception("Property not found");
		}

		public static ref SendProperty FindParentProperty(this Span<ServerClass> span, string serverClass, string property)
		{
			ref ServerClass sc = ref span.FindServerClass(serverClass);
			for (int i = 0; i < sc.properties.Length; i++)
			{
				//Console.WriteLine($"{sc.properties[i].parentName} - {sc.properties[i].parent}");
				if (sc.properties[i].parentName == property)
				{
					return ref sc.properties[i];
				}
			}

			throw new Exception("Property not found");
		}
	}

	public class ScorePacketHandler
	{
		DemoPacketContainer container;
		DemoEventHandler eventHandler;

		int tScore = 0;
		//int tEnt = 0;
		string tTeamName = "";

		int ctScore = 0;
		//int ctEnt = 0;
		string ctTeamName = "";

		Guid SCORE_TOTAL_ID;
		Guid TEAM_NAME_ID;
		Guid CLAN_NAME_ID;

		public ScorePacketHandler(DemoPacketContainer container, DemoEventHandler eventHandler)
		{
			this.container = container;
			this.eventHandler = eventHandler;
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			SCORE_TOTAL_ID = serverClasses.FindProperty("CCSTeam", "m_scoreTotal").id;
			TEAM_NAME_ID = serverClasses.FindProperty("CCSTeam", "m_szTeamname").id;
			CLAN_NAME_ID = serverClasses.FindProperty("CCSTeam", "m_szClanTeamname").id;
		}

		public void Execute(ref Entity entity, ref SendProperty property, int v)
		{
			if (property.id == SCORE_TOTAL_ID)
			{
				if (entity.id == container.tEnt)
				{
					eventHandler.InvokeScoreChanged(v, tScore, Team.Terrorists);
					tScore = v;
				}
				else if (entity.id == container.ctEnt)
				{
					eventHandler.InvokeScoreChanged(v, ctScore, Team.CounterTerrorists);
					ctScore = v;
				}
			}
		}

		public void Execute(ref Entity entity, ref SendProperty property, string v)
		{
			if (property.id == TEAM_NAME_ID)
			{
				if (v == "TERRORIST")
				{
					container.tEnt = entity.id;
				}
				else if (v == "CT")
				{
					container.ctEnt = entity.id;
				}
			}
			else if (property.id == CLAN_NAME_ID)
			{
				if (entity.id == container.tEnt)
				{
					tTeamName = v;
				}
				else if (entity.id == container.ctEnt)
				{
					ctTeamName = v;
				}
			}
		}

		public int GetCTID()
		{
			return container.ctEnt;
		}

		public int GetTID()
		{
			return container.tEnt;
		}
	}
}
