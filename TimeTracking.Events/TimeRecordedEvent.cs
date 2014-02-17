using System;

namespace TimeTracking.Events
{
	public class TimeRecordedEvent : EventBase
	{
		public DateTime Date { get; private set; }

		public int Minutes { get; private set; }

		public TimeRecordedEvent(DateTime date, int minutes)
		{
			Date = date.Date;
			Minutes = minutes;
		}
	}
}