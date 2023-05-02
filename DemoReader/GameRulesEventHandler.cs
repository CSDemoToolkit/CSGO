namespace DemoReader
{
	public class GameRulesEventHandler
	{
		DemoEventHandler eventHandler;

		Guid GAME_RULES_PROXY_ID;
		Guid GAME_PHASE_ID;
		Guid ROUND_WIN_STATUS_ID;

		public GameRulesEventHandler(DemoEventHandler eventHandler)
		{
			this.eventHandler=eventHandler;
		}

		public void Init(Span<ServerClass> serverClasses)
		{
			GAME_RULES_PROXY_ID = serverClasses.FindServerClass("CCSGameRulesProxy").id;
			GAME_PHASE_ID = serverClasses.FindProperty("CCSGameRulesProxy", "m_gamePhase").id;
			ROUND_WIN_STATUS_ID = serverClasses.FindProperty("CCSGameRulesProxy", "m_iRoundWinStatus").id;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, ref SendProperty property, int v)
		{
			if (serverClass.id == GAME_RULES_PROXY_ID)
			{
				if (property.id == GAME_PHASE_ID)
				{
					eventHandler.InvokeGamePhaseChange((GamePhase)v);
				}
				else if (property.id == ROUND_WIN_STATUS_ID)
				{
					eventHandler.InvokeRoundWinStatusChange((RoundWinStatus)v);
				}
			}	
		}
	}
}
