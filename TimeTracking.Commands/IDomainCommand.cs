using System;

namespace TimeTracking.Commands
{
	public interface IDomainCommand
	{
		Guid CommandId { get; }
	}
}