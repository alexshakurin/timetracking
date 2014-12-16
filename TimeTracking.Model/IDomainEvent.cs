
namespace TimeTracking.Model
{
	public interface IDomainEvent
	{
		string SourceId { get; }
	}
}