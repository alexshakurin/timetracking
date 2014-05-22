using System;

namespace TimeTracking.Core
{
	public class TrackingData
	{
		public TimeTrackingKey Key { get; private set; }

		public DateTimeOffset Start { get; private set; }

		public TimeSpan Interval { get; private set; }

		public TrackingData(TimeTrackingKey key, DateTimeOffset start, TimeSpan interval)
		{
			Key = key;
			Start = start;
			Interval = interval;
		}
	}
}