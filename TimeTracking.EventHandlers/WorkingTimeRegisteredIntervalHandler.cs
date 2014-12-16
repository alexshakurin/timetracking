using TimeTracking.Model.Events;
using TimeTracking.ReadModel;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeRegisteredIntervalHandler : IEventHandler<WorkingTimeRegistered>
	{
		private readonly ReadModelRepository repository;

		public WorkingTimeRegisteredIntervalHandler(ReadModelRepository repository)
		{
			this.repository = repository;
		}

		public void Handle(WorkingTimeRegistered @event)
		{
			repository.AddInterval(@event.SourceId,
				@event.SourceId,
				@event.Start.TimeOfDay,
				@event.End.TimeOfDay,
				@event.Memo);
		}
	}
}