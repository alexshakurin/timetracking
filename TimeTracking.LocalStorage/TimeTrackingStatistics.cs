namespace TimeTracking.LocalStorage
{
	public class TimeTrackingStatistics
	{
		public string Date { get; protected set; }

		public double Seconds { get; protected set; }

		public string LatestMemo { get; set; }

		// Required for EntityFramework
		internal TimeTrackingStatistics()
		{
		}

		public TimeTrackingStatistics(string date)
		{
			Date = date;
		}

		public void AddSeconds(double seconds)
		{
			Seconds += seconds;
		}
	}
}