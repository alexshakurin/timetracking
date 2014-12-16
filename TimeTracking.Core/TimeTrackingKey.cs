using System;

namespace TimeTracking.Core
{
	public class TimeTrackingKey
	{
		private const string format = "yyyy-MM-dd";

		public string Key { get; private set; }

		public DateTime Date { get; private set; }

		public TimeTrackingKey(string key, DateTime date)
		{
			Key = key;
			Date = date;
		}

		public string GetDateString()
		{
			return Date.ToString(format);
		}

		public static TimeTrackingKey FromDate(DateTime dateTime)
		{
			return new TimeTrackingKey(dateTime.ToString(format), dateTime);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			var that = obj as TimeTrackingKey;
			if (that == null)
			{
				return false;
			}

			return string.Equals(Key, that.Key, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return Key.GetHashCode();
		}
	}
}