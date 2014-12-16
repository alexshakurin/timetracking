using System;
using TimeTracking.Model.Events;

namespace TimeTracking.EventHandlers
{
	public class WorkingTimeDeletedFileWriterHandler : IEventHandler<WorkingTimeDeleted>
	{
		public void Handle(WorkingTimeDeleted @event)
		{
			throw new NotImplementedException();
		}
	}
}