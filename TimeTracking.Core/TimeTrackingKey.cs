using System;

namespace TimeTracking.Core
{
	public class TimeTrackingKey
	{
		public string Key { get; private set; }

		public DateTime Date { get; private set; }

		public TimeTrackingKey(string key, DateTime date)
		{
			Key = key;
			Date = date;
		}
	}
}