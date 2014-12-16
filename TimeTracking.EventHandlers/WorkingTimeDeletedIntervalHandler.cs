using TimeTracking.Model.Events;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeDeletedIntervalHandler : IEventHandler<WorkingTimeDeleted>
	{
		public void Handle(WorkingTimeDeleted @event)
		{
			throw new System.NotImplementedException();
		}
	}
}