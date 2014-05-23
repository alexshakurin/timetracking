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
		public DateTimeOffset Start { get; private set; }

		[DataMember]
		public DateTimeOffset End { get; private set; }

		[DataMember]
		public string Memo { get; private set; }

		public WorkingTimeRegistered(DateTime date, DateTimeOffset start, DateTimeOffset end, string memo)
		{
			Date = date.Date;
			Start = start;
			End = end;
			Memo = memo;
		}
	}
}