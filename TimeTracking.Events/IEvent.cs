using System;

namespace TimeTracking.Events
{
	public interface IEvent
	{
		Guid EventId { get; }
	}
}