namespace DemoReader
{
	public class ScoreEventHandler
	{
		DemoEventHandler eventHandler;

		int tScore = 0;
		int tEnt = 0;
		string tTeamName = "";

		int ctScore = 0;
		int ctEnt = 0;
		string ctTeamName = "";

		public ScoreEventHandler(DemoEventHandler eventHandler)
		{
			this.eventHandler = eventHandler;
		}

		public void Execute(ref Entity entity, in SendProperty property, int v)
		{
			if (entity.id == tEnt && property.varName == "m_scoreTotal")
			{
				eventHandler.InvokeScoreChanged(v, tScore, Team.Terrorists);
				tScore = v;
			}
			else if (entity.id == ctEnt && property.varName == "m_scoreTotal")
			{
				eventHandler.InvokeScoreChanged(v, ctScore, Team.CounterTerrorists);
				ctScore = v;
			}
		}

		public void Execute(ref Entity entity, in SendProperty property, string v)
		{
			if (property.varName == "m_szTeamname")
			{
				if (v == "TERRORIST")
				{
					tEnt = entity.id;
				}
				else if (v == "CT")
				{
					ctEnt = entity.id;
				}
			}
			else if (property.varName == "m_szClanTeamname")
			{
				if (entity.id == tEnt)
				{
					tTeamName = v;
				}
				else if (entity.id == ctEnt)
				{
					ctTeamName = v;
				}
			}
		}

		public int GetCTID()
		{
			return ctEnt;
		}

		public int GetTID()
		{
			return tEnt;
		}
	}
}
