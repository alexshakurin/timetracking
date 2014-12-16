using System;
using TimeTracking.Extensions.Exceptions;

namespace TimeTracking.Extensions
{
	public static class ExceptionExtensionMethods
	{
		public static bool IsFatal(this Exception ex)
		{
			return ex is OutOfMemoryException
				|| ex is StackOverflowException
				|| ex is UnrecoverableApplicationException;
		}
	}
}