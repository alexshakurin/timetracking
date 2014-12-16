using System;

namespace TimeTracking.Model
{
	public static class DateTimeExtensionMethods
	{
		public static bool BetweenNotEqual(this DateTimeOffset dateTime, DateTimeOffset start, DateTimeOffset end)
		{
			return dateTime > start && dateTime < end;
		}
	}
}