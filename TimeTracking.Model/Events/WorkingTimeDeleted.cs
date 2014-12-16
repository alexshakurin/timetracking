using System;
using System.Runtime.Serialization;

namespace TimeTracking.Model.Events
{
	[DataContract]
	public class WorkingTimeDeleted : VersionedEvent
	{
		[DataMember]
		public DateTime Date { get; private set; }

		[DataMember]
		public DateTimeOffset Start { get; private set; }

		[DataMember]
		public DateTimeOffset End { get; private set; }

		public WorkingTimeDeleted(DateTime date, DateTimeOffset start, DateTimeOffset end)
		{
			Date = date.Date;
			Start = start;
			End = end;
		}

		public override string ToString()
		{
			return string.Format("WorkingTimeDeleted: Date '{0}', '{1} - {2}', memo '{3}'",
				Date.ToString("yyyy_MM_dd"),
				Start,
				End);
		}
	}
}