using System;
using System.Collections.Generic;
using System.Linq;
using TimeTracking.Model.Events;

namespace TimeTracking.Model
{
	public class WorkingTime : EventSourced
	{
		public TimeSpan TotalTime { get; private set; }

		public WorkingTime(string id)
			: base(id)
		{
			Handles<WorkingTimeRegistered>(OnWorkingTimeRegistered);
		}

		public WorkingTime(string id, IEnumerable<IVersionedEvent> history)
			: this(id)
		{
			LoadFrom(history.ToList().AsReadOnly());
		}

		public void RegisterTime(DateTime date, TimeSpan time, string memo)
		{
			if (TotalTime.TotalSeconds == 0 && time.TotalSeconds < 0)
			{
				throw new ArgumentOutOfRangeException("time", "Can't add negative time because working time is zero");
			}

			Update(new WorkingTimeRegistered(date, time, memo));
		}

		private void OnWorkingTimeRegistered(WorkingTimeRegistered @event)
		{
			TotalTime += @event.Time;
		}
	}
}