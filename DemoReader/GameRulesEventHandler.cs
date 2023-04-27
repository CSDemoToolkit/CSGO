namespace DemoReader
{
	public class GameRulesEventHandler
	{
		DemoEventHandler eventHandler;

		public GameRulesEventHandler(DemoEventHandler eventHandler)
		{
			this.eventHandler=eventHandler;
		}

		public void Execute(ref ServerClass serverClass, ref Entity entity, in SendProperty property, int v)
		{
			if (serverClass.name == "CCSGameRulesProxy")
			{
				if (property.varName == "m_gamePhase")
				{
					eventHandler.InvokeGamePhaseChange((GamePhase)v);
				}
				else if (property.varName == "m_iRoundWinStatus")
				{
					eventHandler.InvokeRoundWinStatusChange((RoundWinStatus)v);
				}
			}	
		}
	}
}
