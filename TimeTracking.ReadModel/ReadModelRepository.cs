using System;
using System.Linq;
using TimeTracking.LocalStorage;

namespace TimeTracking.ReadModel
{
	public class ReadModelRepository
	{
		private readonly Func<EventStoreDbContext> contextFactory;

		public ReadModelRepository(Func<EventStoreDbContext> contextFactory)
		{
			this.contextFactory = contextFactory;
		}

		public void AddDurationForday(string day, TimeSpan duration)
		{
			using (var context = contextFactory())
			{
				var existingStatistics = context.Set<TimeTrackingStatistics>()
					.FirstOrDefault(t => t.Date == day);

				if (existingStatistics == null)
				{
					existingStatistics = new TimeTrackingStatistics(day);
					context.Set<TimeTrackingStatistics>().Add(existingStatistics);
				}

				existingStatistics.AddSeconds((int)duration.TotalSeconds);
				context.SaveChanges();
			}
		}

		public TimeSpan GetDurationForDay(string day)
		{
			var duration = TimeSpan.Zero;
			using (var context = contextFactory())
			{
				var statistics = context.Set<TimeTrackingStatistics>()
					.FirstOrDefault(tts => tts.Date == day);

				if (statistics != null)
				{
					duration = TimeSpan.FromSeconds(statistics.Seconds);
				}
			}

			return duration;
		}
	}
}