using System;
using System.Collections.Generic;
using System.Linq;
using TimeTracking.Model.Events;

namespace TimeTracking.Model
{
	public class WorkingTime : EventSourced
	{
		public int TotalMinutes { get; private set; }

		public WorkingTime(Guid id) : base(id)
		{
			Handles<WorkingTimeRegistered>(OnWorkingTimeRegistered);
		}

		public WorkingTime(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            LoadFrom(history.ToList().AsReadOnly());
        }

		public void RegisterTime(DateTime date, int minutes, string memo)
		{
			if (TotalMinutes == 0 && minutes < 0)
			{
				throw new ArgumentOutOfRangeException("minutes");
			}

			Update(new WorkingTimeRegistered(date, minutes, memo));
		}

		private void OnWorkingTimeRegistered(WorkingTimeRegistered @event)
		{
			TotalMinutes += @event.Minutes;
		}
	}
}