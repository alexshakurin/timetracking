
using System.Runtime.Serialization;

namespace TimeTracking.Model
{
	[DataContract]
	public class VersionedEvent : IVersionedEvent
	{
		[DataMember]
		public string SourceId { get; internal set; }

		[DataMember]
		public long Version { get; internal set; }
	}
}