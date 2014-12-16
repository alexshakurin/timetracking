using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimeTracking.Infrastructure.Serialization;
using TimeTracking.LocalStorage;
using TimeTracking.Model;

namespace TimeTracking.Infrastructure.Impl
{
	public class SqlEventSourcedRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourced
	{
		private static readonly string sourceType = typeof(T).Name;
		private readonly ITextSerializer serializer;
		private readonly IEventBus eventBus;
		private readonly Func<EventStoreDbContext> contextFactory;
		private readonly Func<string, IEnumerable<IVersionedEvent>, T> entityFactory;

		// Unity container automatically resolves Func<T> to Resolve<T>
		public SqlEventSourcedRepository(IEventBus eventBus,
			ITextSerializer serializer,
			Func<EventStoreDbContext> contextFactory)
		{
			this.eventBus = eventBus;
			this.serializer = serializer;
			this.contextFactory = contextFactory;

			var constructor = typeof(T).GetConstructor(new[] { typeof(string), typeof(IEnumerable<IVersionedEvent>) });
			if (constructor == null)
			{
				throw new InvalidCastException("Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
			}

			entityFactory = (id, events) => (T)constructor.Invoke(new object[] { id, events });
		}

		public IReadOnlyCollection<string> ListAllKeys()
		{
			IReadOnlyCollection<string> list;
			using (var context = contextFactory.Invoke())
			{
				list = context.Set<StoredEvent>()
					.Where(x => x.AggregateType == sourceType)
					.Select(x => x.AggregateId)
					.Distinct()
					.ToList()
					.AsReadOnly();
			}

			return list;
		}

		public T Find(string id)
		{
			using (var context = contextFactory.Invoke())
			{
				var deserialized = context.Set<StoredEvent>()
					.Where(x => x.AggregateId == id && x.AggregateType == sourceType)
					.OrderBy(x => x.Version)
					.AsEnumerable()
					.Select(Deserialize)
					.ToList();

				if (deserialized.Any())
				{
					return entityFactory.Invoke(id, deserialized);
				}

				return null;
			}
		}

		public void Save(T eventSourced, string correlationId)
		{
			var events = eventSourced.Events.ToArray();
			using (var context = contextFactory.Invoke())
			{
				var eventsSet = context.Set<StoredEvent>();
				foreach (var e in events)
				{
					eventsSet.Add(Serialize(e, correlationId));
				}

				context.SaveChanges();
			}

			eventBus.Publish(events.ToList().AsReadOnly());
		}

		private StoredEvent Serialize(IVersionedEvent e, string correlationId)
		{
			StoredEvent serialized;
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, e);
				serialized = new StoredEvent
				{
					AggregateId = e.SourceId,
					AggregateType = sourceType,
					Version = e.Version,
					Payload = writer.ToString(),
					CorrelationId = correlationId
				};
			}
			return serialized;
		}

		private IVersionedEvent Deserialize(StoredEvent @event)
		{
			using (var reader = new StringReader(@event.Payload))
			{
				return (IVersionedEvent)serializer.Deserialize(reader);
			}
		}
	}
}