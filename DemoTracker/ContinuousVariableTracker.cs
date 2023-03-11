using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoTracker
{
	public class ContinuousVariableTracker<T> where T : unmanaged
	{
		private List<T> _values;
		
		private class ValueSegment
		{
			public int StartTick;
			public int TickDuration;
			public int ValuesIndex;

			public ValueSegment(int startTick, int valuesIndex)
			{
				StartTick = startTick;
				TickDuration = 1;
				ValuesIndex = valuesIndex;
			}

			public int EndTick
			{
				get { return StartTick + TickDuration - 1; }
			}

			public bool IsTickInsideSegment(int tick)
			{
				return tick >= StartTick && tick <= EndTick;
			}

			public int GetTickValuesIndex(int tick)
			{
				int ticksSinceStartOfSegment = tick - StartTick;
				int tickValuesIndex = ValuesIndex + ticksSinceStartOfSegment;
				return Math.Clamp(tickValuesIndex, ValuesIndex, ValuesIndex + TickDuration);
			}
		}

		private SortedList<int, ValueSegment> _segmentsByTick;

		private ValueSegment? _currentSegment;
		private int _prevSegmentEndTick;
		private int _nextSegmentStartTick;

		public ContinuousVariableTracker()
		{
			_values = new List<T>();
			_segmentsByTick = new SortedList<int, ValueSegment>();

			_currentSegment = null;
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
				ValueSegment lastSegment = _segmentsByTick.Last().Value;
				return lastSegment.EndTick + 1;
			}
		}

		public void Add(int tick, T? value)
		{
			if (value == null || EqualityComparer<T?>.Default.Equals(value, default(T)))
			{
				return;
			}

			ValueSegment? segment = FindSegmentBeforeTick(tick);
			int valuesIndex = segment?.GetTickValuesIndex(tick) ?? 0;
			_values.Insert(valuesIndex, (T)value);
			if (segment?.IsTickInsideSegment(tick) ?? false)
			{
				// Tick is already contained, no need to update any Segments
				return;
			}

			if (segment != null && tick == segment.EndTick + 1)
			{
				// Value tails prior segment, extend it
				segment.TickDuration++;
			}
			else
			{
				// No segment contains this tick, create new one
				_segmentsByTick.Add(tick, new ValueSegment(tick, valuesIndex));
			}

			bool wasValueInsertedInMiddle = (Count - 1 > tick);
			if (wasValueInsertedInMiddle)
			{
				UpdateSegmentsAfterTick(tick);
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
					return null;
				}
				if (tick < 0)
				{
					throw new IndexOutOfRangeException($"Cannot access tick {tick}. Tick cannot be negative.");
				}

				if (_currentSegment == null)
				{
					return UpdateSegmentBoundary(tick);
				}
				if (_currentSegment.IsTickInsideSegment(tick))
				{
					return _values[_currentSegment.GetTickValuesIndex(tick)];
				}

				// Tick is outside current Segment
				bool isTickBeforeFirstSegment = (tick < _currentSegment.StartTick && _prevSegmentEndTick == -1);
				bool isTickAfterLastSegment = (tick > _currentSegment.EndTick && _nextSegmentStartTick == -1);
				bool isTickBetweenPrevAndNextSegments = (tick > _prevSegmentEndTick && tick < _nextSegmentStartTick);
				if (isTickBeforeFirstSegment || isTickBetweenPrevAndNextSegments || isTickAfterLastSegment)
				{
					return null;
				}

				// Tick is outside current Boundary, update it
				return UpdateSegmentBoundary(tick);
			}
		}

		private T? UpdateSegmentBoundary(int tick)
		{
			_currentSegment = FindSegmentBeforeTick(tick);
			if (_currentSegment == null)
			{
				return null;
			}

			int indexOfCurrentSegment = _segmentsByTick.IndexOfKey(_currentSegment.StartTick);
			_prevSegmentEndTick = -1;
			_nextSegmentStartTick = -1;
			if (indexOfCurrentSegment > 0)
			{
				_prevSegmentEndTick = _segmentsByTick.GetValueAtIndex(indexOfCurrentSegment - 1).EndTick;
			}
			if (indexOfCurrentSegment < _segmentsByTick.Count - 1)
			{
				_nextSegmentStartTick = _segmentsByTick.GetValueAtIndex(indexOfCurrentSegment + 1).StartTick;
			}
			return this[tick];
		}

		private ValueSegment? FindSegmentBeforeTick(int tick)
		{
			return _segmentsByTick.Values.LastOrDefault(x => x.StartTick <= tick, null);
		}

		private void UpdateSegmentsAfterTick(int tick)
		{
			// Combine segments if tick connects them
			ValueSegment segment = FindSegmentBeforeTick(tick);
			ValueSegment? segmentStartingOnNextTick = _segmentsByTick.GetValueOrDefault(tick + 1, null);
			if (segmentStartingOnNextTick != null)
			{
				segment.TickDuration += segmentStartingOnNextTick.TickDuration;
				_segmentsByTick.Remove(tick + 1);
			}

			// There are segments following the one for current tick. Since we inserted value
			// in middle of _values, we need to update the Segment tick indices
			foreach (ValueSegment followingSegment in _segmentsByTick.Values)
			{
				if (followingSegment.StartTick <= tick)
				{
					continue;
				}
				followingSegment.ValuesIndex++;
			}
		}
	}
}
