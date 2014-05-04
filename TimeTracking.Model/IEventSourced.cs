using System;
using System.Collections.Generic;

namespace TimeTracking.Model
{
	public interface IEventSourced
	{
		Guid Id { get; }

		long Version { get; }

		IReadOnlyCollection<IVersionedEvent> Events { get; }
	}
}