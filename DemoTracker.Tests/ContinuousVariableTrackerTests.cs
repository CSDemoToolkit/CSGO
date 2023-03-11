using DemoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DemoTracker.Tests
{
	[TestClass]
	public class ContinuousVariableTrackerTests
	{
		[TestMethod]
		public void Insert()
		{
			ContinuousVariableTracker<Vector3> tracker = new ContinuousVariableTracker<Vector3>();
			Assert.AreEqual(0, tracker.Count);

			// Some movement
			tracker.Add(10, new Vector3(0, 0, 0));
			tracker.Add(11, new Vector3(0, 0, 1));
			tracker.Add(12, new Vector3(0, 0, 2));

			// All values in range [0, lastTick] should be retrieveable. So all empty ticks between that where there is no
			// values should be 'padded', and Count should reflect that, so we can use it to infer last tick with data.
			Assert.AreEqual(13, tracker.Count);
		}

		[TestMethod] public void InsertNull()
		{
			ContinuousVariableTracker<Vector3> tracker = new ContinuousVariableTracker<Vector3>();
			tracker.Add(10, new Vector3(0, 0, 0));
			tracker.Add(11, new Vector3(0, 0, 1));
			tracker.Add(12, new Vector3(0, 0, 2));

			// Empty values provide no information, as all ticks outside provided data are already
			// presumed 'null'. Therefore they should not cause padding
			tracker.Add(2023, null);
			Assert.AreEqual(13, tracker.Count);
		}

		[TestMethod]
		public void Get()
		{
			ContinuousVariableTracker<Vector3> tracker = new ContinuousVariableTracker<Vector3>();
			int tickMovementBegin1 = 1024;
			int tickMovementBegin2 = 2864;
			tracker.Add(tickMovementBegin1, new Vector3(0, 0, 1));
			tracker.Add(tickMovementBegin1 + 1, new Vector3(0, 0, 2));
			tracker.Add(tickMovementBegin1 + 2, new Vector3(0, 0, 3));
			tracker.Add(tickMovementBegin2, new Vector3(1, 0, 0));
			tracker.Add(tickMovementBegin2 + 1, new Vector3(2, 0, 0));
			tracker.Add(tickMovementBegin2 + 2, new Vector3(3, 0, 0));

			// Movement prior to, between the segments, and after, should be null
			Assert.AreEqual(null, tracker[tickMovementBegin1 - 1]);
			Assert.AreEqual(null, tracker[tickMovementBegin2 - 1]);
			Assert.AreEqual(null, tracker[tickMovementBegin2 + 3]);

			// Test ticks where there are movements
			Assert.AreEqual(new Vector3(0, 0, 2), tracker[tickMovementBegin1 + 1]);
			Assert.AreEqual(new Vector3(2, 0, 0), tracker[tickMovementBegin2 + 1]);
		}
	}
}
