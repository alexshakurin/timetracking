using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace TimeTracking.Model
{
	public interface IEventSourced
	{
		Guid Id { get; }

		int Version { get; }

		IReadOnlyCollection<IVersionedEvent> Events { get; }
	}
}