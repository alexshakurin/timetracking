using System;

namespace TimeTracking.Model.Exceptions
{
	public class IntervalDuplicateException : Exception
	{
		public IntervalDuplicateException(string message) : base(message)
		{
		}
	}
}