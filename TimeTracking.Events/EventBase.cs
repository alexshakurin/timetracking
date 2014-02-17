using System;

namespace TimeTracking.Events
{
	public abstract class EventBase : IEvent
	{
		public Guid EventId { get; private set;}

		protected EventBase(Guid eventId)
		{
			EventId = eventId;
		}

		protected EventBase() : this(Guid.NewGuid())
		{
		}
	}
}