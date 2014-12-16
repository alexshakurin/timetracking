using System;

namespace TimeTracking.Extensions.Exceptions
{
	public class UnrecoverableApplicationException : ApplicationException
	{
		public UnrecoverableApplicationException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}