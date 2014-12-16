using System;
using System.Collections.Generic;
using System.Linq;
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

		public void DeleteMissingKeys(IReadOnlyCollection<string> keys)
		{
			lock (syncRoot)
			{
				using (var context = contextFactory())
				{
					var existingKeys = context.Set<TimeTrackingStatistics>()
						.Select(s => s.Date)
						.Distinct()
						.ToList();

					var missingKeys = existingKeys.Except(keys).ToList();

					var statisticsToDelete = context.Set<TimeTrackingStatistics>()
						.Where(s => missingKeys.Contains(s.Date))
						.ToList();

					context.Set<TimeTrackingStatistics>().RemoveRange(statisticsToDelete);

					context.SaveChanges();
				}
			}
		}

		public void SetDayStatistics(string day, TimeSpan duration)
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

					existingStatistics.SetSeconds(duration.TotalSeconds);
					context.SaveChanges();
				}
			}
		}

		public void RemoveDayStatistics(string day, TimeSpan duration)
		{
			lock (syncRoot)
			{
				using (var context = contextFactory())
				{
					var existingStatistics = context.Set<TimeTrackingStatistics>()
						.FirstOrDefault(t => t.Date == day);

					if (existingStatistics != null)
					{
						existingStatistics.RemoveSeconds(duration.TotalSeconds);

						context.SaveChanges();
					}
				}
			}
		}

		public void AddDayStatistics(string day, TimeSpan duration, string memo)
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

					existingStatistics.AddSeconds(duration.TotalSeconds);
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

			IList<double> statisticsList;
			using (var context = contextFactory())
			{
				statisticsList = context.Set<TimeTrackingStatistics>()
					.Where(s => periodsList.Contains(s.Date))
					.Select(s => s.Seconds)
					.ToList();
			}

			return TimeSpan.FromSeconds(statisticsList.Sum());
		}

		public IReadOnlyCollection<WorkingTimeInterval> GetIntervalsForDay(string key, string day)
		{
			var intervals = new List<WorkingTimeInterval>();

			using (var context = contextFactory())
			{
				intervals.AddRange(context.Set<WorkingTimeInterval>()
					.Where(wti => wti.AggregateId == key && wti.Date == day)
					.ToList());
			}

			return intervals.ToList().AsReadOnly();
		}

		public void AddInterval(string key, string date, TimeSpan start, TimeSpan end, string targetMemo)
		{
			var memo = targetMemo ?? string.Empty;

			using (var context = contextFactory())
			{
				var startTime = start.ToString();
				var endTime = end.ToString();

				var existingInterval = context.Set<WorkingTimeInterval>()
					.FirstOrDefault(wti => wti.AggregateId == key
						&& wti.Date == date
						&& wti.EndTime == startTime
						&& wti.Memo == memo);

				if (existingInterval == null)
				{
					existingInterval = new WorkingTimeInterval(key, date);
					existingInterval.StartTime = startTime;
					existingInterval.Memo = memo;
					existingInterval.EndTime = endTime;
					context.Set<WorkingTimeInterval>().Add(existingInterval);
				}
				else
				{
					existingInterval.EndTime = endTime;
				}

				context.Configuration.ValidateOnSaveEnabled = false;
				context.SaveChanges();
			}
		}
	}
}