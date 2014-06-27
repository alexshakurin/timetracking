using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class HoursTrackingDataViewModel : ViewModel
	{
		public static int start = 0;

		public ObservableCollection<MinutesTrackingDataViewModel> MinutesData { get; private set; }

		public string Hour { get; private set; }

		public HoursTrackingDataViewModel(int hour)
		{
			Hour = TimeSpan.FromHours(hour).ToString(@"hh\:mm");
			MinutesData = new ObservableCollection<MinutesTrackingDataViewModel>();

			MinutesData.Add(new MinutesTrackingDataViewModel(hour, 0 + start, 0));

			start++;

			if ((start + MinutesData.Last().MinutesLength) >= 60)
			{
				start = 0;
			}

			
		}
	}
}