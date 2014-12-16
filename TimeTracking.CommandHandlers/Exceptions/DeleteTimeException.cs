using System;

namespace TimeTracking.CommandHandlers.Exceptions
{
	public class DeleteTimeException : Exception
	{
		public DeleteTimeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}