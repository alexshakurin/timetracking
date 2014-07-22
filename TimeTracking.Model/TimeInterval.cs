using System;

namespace TimeTracking.Model
{
	public class TimeInterval
	{
		public DateTimeOffset Start { get; private set; }

		public DateTimeOffset End { get; private set; }

		public TimeInterval(DateTimeOffset start, DateTimeOffset end)
		{
			Start = start;
			End = end;
		}

		public bool Intersects(TimeInterval that)
		{
			if (that == null)
			{
				return false;
			}

			return Equals(that)
				|| that.Start.BetweenNotEqual(Start, End)
				|| that.End.BetweenNotEqual(Start, End)
				|| Start.BetweenNotEqual(that.Start, that.End)
				|| End.BetweenNotEqual(that.Start, that.End);
		}

		public override int GetHashCode()
		{
			return string.Format("{0}::{1}", Start, End).GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var that = obj as TimeInterval;

			if (that == null)
			{
				return false;
			}

			return that.Start == Start && that.End == End;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1}", Start, End);
		}
	}
}