using System;

namespace TimeTracking.CommandHandlers.Exceptions
{
	public class RegisterTimeException : Exception
	{
		public RegisterTimeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}