using System;
using Microsoft.Practices.ServiceLocation;
using TimeTracking.Extensions;
using TimeTracking.ReadModel;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class TimeTrackingDetailsViewModel : ClosableViewModel, ITimeTrackingDetailsViewModel
	{
		private DateTime? selectedDate;

		private DayTimeTrackingDetailsViewModel selectedDateData;

		public DayTimeTrackingDetailsViewModel SelectedDateData
		{
			get { return selectedDateData; }
			private set
			{
				if (selectedDateData == value)
				{
					return;
				}

				selectedDateData = value;
				RaisePropertyChanged(() => SelectedDateData);
			}
		}

		public DateTime? SelectedDate
		{
			get { return selectedDate; }
			set
			{
				if (selectedDate == value)
				{
					return;
				}

				selectedDate = value;
				if (selectedDate.HasValue)
				{
					LoadDataForSelectedDate();
				}

				RaisePropertyChanged(() => SelectedDate);
			}
		}

		private void LoadDataForSelectedDate()
		{
			if (SelectedDate.HasValue)
			{
				var repository = ServiceLocator.Current.GetInstance<ReadModelRepository>();
				var key = TimeTrackingViewModel.GetKey(SelectedDate.Value);
				var intervals = repository.GetIntervalsForDay(key.Key, key.GetDateString());
				var dayStatistics = repository.GetStatisticsForDay(key.GetDateString());

				SelectedDateData = new DayTimeTrackingDetailsViewModel(intervals,
					dayStatistics.Maybe(ds => TimeSpan.FromSeconds(ds.Seconds)));
			}
		}
	}
}