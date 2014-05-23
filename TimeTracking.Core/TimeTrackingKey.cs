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