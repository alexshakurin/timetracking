using TimeTracking.Events;

namespace TimeTracking.Infrastructure
{
	public interface IEventBus
	{
		void Publish<TEvent>(TEvent @event) where TEvent : IEvent;
	}
}