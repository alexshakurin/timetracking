using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using TimeTracking.EventHandlers;
using TimeTracking.Events;

namespace TimeTracking.Infrastructure.Impl
{
	public class EventBus : IEventBus
	{
		public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
		{
			var handlers = ServiceLocator.Current.GetAllInstances<IEventHandler<TEvent>>();

			foreach (var handler in handlers)
			{
				handler.Handle(@event);
			}
		}
	}
}