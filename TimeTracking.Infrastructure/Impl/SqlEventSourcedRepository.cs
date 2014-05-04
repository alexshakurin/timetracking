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
		private readonly Func<EventStoreDbContext> contextFactory;
		private readonly Func<Guid, IEnumerable<IVersionedEvent>, T> entityFactory;

		public SqlEventSourcedRepository(ITextSerializer serializer,
			Func<EventStoreDbContext> contextFactory)
		{
			this.serializer = serializer;
			this.contextFactory = contextFactory;

			var constructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) });
			if (constructor == null)
			{
				throw new InvalidCastException("Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
			}

			entityFactory = (id, events) => (T)constructor.Invoke(new object[] { id, events });
		}

		public T Find(Guid id)
		{
			var stringId = id.ToString();
			using (var context = contextFactory.Invoke())
			{
				var deserialized = context.Set<StoredEvent>()
					.Where(x => x.AggregateId == stringId && x.AggregateType == sourceType)
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
		}

		private StoredEvent Serialize(IVersionedEvent e, string correlationId)
		{
			StoredEvent serialized;
			using (var writer = new StringWriter())
			{
				serializer.Serialize(writer, e);
				serialized = new StoredEvent
				{
					AggregateId = e.SourceId.ToString(),
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