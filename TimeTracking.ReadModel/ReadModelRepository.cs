using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TimeTracking.LocalStorage;

namespace TimeTracking.ReadModel
{
	public class ReadModelRepository
	{
		private static readonly object syncRoot = new object();

		private readonly Func<EventStoreDbContext> contextFactory;

		public ReadModelRepository(Func<EventStoreDbContext> contextFactory)
		{
			this.contextFactory = contextFactory;
		}

		public void UpdateDayStatistics(string day, TimeSpan duration, string memo)
		{
			lock (syncRoot)
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
					existingStatistics.LatestMemo = memo;
					context.SaveChanges();
				}
			}
		}

		public TimeTrackingStatistics GetStatisticsForDay(string day)
		{
			TimeTrackingStatistics statistics;
			using (var context = contextFactory())
			{
				statistics = context.Set<TimeTrackingStatistics>()
					.FirstOrDefault(tts => tts.Date == day);
			}

			return statistics;
		}

		public TimeSpan GetTotalForPeriods(IReadOnlyCollection<string> periods)
		{
			var periodsList = new List<string>(periods);

			IList<int> statisticsList;
			using (var context = contextFactory())
			{
				statisticsList = context.Set<TimeTrackingStatistics>()
					.Where(s => periodsList.Contains(s.Date))
					.Select(s => s.Seconds)
					.ToList();
			}

			return TimeSpan.FromSeconds(statisticsList.Sum());
		}
	}
}