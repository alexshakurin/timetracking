using System;
using System.Threading;
using System.Threading.Tasks;
using TimeTracking.Commands;
using TimeTracking.Infrastructure;

namespace TimeTracker.TimePublishing
{
	public static class TimePublisher
	{
		private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

		//public static void PublishTimeRegistration(ICommandBus commandBus,
		//	string timeKey,
		//	DateTime date,
		//	TimeSpan duration,
		//	string memo,
		//	Action<Exception> onTimeRegistrationError)
		//{
		//	PublishTimeRegistration(commandBus,
		//		new RegisterTimeCommand(timeKey, date, duration, memo),
		//		onTimeRegistrationError);
		//}

		public static void PublishTimeRegistration(ICommandBus commandBus,
			RegisterTimeCommand command,
			Action<Exception> onTimeRegistrationError)
		{
			Task.Run(async () =>
			{
				Exception error = null;

				try
				{
					await semaphoreSlim.WaitAsync();
					await commandBus.Publish(command);
				}
				catch (Exception ex)
				{
					error = ex;
				}
				finally
				{
					semaphoreSlim.Release();
				}

				if (error != null)
				{
					onTimeRegistrationError(error);
				}
			});
		}
	}
}