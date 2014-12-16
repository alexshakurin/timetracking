using System;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public interface ITimeTrackingDetailsViewModel
	{
		DateTime? SelectedDate { get; set; }

		DayTimeTrackingDetailsViewModel SelectedDateData { get; }
	}
}