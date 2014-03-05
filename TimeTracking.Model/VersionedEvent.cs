using System;

namespace TimeTracking.Model
{
	public class VersionedEvent : IVersionedEvent
	{
		public Guid SourceId { get; internal set; }

		public int Version { get; internal set; }
	}
}