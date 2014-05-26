using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TimeTracking.Logging;
using TimeTracking.Model;

namespace TimeTracking.Infrastructure.Impl
{
	public class EventBus : IEventBus
	{
		private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		private readonly EventDispatcher dispatcher;

		public EventBus(EventDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
		}

		public void Publish(IReadOnlyCollection<IVersionedEvent> @events)
		{
			var localList = new ReadOnlyCollection<IVersionedEvent>(@events.ToList());

			Task.Run(async () =>
				{
					try
					{
						await semaphore.WaitAsync();

						foreach (var @event in localList)
						{
							var localEvent = @event;
							await Task.Run(() => dispatcher.Dispatch(localEvent));
						}
					}
					catch (Exception ex)
					{
						// TODO: Error processing
						LogHelper.Error(string.Format("Event processing failed with error '{0}'", ex));
						throw;
					}
					finally
					{
						semaphore.Release();
					}
				});
		}
	}
}