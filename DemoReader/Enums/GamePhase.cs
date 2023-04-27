namespace DemoReader
{
	public enum GamePhase
	{
		Init = 0,
		Pregame,
		StartGame,
		TeamSideSwitch, // Set when a team side switch happened usually at the beggining of the new round
		GameHalfEnded,
		GameEnded,
		StaleMate,
		GameOver,
	}
}
