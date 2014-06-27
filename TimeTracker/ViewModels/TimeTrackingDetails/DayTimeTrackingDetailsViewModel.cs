using System.Collections.ObjectModel;
using System.Linq;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class DayTimeTrackingDetailsViewModel : ViewModel
	{
		public ObservableCollection<HoursTrackingDataViewModel> HoursData { get; private set; }

		public DayTimeTrackingDetailsViewModel()
		{
			HoursData = new ObservableCollection<HoursTrackingDataViewModel>();

			foreach (var hour in Enumerable.Range(0, 24))
			{
				HoursData.Add(new HoursTrackingDataViewModel(hour));
			}
		}
	}
}