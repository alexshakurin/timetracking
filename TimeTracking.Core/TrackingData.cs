using System;

namespace TimeTracking.Core
{
	public class TrackingData
	{
		public TimeTrackingKey Key { get; private set; }

		public DateTimeOffset Start { get; private set; }

		public DateTimeOffset CurrentDate { get; private set; }

		public DateTimeOffset PreviousDate { get; private set; }

		public TimeSpan Elapsed { get; private set; }

		public TimeSpan ElapsedSinceStart { get; private set; }

		public TrackingData(TimeTrackingKey key,
			DateTimeOffset start,
			DateTimeOffset currentDate,
			DateTimeOffset previousDate)
		{
			Key = key;
			Start = start;
			CurrentDate = currentDate;
			PreviousDate = previousDate;
			Elapsed = currentDate - previousDate;
			ElapsedSinceStart = currentDate - start;
		}

		public TrackingData Tick(TimeTrackingKey key, DateTimeOffset currentDate)
		{
			// Preserve Start
			// New currentDate becomes actual current date
			// CurrentDate of this instance becomes PreviousDate of new instance
			return new TrackingData(key, Start, currentDate, CurrentDate);
		}
	}
}