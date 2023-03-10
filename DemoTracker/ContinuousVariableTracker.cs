using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoTracker
{
	public class ContinuousVariableTracker<T>
	{
		private List<T> _values;
		
		private class ValueSegment
		{
			public int TickDuration;
			public int ValuesIndex;

			public ValueSegment(int valuesIndex)
			{
				TickDuration = 1;
				ValuesIndex = valuesIndex;
			}
		}
		private SortedList<int, ValueSegment> _segmentsByTick;

		private int _currentSegmentStartTick;
		private ValueSegment _currentSegment;
		private int _prevSegmentEndTick;
		private int _nextSegmentStartTick;

		public ContinuousVariableTracker()
		{
			_values = new List<T>();
			_segmentsByTick = new SortedList<int, ValueSegment>();

			_currentSegmentStartTick = -1;
			_currentSegment = new ValueSegment(-1);
			_prevSegmentEndTick = -1;
			_nextSegmentStartTick = -1;
		}

		public int Count
		{
			get
			{
				if (_values.Count == 0)
				{
					return 0;
				}
				var lastEntry = _segmentsByTick.Last();
				int tickValueSegmentStarts = lastEntry.Key;
				int valueSegmentDuration = lastEntry.Value.TickDuration;
				return tickValueSegmentStarts + valueSegmentDuration;
			}
		}

		public void Add(int tick, T? value)
		{
			if (value == null || EqualityComparer<T>.Default.Equals(value, default(T)))
			{
				return;
			}

			bool valueInsertedInMiddle = false;
			int closestSegmentStartTick = _segmentsByTick.Keys.LastOrDefault(x => x <= tick, -1);
			if (closestSegmentStartTick == -1)
			{
				// No segment prior to tick, i.e. earliest tick added so far
				valueInsertedInMiddle = (Count != 0);
				_values.Insert(0, value);
				_segmentsByTick.Add(tick, new ValueSegment(0));
			}
			else
			{
				ValueSegment closestSegment = _segmentsByTick[closestSegmentStartTick];
				int closestSegmentEndTick = closestSegmentStartTick + closestSegment.TickDuration - 1;

				int ticksSinceStartOfSegment = tick - closestSegmentStartTick;
				int valuesIndex = closestSegment.ValuesIndex + ticksSinceStartOfSegment;

				if (tick >= closestSegmentStartTick && tick <= closestSegmentEndTick)
				{
					// Tick is already contained, just update value
					_values[valuesIndex] = value;
					return;
				}

				bool tickTrailsSegment = (tick == closestSegmentEndTick + 1);
				valuesIndex = Math.Min(valuesIndex, closestSegment.ValuesIndex + closestSegment.TickDuration);
				_values.Insert(valuesIndex, value);
				valueInsertedInMiddle = (Count - 1 > tick);
				if (tickTrailsSegment)
				{
					// Value tails prior segment, extend it
					_segmentsByTick[closestSegmentStartTick].TickDuration++;
				}
				else
				{
					// No segment contains this tick, create new one
					_segmentsByTick.Add(tick, new ValueSegment(valuesIndex));
				}
			}

			if (valueInsertedInMiddle)
			{
				int closestNextSegmentTick = _segmentsByTick.Keys.FirstOrDefault(x => x > tick, -1);
				if (tick == closestNextSegmentTick - 1)
				{
					// Tick prepends next segment, remove it
					_segmentsByTick[closestSegmentStartTick].TickDuration += _segmentsByTick[closestNextSegmentTick].TickDuration;
					_segmentsByTick.Remove(closestNextSegmentTick);
				}
				if (closestNextSegmentTick != -1)
				{
					// There are segments following the one for current tick. Since we inserted value
					// in middle of _values, we need to update the Segment tick indices
					foreach (int segmentStartTick in _segmentsByTick.Keys)
					{
						if (segmentStartTick <= tick)
						{
							continue;
						}
						_segmentsByTick[segmentStartTick].ValuesIndex++;
					}
				}
			}
		}

		public KeyValuePair<int, T?> First()
		{
			int firstTickWithValue = _segmentsByTick.First().Key;
			return new KeyValuePair<int, T?>(firstTickWithValue, this[firstTickWithValue]);
		}

		public T? this[int tick]
		{
			get
			{
				if (Count == 0)
				{
					throw new IndexOutOfRangeException($"Cannot access tick {tick}. Buffer is empty");
				}
				if (tick < 0)
				{
					throw new IndexOutOfRangeException($"Cannot access tick {tick}. Tick cannot be negative.");
				}

				if (_currentSegmentStartTick == -1)
				{
					return FindValueSegment(tick);
				}

				// Cases where tick is outside current Segment
				if (tick < _currentSegmentStartTick)
				{
					if (_prevSegmentEndTick == -1 || tick > _prevSegmentEndTick)
					{
						return default(T);
					}
					// Value belongs to Segment outside current, update
					return FindValueSegment(tick);
				}
				int currentSegmentEndTick = _currentSegmentStartTick + _currentSegment.TickDuration - 1;
				if (tick > currentSegmentEndTick)
				{
					if (_nextSegmentStartTick == -1 || tick < _nextSegmentStartTick)
					{
						return default(T);
					}
					// Value belongs to Segment outside current, update
					return FindValueSegment(tick);
				}

				int ticksSinceStartOfSegment = tick - _currentSegmentStartTick;
				int valuesIndex = _currentSegment.ValuesIndex + ticksSinceStartOfSegment;
				return _values[valuesIndex];
			}
		}

		public T? FindValueSegment(int tick)
		{
			_currentSegmentStartTick = _segmentsByTick.Keys.LastOrDefault(x => x <= tick, -1);
			if (_currentSegmentStartTick == -1)
			{
				return default(T);
			}

			_currentSegment = _segmentsByTick[_currentSegmentStartTick];
			int indexOfCurrentSegment = _segmentsByTick.IndexOfKey(_currentSegmentStartTick);
			if (indexOfCurrentSegment > 0)
			{
				int prevSegmentStartTick = _segmentsByTick.GetKeyAtIndex(indexOfCurrentSegment - 1);
				_prevSegmentEndTick = prevSegmentStartTick + _segmentsByTick[prevSegmentStartTick].TickDuration;
			}
			if (indexOfCurrentSegment < _segmentsByTick.Count - 1)
			{
				_nextSegmentStartTick = _segmentsByTick.GetKeyAtIndex(indexOfCurrentSegment + 1);
			}
			else
			{
				_nextSegmentStartTick = -1;
			}
			return this[tick];
		}
	}
}
