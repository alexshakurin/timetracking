using System.Collections.Generic;
using TimeTracking.Model;

namespace TimeTracking.Infrastructure
{
	public interface IEventBus
	{
		void Publish(IReadOnlyCollection<IVersionedEvent> @events);
	}
}