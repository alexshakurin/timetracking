using System;
using System.Threading.Tasks;
using TimeTracking.Commands;
using TimeTracking.Infrastructure;

namespace TimeTracker.TimePublishing
{
	public static class TimePublisher
	{
		
		public static void PublishTimeRegistration(ICommandBus commandBus,
			string timeKey,
			DateTime date,
			TimeSpan duration,
			string memo,
			Action<Exception> onTimeRegistrationError)
		{
			PublishTimeRegistration(commandBus,
				new RegisterTimeCommand(timeKey, date, duration, memo),
				onTimeRegistrationError);
		}

		public static void PublishTimeRegistration(ICommandBus commandBus,
			RegisterTimeCommand command,
			Action<Exception> onTimeRegistrationError)
		{
			Task.Run(async () =>
			{
				try
				{
					await commandBus.Publish(command);
				}
				catch (Exception ex)
				{
					onTimeRegistrationError(ex);
				}
			});
		}
	}
}