using DemoInfo;
using System.Diagnostics;
using Tracker;

namespace DemoTracker.Tests
{
	[TestClass]
	public class VariableTrackerTests
	{
		[TestMethod]
		public void Insert()
		{
			VariableTracker<Team> tracker = new VariableTracker<Team>();
			Assert.AreEqual(0, tracker.Count);

			// Player joins Spectators
			int tickPlayerJoined = 2023;
			tracker.Add(tickPlayerJoined, Team.Terrorist);
			Assert.AreEqual(1, tracker.Count);

			// Redundant value, should not be added
			tracker.Add(tickPlayerJoined, Team.Terrorist);
			Assert.AreEqual(1, tracker.Count);

			// Redundant value added non-chronologically, should not be added
			tracker.Add(tickPlayerJoined - 10, Team.Spectate);
			Assert.AreEqual(2, tracker.Count);
			tracker.Add(tickPlayerJoined - 10, Team.Spectate);
			Assert.AreEqual(2, tracker.Count);
		}


		[TestMethod]
		public void Get()
		{
			VariableTracker<Team> tracker = new VariableTracker<Team>();
			int tickPlayerJoined = 2048;
			int tickPlayerJoinedTerrorists = 4184;
			int tickPlayerJoinedCounterTerrorists = 12904;
			tracker.Add(tickPlayerJoined, Team.Spectate);
			tracker.Add(tickPlayerJoinedTerrorists, Team.Terrorist);
			tracker.Add(tickPlayerJoinedCounterTerrorists, Team.CounterTerrorist);
			Assert.AreEqual(3, tracker.Count);

			// Player not joined yet
			Assert.AreEqual(null, tracker[tickPlayerJoined - 100]);

			// Player is now Spectator
			Assert.AreEqual(Team.Spectate, tracker[tickPlayerJoined]);
			Assert.AreEqual(Team.Spectate, tracker[tickPlayerJoined + 10]);
			Assert.AreEqual(Team.Spectate, tracker[tickPlayerJoinedTerrorists - 1]);

			// Player joined Terrorists
			Assert.AreEqual(Team.Terrorist, tracker[tickPlayerJoinedTerrorists]);
			Assert.AreEqual(Team.Terrorist, tracker[tickPlayerJoinedTerrorists + 10]);
			Assert.AreEqual(Team.Terrorist, tracker[tickPlayerJoinedCounterTerrorists - 1]);

			// Player joined Counter-Terrorists, until game ended
			Assert.AreEqual(Team.CounterTerrorist, tracker[tickPlayerJoinedCounterTerrorists]);
			Assert.AreEqual(Team.CounterTerrorist, tracker[tickPlayerJoinedCounterTerrorists + 100]);
			Assert.AreEqual(Team.CounterTerrorist, tracker[tickPlayerJoinedCounterTerrorists + 10000]);
		}
	}
}