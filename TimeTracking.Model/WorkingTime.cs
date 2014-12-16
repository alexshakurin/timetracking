using System;
using System.Collections.Generic;
using System.Linq;
using TimeTracking.Model.Events;
using TimeTracking.Model.Exceptions;

namespace TimeTracking.Model
{
	public class WorkingTime : EventSourced
	{
		private readonly List<TimeInterval> intervals;

		public TimeSpan Total { get; private set; }

		public WorkingTime(string id)
			: base(id)
		{
			intervals = new List<TimeInterval>();
			Handles<WorkingTimeRegistered>(OnWorkingTimeRegistered);
			Handles<WorkingTimeDeleted>(OnWorkingTimeDeleted);
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

		public void DeleteTime(DateTime date, DateTimeOffset start, DateTimeOffset end)
		{
			var time = end - start;
			if (Total.TotalSeconds == 0 && time.TotalSeconds < 0)
			{
				throw new ArgumentException("Can't remove time because working time is zero");
			}

			Update(new WorkingTimeDeleted(date, start, end));
		}

		private void OnWorkingTimeDeleted(WorkingTimeDeleted @event)
		{
			var targetInterval = new TimeInterval(@event.Start, @event.End);

			var intersections = intervals.Where(t => t.Intersects(targetInterval)).ToList();
			if (intersections.Count == 0)
			{
				throw new IntervalNotBelongToDayException(string.Format("{0} is not recorded for day {1}", @event.ToString(), @event.Date.ToShortDateString()));
			}

			var time = @event.End - @event.Start;
			Total -= time;
			intersections.ForEach(i => intervals.Remove(i));
		}

		private void OnWorkingTimeRegistered(WorkingTimeRegistered @event)
		{
			var newInterval = new TimeInterval(@event.Start, @event.End);

			var firstIntersect = intervals.FirstOrDefault(i => i.Intersects(newInterval) || i.Equals(newInterval));
			if (firstIntersect != null)
			{
				throw new IntervalDuplicateException(string.Format("Unable to register time for interval {0} because it intersects with existing interval {1}",
					newInterval,
					firstIntersect));
			}

			var time = @event.End - @event.Start;
			Total += time;
			intervals.Add(newInterval);
		}
	}
}