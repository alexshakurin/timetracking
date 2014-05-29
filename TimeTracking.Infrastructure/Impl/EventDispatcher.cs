using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using TimeTracking.EventHandlers;
using TimeTracking.Logging;
using TimeTracking.Model;

namespace TimeTracking.Infrastructure.Impl
{
	public class EventDispatcher
	{
		private readonly Dictionary<Type, Action<IVersionedEvent>> handlers;

		public EventDispatcher()
		{
			handlers = new Dictionary<Type, Action<IVersionedEvent>>();
		}

		public void Register<T>() where T : IVersionedEvent
		{
			var eventType = typeof (T);
			handlers[eventType] = @event =>
			{
				var eventHandlers = ServiceLocator.Current.GetAllInstances<IEventHandler<T>>();
				foreach (var eventHandler in eventHandlers)
				{
					eventHandler.Handle((T)@event);
				}
			};
		}

		public void Dispatch(IVersionedEvent @event)
		{
			Action<IVersionedEvent> handler;

			var eventType = @event.GetType();

			if (!handlers.TryGetValue(eventType, out handler))
			{
				LogHelper.Error(string.Format("Unable to find handler for event type {0}", eventType));
			}
			else
			{
				handler(@event);
			}
		}
	}
}