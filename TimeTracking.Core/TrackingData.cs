using System;

namespace TimeTracking.Core
{
	public class TrackingData
	{
		public TimeTrackingKey Key { get; private set; }

		public DateTimeOffset Timestamp { get; private set; }

		public TrackingData(TimeTrackingKey key, DateTimeOffset timestamp)
		{
			Key = key;
			Timestamp = timestamp;
		} 
	}
}