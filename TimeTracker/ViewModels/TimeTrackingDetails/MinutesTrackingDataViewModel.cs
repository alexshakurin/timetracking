using System;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class MinutesTrackingDataViewModel : ViewModel
	{
		public int Minute { get; private set; }

		public int MinutesLength { get; private set; }

		public string Memo { get; private set; }

		public string Range { get; private set; }

		public MinutesTrackingDataViewModel(int hour, int minute, int second)
		{
			Minute = minute;
			MinutesLength = 25;

			var rangeStart = new TimeSpan(hour, minute, second);
			var rangeEnd = rangeStart.Add(TimeSpan.FromMinutes(MinutesLength));

			Range = string.Format("{0} - {1}", rangeStart, rangeEnd);

			Memo = "Test memo";
		}
	}
}