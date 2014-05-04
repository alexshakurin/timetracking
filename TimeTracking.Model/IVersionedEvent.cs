namespace TimeTracking.Model
{
	public interface IVersionedEvent : IDomainEvent
	{
		long Version { get; }
	}
}