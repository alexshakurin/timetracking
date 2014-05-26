
using TimeTracking.Model.Events;
using TimeTracking.ReadModel;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeRegisteredEventHandler : IEventHandler<WorkingTimeRegistered>
	{
		private readonly ReadModelRepository repository;

		public WorkingTimeRegisteredEventHandler(ReadModelRepository repository)
		{
			this.repository = repository;
		}

		public void Handle(WorkingTimeRegistered @event)
		{
			repository.AddDurationForday(@event.SourceId, @event.End - @event.Start);
		}
	}
}