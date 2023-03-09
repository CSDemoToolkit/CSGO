#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Tracker
{
	public class VariableTracker<T>
	{
		private SortedList<int, T?> _valueByTick;
		private T? _prevValue;
		private int _prevValueTick;
		private int _nextValueTick;

		public VariableTracker()
		{
			_valueByTick = new SortedList<int, T?>();

			_prevValue = default(T);
			_prevValueTick = -1;
			_nextValueTick = -1;
		}

		public int Count
		{
			get
			{
				return _valueByTick.Count;
			}
		}

		public void Add(int tick, T? value)
		{
			if (tick < 0)
			{
				throw new IndexOutOfRangeException($"Cannot add tick {tick}. Tick cannot be negative.");
			}
			int closestUpperBoundTickKey = _valueByTick.Keys.LastOrDefault(x => x <= tick, -1);
			if (closestUpperBoundTickKey > 0 && EqualityComparer<T>.Default.Equals(value, _valueByTick[closestUpperBoundTickKey]))
			{
				// Value hasn't changed, do not add
				return;
			}
			_valueByTick.Add(tick, value);
		}

		public KeyValuePair<int, T?> First()
		{
			return _valueByTick.First();
		}

		public KeyValuePair<int, T?> Last()
		{
			return _valueByTick.Last();
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

				if (_prevValueTick == -1 && _nextValueTick == -1)
				{
					return FindValueRange(tick);
				}
				// If tick is within [prevTick, nextTick) range, it hasn't changed
				bool isTickWithinSameBoundsAsLastValue = (tick >= _prevValueTick && tick < _nextValueTick);
				bool isTickAfterLast = (_nextValueTick == -1);
				if (isTickWithinSameBoundsAsLastValue || isTickAfterLast)
				{
					return _prevValue;
				}

				// Tick is within a new range
				return FindValueRange(tick);
			}
		}

		private T? FindValueRange(int tick)
		{
			// TODO: Implement Binary Search to search for values. Not implemented as of now because 
			// _valueByTick.keys is an IList, which doesn't support .BinarySearch(index), like List does.
			int _prevValueTick = _valueByTick.Keys.LastOrDefault(x => x <= tick, -1);
			if (_prevValueTick < 0)
			{
				// tick is before first tick in List
				_prevValue = default(T);
				_nextValueTick = First().Key;
			}
			else if (_prevValueTick == Last().Key)
			{
				// tick is after last tick in List
				_prevValue = _valueByTick[_prevValueTick];
				_nextValueTick = -1;
			}
			else
			{
				// tick is between two ticks in List
				int nextValueTickIndex = _valueByTick.IndexOfKey(_prevValueTick) + 1;
				_prevValue = _valueByTick[_prevValueTick];
				_nextValueTick = _valueByTick.GetKeyAtIndex(nextValueTickIndex);
			}

			return _prevValue;
		}
	}
}
