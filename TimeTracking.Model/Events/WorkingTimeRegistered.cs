using System;
using System.Runtime.Serialization;

namespace TimeTracking.Model.Events
{
	[DataContract]
	public class WorkingTimeRegistered : VersionedEvent
	{
		[DataMember]
		public DateTime Date { get; private set; }

		[DataMember]
		public TimeSpan Time { get; private set; }

		[DataMember]
		public string Memo { get; private set; }

		public WorkingTimeRegistered(DateTime date, TimeSpan time, string memo)
		{
			if (time.TotalSeconds == 0)
			{
				throw new ArgumentException("Total time cannot be zero", "time");
			}

			Date = date.Date;
			Time = time;
			Memo = memo;
		}
	}
}