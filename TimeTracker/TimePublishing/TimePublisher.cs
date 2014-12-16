using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using TimeTracking.Commands;
using TimeTracking.Infrastructure;
using TimeTracking.Model;
using TimeTracking.ReadModel;

namespace TimeTracker.TimePublishing
{
	public static class TimePublisher
	{
		private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

		public static void Refresh(Action<Exception> onError)
		{
			Task.Run(async () =>
				{
					Exception error = null;

					try
					{
						await semaphoreSlim.WaitAsync();

						var eventSourcedRepository = ServiceLocator.Current.GetInstance<IEventSourcedRepository<WorkingTime>>();

						var existingKeys = eventSourcedRepository.ListAllKeys();

						var readModelRepository = ServiceLocator.Current.GetInstance<ReadModelRepository>();

						foreach (var aggregateId in existingKeys)
						{
							var workingTime = eventSourcedRepository.Find(aggregateId);
							readModelRepository.SetDayStatistics(aggregateId, workingTime.Total);
						}

						readModelRepository.DeleteMissingKeys(existingKeys);
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
						onError(error);
					}
				});
		}

		public static void PublishTimeRegistration(ICommandBus commandBus,
			RegisterTimeCommand command,
			Action<Exception> onTimeRegistrationError)
		{
			Task.Run(async () =>
			{
				Exception error = null;

				try
				{
					// Allow only one Delete/Register TimeCommand to be processed at a time
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

		public static void PublishTimeDelete(ICommandBus commandBus,
			DeleteTimeCommand command,
			Action<Exception> onTimeDeleteError)
		{
			Task.Run(async () =>
			{
				Exception error = null;

				try
				{
					// Allow only one Delete/Register TimeCommand to be processed at a time
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
					onTimeDeleteError(error);
				}
			});
		}
	}
}