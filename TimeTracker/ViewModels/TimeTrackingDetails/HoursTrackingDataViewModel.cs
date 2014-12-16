using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class HoursTrackingDataViewModel : ViewModel
	{
		public ObservableCollection<MinutesTrackingDataViewModel> MinutesData { get; private set; }

		public string Hour { get; private set; }

		public HoursTrackingDataViewModel(int hour,
			IReadOnlyCollection<MinutesTrackingDataViewModel> minutesData)
		{
			Hour = TimeSpan.FromHours(hour).ToString(@"hh\:mm");
			MinutesData = new ObservableCollection<MinutesTrackingDataViewModel>(minutesData);
		}
	}
}