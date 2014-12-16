using System.Collections.Generic;

namespace TimeTracking.Model
{
	public interface IEventSourced
	{
		string Id { get; }

		long Version { get; }

		IReadOnlyCollection<IVersionedEvent> Events { get; }
	}
}