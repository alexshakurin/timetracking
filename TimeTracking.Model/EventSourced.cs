using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeTracking.Model
{
	public abstract class EventSourced : IEventSourced
	{
		private readonly Dictionary<Type, Action<IVersionedEvent>> handlers = new Dictionary<Type, Action<IVersionedEvent>>();
		private readonly List<IVersionedEvent> pendingEvents = new List<IVersionedEvent>();

		private readonly string id;
		private long version;

		public long Version
		{
			get
			{
				return version;
			}
		}

		public string Id
		{
			get
			{
				return id;
			}
		}

		public IReadOnlyCollection<IVersionedEvent> Events
		{
			get
			{
				return pendingEvents.ToList().AsReadOnly();
			}
		}

		protected EventSourced(string id)
		{
			this.id = id;
		}

		protected void Handles<TEvent>(Action<TEvent> handler)
		{
			handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
		}

		protected void LoadFrom(IReadOnlyCollection<IVersionedEvent> events)
		{
			foreach (var @event in events)
			{
				InvokeEvent(@event);
				version = @event.Version;
			}
		}

		protected void Update(VersionedEvent @event)
		{
			@event.SourceId = Id;
			@event.Version = version + 1;
			InvokeEvent(@event);
			version = @event.Version;
			pendingEvents.Add(@event);
		}

		private void InvokeEvent<TEvent>(TEvent @event) where TEvent : IVersionedEvent
		{
			handlers[@event.GetType()].Invoke(@event);
		}
	}
}