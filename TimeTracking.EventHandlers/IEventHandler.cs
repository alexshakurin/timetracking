
using TimeTracking.Events;

namespace TimeTracking.EventHandlers
{
	public interface IEventHandler<TEvent>
		where TEvent : IEvent
	{
		void Handle(TEvent @event);
	}
}