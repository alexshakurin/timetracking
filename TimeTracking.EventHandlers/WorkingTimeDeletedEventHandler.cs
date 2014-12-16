using TimeTracking.Model.Events;
using TimeTracking.ReadModel;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeDeletedEventHandler : IEventHandler<WorkingTimeDeleted>
	{
		private readonly ReadModelRepository repository;

		public WorkingTimeDeletedEventHandler(ReadModelRepository repository)
		{
			this.repository = repository;
		}

		public void Handle(WorkingTimeDeleted @event)
		{
			repository.RemoveDayStatistics(@event.SourceId, @event.End - @event.Start);
		}
	}
}