using System;
using System.Collections.Generic;
using System.Linq;
using TimeTracking.Model.Events;

namespace TimeTracking.Model
{
	public class WorkingTime : EventSourced
	{
		public DateTimeOffset Start { get; private set; }
		public DateTimeOffset End { get; private set; }

		public TimeSpan Total { get; private set; }

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

		public void RegisterTime(DateTime date, DateTimeOffset start, DateTimeOffset end, string memo)
		{
			var time = end - start;
			if (Total.TotalSeconds == 0 && time.TotalSeconds < 0)
			{
				throw new ArgumentException("Can't add negative time because working time is zero");
			}

			Update(new WorkingTimeRegistered(date, start, end, memo));
		}

		private void OnWorkingTimeRegistered(WorkingTimeRegistered @event)
		{
			var time = @event.End - @event.Start;
			Total += time;
		}
	}
}