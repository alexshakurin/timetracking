
namespace TimeTracking.LocalStorage
{
	public class StoredEvent
	{
		public string AggregateId { get; set; }

		public string AggregateType { get; set; }

		public long Version { get; set; }

		public string Payload { get; set; }

		public string CorrelationId { get; set; }
	}
}