using System;

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
			// TODO: Load time tracking details for a day

			SelectedDateData = new DayTimeTrackingDetailsViewModel();
		}
	}
}