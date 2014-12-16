
using TimeTracking.Model;

namespace TimeTracking.EventHandlers
{
	public interface IEventHandler
	{
	}

	public interface IEventHandler<TEvent>
		where TEvent : IVersionedEvent
	{
		void Handle(TEvent @event);
	}
}