using System;

namespace TimeTracking.Model.Exceptions
{
	public class IntervalNotBelongToDayException : Exception
	{
		public IntervalNotBelongToDayException(string message) : base(message)
		{
		}
	}
}