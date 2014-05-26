namespace TimeTracking.LocalStorage
{
	public class TimeTrackingStatistics
	{
		public string Date { get; protected set; }

		public int Seconds { get; protected set; }

		// Required for EntityFramework
		internal TimeTrackingStatistics()
		{
		}

		public TimeTrackingStatistics(string date)
		{
			Date = date;
		}

		public void AddSeconds(int seconds)
		{
			Seconds += seconds;
		}
	}
}