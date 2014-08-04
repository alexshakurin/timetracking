using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimeTracker.Localization;
using TimeTracking.LocalStorage;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class DayTimeTrackingDetailsViewModel : ViewModel
	{
		public ObservableCollection<HoursTrackingDataViewModel> HoursData { get; private set; }

		private TimeSpan dayTotal;

		public TimeSpan DayTotal
		{
			get { return dayTotal; }
			set
			{
				if (dayTotal == value)
				{
					return;
				}

				dayTotal = value;
				RaisePropertyChanged(() => DayTotal);
			}
		}

		public DayTimeTrackingDetailsViewModel(IReadOnlyCollection<WorkingTimeInterval> intervals,
			TimeSpan dayTotal)
		{
			DayTotal = dayTotal;

			HoursData = new ObservableCollection<HoursTrackingDataViewModel>();

			const int firstHourOfDay = 0;
			const int hoursInDay = 24;

			var intervalsData = intervals.Select(i =>
				{
					var timeSpanStart = TimeSpan.Parse(i.StartTime);
					var timeSpanEnd = TimeSpan.Parse(i.EndTime);

					var startWithoutSeconds = new TimeSpan(timeSpanStart.Hours, timeSpanStart.Minutes, 0);
					var endWithoutSeconds = new TimeSpan(timeSpanEnd.Hours, timeSpanEnd.Minutes, 0);

					return new { Start = startWithoutSeconds, End = endWithoutSeconds, i.Memo };
				})
				.ToList();

			foreach (var hour in Enumerable.Range(firstHourOfDay, hoursInDay))
			{
				var hourIntervalStart = new TimeSpan(hour, 0, 0);
				var hourIntervalEnd = new TimeSpan(hour, 59, 59);

				var viewModels = new List<MinutesTrackingDataViewModel>();

				foreach (var interval in intervalsData)
				{
					var intersection = GetIntersection(hourIntervalStart,
						hourIntervalEnd,
						interval.Start,
						interval.End);

					var totalMinutes = intersection.TotalMinutes;
					if (totalMinutes > 0)
					{
						viewModels.Add(new MinutesTrackingDataViewModel(LocalizationService.Default,
							hourIntervalStart.Hours,
							intersection.Start.Minutes,
							totalMinutes,
							interval.Start,
							interval.End,
							interval.Memo));
					}
				}

				HoursData.Add(new HoursTrackingDataViewModel(hour, viewModels));
			}
		}

		private struct Intersection
		{
			public TimeSpan Start { get; set; }
			public TimeSpan End { get; set; }

			public int TotalMinutes
			{
				get
				{
					return (int)(End - Start).TotalMinutes;
				}
			}
		}

		private Intersection GetIntersection(TimeSpan hourIntervalStart, TimeSpan hourIntervalEnd,
			TimeSpan workingIntervalStart,
			TimeSpan workingIntervalEnd)
		{
			if (workingIntervalStart > hourIntervalEnd)
			{
				return new Intersection();
			}

			if (workingIntervalEnd < hourIntervalStart)
			{
				return new Intersection();
			}

			var intersectionStart = workingIntervalStart > hourIntervalStart
				? workingIntervalStart
				: hourIntervalStart;

			var intersectionEnd = workingIntervalEnd < hourIntervalEnd
				? workingIntervalEnd
				: hourIntervalEnd;

			return new  Intersection{ Start = intersectionStart, End = intersectionEnd };
		}
	}
}