using System;

namespace TimeTracker.ViewModels.TimeTrackingDetails
{
	public class MinutesTrackingDataViewModel : ViewModel
	{
		public int Minute { get; private set; }

		public int MinutesLength { get; private set; }

		public string Memo { get; private set; }

		//public string Range { get; private set; }

		public string Description { get; private set; }

		public MinutesTrackingDataViewModel(int minuteStart,
			int minutesLength,
			TimeSpan rangeStart,
			TimeSpan rangeEnd,
			string memo)
		{
			Minute = minuteStart;
			MinutesLength = minutesLength;

			var range = string.Format("{0} - {1}", rangeStart, rangeEnd);
			Description = string.Format("{0} ({1})", range, memo);

			Memo = memo;
		}
	}
}