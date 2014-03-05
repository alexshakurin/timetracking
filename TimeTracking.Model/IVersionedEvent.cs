namespace TimeTracking.Model
{
	public interface IVersionedEvent : IDomainEvent
	{
		int Version { get; }
	}
}