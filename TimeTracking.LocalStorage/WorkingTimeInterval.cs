namespace TimeTracking.LocalStorage
{
	public class WorkingTimeInterval
	{
		public string AggregateId { get; protected set; }

		public string Date { get; protected set; }

		public string StartTime { get; set; }

		public string EndTime { get; set; }

		public string Memo { get; set; }

		// Required for EntityFramework
		internal WorkingTimeInterval()
		{
		}

		public WorkingTimeInterval(string aggregateId, string date)
		{
			AggregateId = aggregateId;
			Date = date;
		}
	}
}