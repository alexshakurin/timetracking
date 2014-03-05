using System;

namespace TimeTracking.Model
{
	public interface IDomainEvent
	{
		Guid SourceId { get; }
	}
}